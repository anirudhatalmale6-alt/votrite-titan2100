// Product:	VotRite
// Module:  PrinterHandler.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using VotRite.Definition;
using VotRite.UI;
using VotRite.Util;
using System.Text;
using VotRiteBallotDataManager.AppCode;
using VotRite.DBClasses;

namespace VotRite
{
    public enum PrintDoc { RECORD, REPORT, VOID }

    class PrinterHandler
    {
        internal struct RptHeader
        {
            private string board;
            private string address;
            private string title;
            private string ballot;
            private string date;
            private string voter;
            private string machine;
            private bool printed;

            public string Board { get { return board; } set { board = value; } }
            public string Address { get { return address; } set { address = value; } }
            public string Title { get { return title; } set { title = value; } }
            public string Ballot { get { return ballot; } set { ballot = value; } }
            public string Date { get { return date; } set { date = value; } }
            public string Voter { get { return voter; } set { voter = value; } }
            public string Machine { get { return machine; } set { machine = value; } }
            public bool Printed { get { return printed; } set { printed = value; } }
        }

        private Hashtable vars;
        private Session session;
        private const int V_SPACE = 2;
        private const int LARGE_V_SPACE = 5;
        private const float HDR_FONT_SZ = 12f;
        private const float TITLE_FONT_SZ = 10f;
        private const float RECORD_FONT_SZ = 8f;
        private const float VOID_FONT_SZ = 50f;
        private const int FEED_LENGHT = 80;
        private const int PAPER_WIDTH = 400;
        private const float LeftMargin = 5f;
        private const float TopMargin = 25f;

        private PrinterSettings pSettings;
        private byte[] FULL_CUT;
        private byte[] BACKWARD_FEED;
        Font hdrFont;
        Font titleFont;
        Font recFont;
        Font voidFont;
        Thread pThread;
        RptHeader header;
        int groupId = 0;
        int contCount = 0;
        private ReportData reportData;
        bool slatesPrinted = false;
        private bool countyPrinted = false;
        //
        public string voterid = "";
        public string ballotid = "";
        public Ballot ballot;
        public List<Slate> slates = new List<Slate>();
        public List<ContestDefinition> contests = new List<ContestDefinition>();
        public string bflname = "temp_audit_rec.png";
        public string outputImage = "";
        public int charsPerLine = 57;
        bool printnewline = false;
        List<textandfont> mListPrint = new List<textandfont>();
        List<textandfont> mListPrint_Final = new List<textandfont>();
        bool addInPrintList = false;
        int linesprinted = 0;

        Bitmap mainbmp;
       // Graphics tempGr;
        int imgpg = 0;
        string folder = "";

        bool fileprintcomplete = false;
        ExcelExport export = new ExcelExport();
        int cells = 5;
        string reportname = "";
        Graphics tempGr_pdf;
        Graphics grFontSiz;
        PrintPageEventArgs argsMain;
        public static BallotDefinition eballot = new BallotDefinition();
        public bool noVoid = false;

        //  Printer APIs declaration
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        public class textandfont
        {
            public string text { get; set; }
            public Font font { get; set; }
        }

        private class ReportData
        {
            public DataTable Data { get; private set; }

            public ReportData(BallotDefinition ballot)
            {
                var query = "select bid, cid, did, cand_name, vdate, cnt, slate_id, pid " +
                    "from counter where bid=" + ballot.Id + ";";

                query = "select bid, cid, did, cand_name, '"+ DateTime.Now.ToString("d")+"' as vdate, sum(cnt) as cnt, slate_id, pid " +
                    "from counter where bid=" + ballot.Id + " group by bid, cid, did, cand_name, vdate, slate_id, pid;";

                Data = DataManager.VotingResultsDataInstance.GetDataV2(query);
            }

            public ReportData()
            {
                var query = "select bid, cid, did, cand_name, vdate, cnt, slate_id, pid from counter";

                query = "select bid, cid, did, cand_name, '" + DateTime.Now.ToString("d") + "' as vdate, sum(cnt) as cnt, slate_id, pid " +
                   "from counter group by bid, cid, did, cand_name, vdate, slate_id, pid;";

                Data = DataManager.VotingResultsDataInstance.GetDataV2(query);
            }
        }

        [DllImport("winspool.Drv",
            EntryPoint = "OpenPrinterA",
            SetLastError = true,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)]
            string szPrinter,
            out IntPtr hPrinter,
            IntPtr pd);

        [DllImport("winspool.Drv",
            EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv",
            EntryPoint = "StartDocPrinterA",
            SetLastError = true,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter,
            Int32 level,
            [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv",
            EntryPoint = "EndDocPrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv",
            EntryPoint = "StartPagePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv",
            EntryPoint = "EndPagePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv",
            EntryPoint = "WritePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter,
            IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public bool PrintingCompleted { get; private set; }

        public PrinterHandler()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;
            pSettings = new PrinterSettings();
            FULL_CUT = new byte[] { 0x1B, 0x69 };   //  full-cut command
            BACKWARD_FEED = new byte[] { 0x1b, 0x6a, 0xff };
            if (AppManager.Instance.reportFontSizeFactor == 0f)
            {
                hdrFont = new Font("Arial", HDR_FONT_SZ, FontStyle.Bold);
                titleFont = new Font("Arial", TITLE_FONT_SZ, FontStyle.Bold);
                recFont = new Font("Arial", RECORD_FONT_SZ);
                voidFont = new Font("Arial", VOID_FONT_SZ);
            }
            else
            {
                hdrFont = new Font("Arial", HDR_FONT_SZ + AppManager.Instance.reportFontSizeFactor, FontStyle.Bold);
                titleFont = new Font("Arial", TITLE_FONT_SZ + AppManager.Instance.reportFontSizeFactor, FontStyle.Bold);
                recFont = new Font("Arial", RECORD_FONT_SZ + AppManager.Instance.reportFontSizeFactor);
                voidFont = new Font("Arial", VOID_FONT_SZ + AppManager.Instance.reportFontSizeFactor);

                charsPerLine -= 2 * (int)AppManager.Instance.reportFontSizeFactor;
            }
        }

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        private bool SendBytesToPrinter(string szPrinterName,
            IntPtr pBytes,
            Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;  //  Assume failure unless 
            //  you specifically succeed.

            di.pDocName = "VotRite";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
                dwError = Marshal.GetLastWin32Error();
            return bSuccess;
        }

        private bool SendStringToPrinter(string szPrinterName,
            string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        public void CutPaperGeneric()
        {
            /// Jim Kapsis 18-06-2016
            /// Printer Settings
            if (!AppManager.PrinterEnabled) return;

            try
            {
                int len = FULL_CUT.Length;
                // Allocate some unmanaged memory for cut command.
                IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(len);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(FULL_CUT, 0, pUnmanagedBytes, len);
                // Send the unmanaged bytes to the printer.
                bool success = SendBytesToPrinter(pSettings.PrinterName,
                    pUnmanagedBytes, len);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        public void CutPaper()
        {
            try
            {


                // temp fix to address cutting
                // temp check if printer is a TSC printer
                // TODO: change to generic
                if (pSettings.PrinterName.Contains("TSC"))
                {
                    // use TSC specific SDK
                    TSCLIB_DLL tscDll = new TSCLIB_DLL(pSettings.PrinterName);
                    tscDll.CutPaper();
                    printnewline = true;
                }
                else
                {
                    //CutPaperGeneric();
                    printnewline = false;
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        private void FeedPaper()
        {
            /// Jim Kapsis 18-06-2016
            /// Printer Settings
            if (!AppManager.PrinterEnabled) return;

            try
            {
                int start = 0;
                int end = (int)Math.Ceiling(FEED_LENGHT * 2.3);
                byte max = 0xff;
                bool success;
                IntPtr pUnmanagedBytes;
                int len;

                while (start < end)
                {
                    if (end - start >= max) max = 0xff;
                    else max = (byte)(end - start);

                    start += max;
                    BACKWARD_FEED[2] = max;

                    len = BACKWARD_FEED.Length;
                    // Allocate some unmanaged memory for cut command.
                    pUnmanagedBytes = Marshal.AllocCoTaskMem(len);
                    // Copy the managed byte array into the unmanaged array.
                    Marshal.Copy(BACKWARD_FEED, 0, pUnmanagedBytes, len);
                    // Send the unmanaged bytes to the printer.
                    success = SendBytesToPrinter(pSettings.PrinterName,
                        pUnmanagedBytes, len);
                    // Free the unmanaged memory that you allocated earlier.
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        private void PrintEmptyReport(object sender, PrintPageEventArgs args)
        {
            Logger.Instance.Write("printing empty report");

            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            StringFormat format = new StringFormat();

            /// Jim Kapsis 18-06-2016 Excel export 
            int cells = 5;
            ExcelExport export = new ExcelExport();
            export.Create();

            Graphics gOut = args.Graphics;
            //Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX),
            //    (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);               // (int)(args.PageBounds.Height / 100 * gOut.DpiY * 2), gOut);
            Bitmap bmp = null;
            bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);
            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            Graphics tempGr = Graphics.FromImage(bmp);
            //tempGr.FillRegion(new SolidBrush(Color.Green), new Region(new Rectangle(0, 0, (int)(args.Graphics.DpiX), (int)(args.Graphics.DpiY))));

            string sZeroInfo = "Machine#:" + AppManager.Configuration["System"]["Machine"] + "  " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "  Cast: 0";
            WriteText(ref tempGr, ref args, sZeroInfo, ref yPos, leftMargin, hdrFont, format);
            /// Jim Kapsis 18-06-2016 Excel export 
            export.AddRowData(cells, sZeroInfo, bold: true);

            // Jim Kapsis 18-06-2016 Excel export 
            export.SaveAndClose();
            export.Dispose();

            string fileName = DateTime.Now.ToString("MMddyyyyHHmmss.ffff") + ".png";
            Logger.Instance.Write("printing into temp image complete");

            try
            {
                string reportsFolderPath = Global.Instance.APP_PATH + "\\reports\\";

                if (!Directory.Exists(reportsFolderPath))
                    Directory.CreateDirectory(reportsFolderPath);

                bmp.Save(reportsFolderPath + fileName);
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            try
            {
                string backupFlashDriveReportsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] + "\\reports\\";

                if (!Directory.Exists(backupFlashDriveReportsPath))
                    Directory.CreateDirectory(backupFlashDriveReportsPath);

                bmp.Save(backupFlashDriveReportsPath + fileName);
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            /// Jim Kapsis. 
            /// if printer enabled
            if (AppManager.PrinterEnabled)
            {
                gOut.DrawImage(bmp, 0f, 0f);
            }

            args.HasMorePages = false;
            contCount = 0;
            header.Printed = false;
            slatesPrinted = false;
            bmp.Dispose();
            tempGr.Dispose();
            gOut.Dispose();
        }

        private string[] GetDateRange(DataTable pTbDate)
        {
            string[] sRet = new string[2] { "", "" };
            for (int i = 0; i < pTbDate.Rows.Count; i++)
            {
                var dd = pTbDate.Rows[i]["vdate"].ToString();
                if (dd.Contains("/"))
                    dd = dd.Split('/')[2] + "-" + dd.Split('/')[0] + "-" + dd.Split('/')[1];
                if (sRet[0] == "")
                {
                    sRet[0] = DateTime.Parse(dd).ToString("yyyy-MM-dd");
                }
                if (sRet[1] == "")
                {
                    sRet[1] = DateTime.Parse(dd).ToString("yyyy-MM-dd");
                }
                if (Int64.Parse(DateTime.Parse(sRet[0]).ToString("yyyyMMdd")) > Int64.Parse(DateTime.Parse(dd).ToString("yyyyMMdd")))
                {
                    sRet[0] = DateTime.Parse(dd).ToString("yyyy-MM-dd");
                }
                if (Int64.Parse(DateTime.Parse(sRet[1]).ToString("yyyyMMdd")) < Int64.Parse(DateTime.Parse(dd).ToString("yyyyMMdd")))
                {
                    sRet[1] = DateTime.Parse(dd).ToString("yyyy-MM-dd");
                }
            }

            if (!string.IsNullOrEmpty(sRet[0]))
            {
                sRet[0] = "Start: " + DateTime.Parse(sRet[0]).ToString("dddd, MMMM dd, yyyy");
            }

            if (!string.IsNullOrEmpty(sRet[1]))
            {
                sRet[1] = "End: " + DateTime.Parse(sRet[1]).ToString("dddd, MMMM dd, yyyy");
            }

            return sRet;
        }

        private void PrintRecord(object sender, PrintPageEventArgs args)
        {
            //PrintPageEventArgs args = argsMain;
            Logger.Instance.Write("PrintRecord");
            addInPrintList = true;
            mListPrint = new List<textandfont>();
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            StringFormat format = new StringFormat();

            Logger.Instance.Write("start printing record");

            if (contCount < session.Ballot.ContestsList.Count)
                args.HasMorePages = true;

            Graphics gOut = args.Graphics;
            Bitmap bmp = mainbmp;

            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            //bmp.SetResolution(2400, 6000);
            Graphics tempGr = Graphics.FromImage(bmp);
            //tempGr_pdf = tempGr;
            //export = new ExcelExport();
            //export.gr = tempGr_pdf= tempGr;
            string bflname = session.Id + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
            //export.CreateRecordsExcel(bflname);
            grFontSiz = args.Graphics;
            if (!header.Printed)
            {
                Logger.Instance.Write("printing header");
                //  Header

                // temp, adding space
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                WriteText(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                //export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                //export.AddRowData(cells, "");
                WriteText(ref tempGr, ref args, header.Board, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                //export.AddRowData(cells, header.Board, bold: true, cellsUnderline: true, centerAlign: false);

                if (session.Ballot.HasCounties)
                {
                    WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                              leftMargin, hdrFont, format);
                    export.AddRowData(cells, session.Ballot.Address, bold: true, cellsUnderline: true, centerAlign: false);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                }

                WriteText(ref tempGr, ref args, header.Title, ref yPos,
                          leftMargin, hdrFont, format);
                if (header.Title != null)
                    export.AddRowData(cells, header.Title, bold: true, cellsUnderline: true, centerAlign: false);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                //export.AddRowData(cells, header.Ballot.Split('\n')[0], bold: true, cellsUnderline: true, centerAlign: false);
                //export.AddRowData(cells, header.Ballot.Split('\n')[1], bold: true, cellsUnderline: true, centerAlign: false);

                WriteText(ref tempGr, ref args, header.Date, ref yPos,
                          leftMargin, hdrFont, format);
                //export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Voter, ref yPos,
                          leftMargin, hdrFont, format);
                //export.AddRowData(cells, header.Voter, bold: true, cellsUnderline: true, centerAlign: false);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);
                //export.AddRowData(cells, header.Machine, bold: true, cellsUnderline: true, centerAlign: false);
                //export.AddRowData(cells, "======VERIFIED BY VOTER======", bold: true, cellsUnderline: false, centerAlign: false);
                WriteText(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

                //  End of header

                header.Printed = true;
            }

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            //slates
            if (session.Ballot.slatesDefinition.Slates.Count > 0 && session.Ballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                WriteText(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                //export.AddRowData(cells, "SLATES", bold: true, cellsUnderline: true, centerAlign: true);

                Slate slate = session.Ballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == session.Ballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                    WriteText(ref tempGr, ref args, slate.Name, ref yPos, leftMargin, recFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, slate.Name, bold: true, cellsUnderline: true, centerAlign: true);
                }
                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);
                //export.AddRowData(cells, "");

                slatesPrinted = true;
            }
            //slates

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            ContestDefinition contest;
            string text;
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            for (int i = contCount; i < session.Ballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + contCount);
                contest = session.Ballot.ContestsList[i];

                if (session.Ballot.slatesDefinition.Slates.Count > 0 && session.Ballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    if (contest.Propositions == null)
                    {
                        var contestHeight = contest.Selected > 0
                                                ? contest.Selected *
                                                  (V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height)
                                                : V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height;
                    }

                    WriteText(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], bold: true, cellsUnderline: false, centerAlign: false);

                    yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    WriteText(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"],
                              ref yPos,
                              leftMargin, titleFont, format);
                    ////export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"], bold: true, cellsUnderline: false, centerAlign: false);
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                    if (contest.Selected == 0)
                    {
                        WriteText(ref tempGr, ref args,
                                  (string)vars["no_selection"], ref yPos, leftMargin, recFont, format);
                        //export.AddRowData(cells, (string)vars["no_selection"], bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                    }
                    else
                    {
                        if (session.Ballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (session.Ballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] +
                                               ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);
                                    //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        WriteText(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);
                                        //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                }
                            }

                        }

                    }
                }
                else
                {
                    if (((yPos + titleFont.GetHeight(args.Graphics) +
                          (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5))
                    {
                        //break;                        
                    }
                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        WriteText(ref tempGr, ref args, proposition.Title.Trim(), ref yPos, leftMargin, titleFont, format);
                        //export.AddRowData(cells, proposition.Title.Trim(), bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            WriteText(ref tempGr, ref args, (string)vars["no_selection"], ref yPos, leftMargin, recFont,
                                      format);
                            //export.AddRowData(cells, (string)vars["no_selection"], bold: false, cellsUnderline: false, centerAlign: false);

                            yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;

                if (contCount < session.Ballot.ContestsList.Count && contest.Propositions == null)
                {
                    tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);

                    yPos += V_SPACE;
                }
            }

            string fileName = session.Id + "_" + imgpg + ".png";
            if (linesprinted == 0)
            {
                Logger.Instance.Write("printing into temp image complete");
                try
                {
                    //export.SaveAndClose();
                    //export.Dispose();
                    //pdfCreate(bflname,"\\records");
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string recordsFolderPath = Global.Instance.APP_PATH + "\\records\\";
                    if (!Directory.Exists(recordsFolderPath))
                        Directory.CreateDirectory(recordsFolderPath);

                    bmp.Save(recordsFolderPath + fileName);
                    //  outputImage = recordsFolderPath + bflname;
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\records\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    bmp.Save(reportsFolderPath + fileName);
                }
                catch (Exception e) { Logger.Instance.Write(e); }

                try
                {
                    string backupFlashDriveRecordsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] +
                                                         "\\records\\";
                    if (!Directory.Exists(backupFlashDriveRecordsPath))
                        Directory.CreateDirectory(backupFlashDriveRecordsPath);

                    bmp.Save(backupFlashDriveRecordsPath + fileName);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                Logger.Instance.Write("draw printable image complete");
                //PrintingCompleted = true;
            }
        }

        private void PrintRecord_to_PDF_Printer(object sender, PrintPageEventArgs args)
        {
            argsMain = args;            

            Graphics gOut = args.Graphics;
            Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);
            //Bitmap bmp = new Bitmap(2400, 6000, gOut);
            mainbmp = bmp;

            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            //bmp.SetResolution(2400, 6000);
            Graphics tempGr = Graphics.FromImage(bmp);
            tempGr_pdf = tempGr;
            //export = new ExcelExport();
            //export.gr = tempGr_pdf= tempGr;
            string bflname = session.Id + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
            //export.CreateRecordsExcel(bflname);
            grFontSiz = args.Graphics;

            Logger.Instance.Write("PrintRecord");
            addInPrintList = true;
            mListPrint = new List<textandfont>();
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            StringFormat format = new StringFormat();
            if (fileprintcomplete)
                goto PrintToPrinter;

            Logger.Instance.Write("start printing record");
            export = new ExcelExport();
            export.gr = tempGr_pdf= args.Graphics;// tempGr_pdf;

            export.CreateRecordsExcel(bflname);

            if (!header.Printed)
            {
                Logger.Instance.Write("printing header");
                //  Header

                // temp, adding space

                export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                export.AddRowData(cells, "");

                export.AddRowData(cells, header.Board, bold: true, cellsUnderline: true, centerAlign: false);

                if (eballot.HasCounties)
                {
                    export.AddRowData(cells, eballot.Address, bold: true, cellsUnderline: true, centerAlign: false);

                }

                if (header.Title != null)
                    export.AddRowData(cells, header.Title, bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Ballot.Split('\n')[0], bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, header.Ballot.Split('\n')[1], bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Voter, bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, header.Machine, bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, "======VERIFIED BY VOTER======", bold: true, cellsUnderline: false, centerAlign: false);

                header.Printed = true;
            }

            //slates
            if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                export.AddRowData(cells, "SLATES", bold: true, cellsUnderline: true, centerAlign: true);

                Slate slate = eballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == eballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    export.AddRowData(cells, "");
                    export.AddRowData(cells, slate.Name, bold: true, cellsUnderline: true, centerAlign: true);
                }
                export.AddRowData(cells, "");

                slatesPrinted = true;
            }
            //slates
            ContestDefinition contest;
            string text;
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            for (int i = contCount; i < eballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + contCount);
                contest = eballot.ContestsList[i];

                if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    export.AddRowData(cells, "");
                    export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], bold: true, cellsUnderline: false, centerAlign: false);
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"], bold: true, cellsUnderline: false, centerAlign: false);

                    if (contest.Selected == 0)
                    {
                        //export.AddRowData(cells, (string)vars["no_selection"], bold: true, cellsUnderline: false, centerAlign: false);
                        export.AddRowData(cells, "No selection made", bold: true, cellsUnderline: false, centerAlign: false);

                    }
                    else
                    {
                        if (eballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (eballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] +
                                               ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                    }
                                }
                            }

                        }

                    }
                }
                else
                {

                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        export.AddRowData(cells, proposition.Title.Trim(), bold: true, cellsUnderline: false, centerAlign: false);

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            //export.AddRowData(cells, (string)vars["no_selection"], bold: false, cellsUnderline: false, centerAlign: false);
                            export.AddRowData(cells, "No selection made", bold: false, cellsUnderline: false, centerAlign: false);
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;


            }

            string fileName = session.Id + "_" + imgpg + ".png";

                try
                {
                    export.SaveAndClose();
                    export.Dispose();
                    pdfCreate(bflname, "records");
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\records\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    // File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\records", bflname + ".xlsx"), Path.Combine(reportsFolderPath, bflname + ".xlsx"));
                    File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\records", bflname + ".pdf"), Path.Combine(reportsFolderPath, bflname + ".pdf"));
                }
                catch (Exception)
                {

                }
                
             //   PrintingCompleted = true;
            /// Jim Kapsis.
            /// Printer Settings
            //tempGr_pdf.Dispose();
            Logger.Instance.Write("record pdf printing complete");
            header.Printed = false;
            slatesPrinted = false;
            contCount = 0;
            linesprinted = 0;

            //PrintPageEventArgs args = argsMain;
            Logger.Instance.Write("PrintRecord");
            addInPrintList = true;
            mListPrint = new List<textandfont>();
            //float leftMargin = LeftMargin;
            yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            format = new StringFormat();

            Logger.Instance.Write("start printing record");

            if (contCount < eballot.ContestsList.Count)
                args.HasMorePages = true;

            tempGr = args.Graphics;

            if (!header.Printed)
            {
                Logger.Instance.Write("printing header");
                //  Header

                // temp, adding space
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                WriteText_toPrinter(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, header.Board, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                if (eballot.HasCounties)
                {
                    WriteText_toPrinter(ref tempGr, ref args, eballot.Address, ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                }

                WriteText_toPrinter(ref tempGr, ref args, header.Title, ref yPos,
                          leftMargin, hdrFont, format);
                
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, header.Date, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, header.Voter, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, header.Machine, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                WriteText_toPrinter(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

                //  End of header

                header.Printed = true;
            }

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            //slates
            if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                WriteText_toPrinter(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                
                Slate slate = eballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == eballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                    WriteText_toPrinter(ref tempGr, ref args, slate.Name, ref yPos, leftMargin, recFont, format);
                }
                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                //tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                //                    args.PageBounds.Left + leftMargin, yPos,
                //                    args.PageBounds.Right - leftMargin, yPos);                
                slatesPrinted = true;
            }
            //slates

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            for (int i = contCount; i < eballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + contCount);
                contest = eballot.ContestsList[i];

                if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    if (contest.Propositions == null)
                    {
                        var contestHeight = contest.Selected > 0
                                                ? contest.Selected *
                                                  (V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height)
                                                : V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height;
                    }

                    WriteText_toPrinter(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], ref yPos,
                              leftMargin, hdrFont, format);
                    
                    yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    WriteText_toPrinter(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"],
                              ref yPos,
                              leftMargin, titleFont, format);
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                    if (contest.Selected == 0)
                    {
                        WriteText_toPrinter(ref tempGr, ref args,"No selection made", ref yPos, leftMargin, recFont, format);
                        
                        yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                    }
                    else
                    {
                        if (eballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                WriteText_toPrinter(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                
                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (eballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] +
                                               ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    WriteText_toPrinter(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);
                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        WriteText_toPrinter(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                }
                            }

                        }

                    }
                }
                else
                {
                    if (((yPos + titleFont.GetHeight(args.Graphics) +
                          (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5))
                    {
                        //break;                        
                    }
                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        WriteText_toPrinter(ref tempGr, ref args, proposition.Title.Trim(), ref yPos, leftMargin, titleFont, format);
                        
                        yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            WriteText_toPrinter(ref tempGr, ref args, "No selection made", ref yPos, leftMargin, recFont,
                                      format);
                            
                            yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                WriteText_toPrinter(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                
                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;

                if (contCount < eballot.ContestsList.Count && contest.Propositions == null)
                {
                    //tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                    //                args.PageBounds.Left + leftMargin, yPos,
                    //                args.PageBounds.Right - leftMargin, yPos);

                    yPos += V_SPACE;
                }
            }

            fileName = session.Id + "_" + imgpg + ".png";
            int xcontCount = contCount;
            contCount = 0;
            if (!fileprintcomplete)
            {
                //PrintRecord_img();
                PrintRecord_img_English();
            }
            fileprintcomplete = true;
           
            contCount = xcontCount;
            //bool vbal = false;
            //if (!bool.TryParse(AppManager.Configuration["Printer"]["Enabled"], out vbal))
            //    vbal = false;
            PrintToPrinter:
            if (AppManager.Instance.printRecordImageonly)
                goto EndPrinting;

            if (AppManager.PrinterEnabled && AppManager.Configuration["Printer"]["Enabled"] == "true")
            {
                tempGr = args.Graphics;
                // gOut.DrawImage(bmp, 0f, 0f);
                argsMain.Graphics.Clear(Color.Transparent);
                args.Graphics.Clear(Color.Transparent);
                tempGr.Clear(Color.Transparent);
                tempGr_pdf.Clear(Color.Transparent);
                float fontheight = 0;
                float n_finalpos = 0;
                float n_leftMargin = 10;// args.MarginBounds.Left;
                float topMargin = 10;// args.MarginBounds.Top;
                String line = null;
                float n_yPos = topMargin;
                int totallines = mListPrint.Count;
                

                List<textandfont> mListPrint_n = new List<textandfont>();
                int inx_in = -1;
                Dictionary<int, List<textandfont>> dictPrint = new Dictionary<int, List<textandfont>>();

                if (linesprinted > 0)
                    mListPrint = mListPrint_Final;
                else
                    mListPrint_Final = mListPrint;

                for (int i = 0; i < mListPrint.Count; i++)
                {

                    mListPrint_n.Add(mListPrint[i]);
                    line = mListPrint[i].text.Trim();
                    fontheight = mListPrint[i].font.GetHeight(args.Graphics);
                    n_yPos += fontheight;
                    //linesprinted += 1;
                    // if last line position exceeds the visible area
                    if (i + 1 < mListPrint.Count)
                    {
                        if (n_yPos + 10 + mListPrint[i + 1].font.GetHeight(args.Graphics) >= args.Graphics.VisibleClipBounds.Height)// && linesprinted < totallines)
                        {
                            n_yPos = topMargin;
                            inx_in++;
                            dictPrint.Add(inx_in, mListPrint_n);
                            mListPrint_n = new List<textandfont>();
                        }
                    }
                    //else
                    //{
                    //    args.HasMorePages = false;

                    //}
                }
                if (mListPrint_n.Count > 0)
                {
                    n_yPos = topMargin;
                    inx_in++;
                    dictPrint.Add(inx_in, mListPrint_n);
                    mListPrint_n = new List<textandfont>();
                }

                
                if (linesprinted > 0)
                {
                    for (int d = 0; d < linesprinted; d++)
                    {
                        dictPrint.Remove(dictPrint.Count - 1);
                    }
                }

                mListPrint = dictPrint[dictPrint.Count - 1];

                for (int i = 0; i < mListPrint.Count; i++)
                {
                    line = mListPrint[i].text.Trim();

                    int ix = line.IndexOf("  ");

                    if (ix > 0)
                    {
                        line = line.Insert(ix, " / ");

                        var words = line.Split(' ');
                        string txt = "";
                        foreach (string word in words)
                        {
                            txt += (word == " " || word == "" ? "" : word + " ");
                        }
                        line = txt;
                    }
                 
                    if (mListPrint[i].text == "--Line--")
                        // args.Graphics.DrawString(" ", mListPrint[i].font, System.Drawing.Brushes.Black, n_leftMargin, n_yPos, new System.Drawing.StringFormat());
                        //mListPrint[i].text = "--Line--";
                        args.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)), args.PageBounds.Left + leftMargin, n_yPos, args.PageBounds.Right - leftMargin, n_yPos);
                    else
                        args.Graphics.DrawString(line, mListPrint[i].font, System.Drawing.Brushes.Black, n_leftMargin, n_yPos, new System.Drawing.StringFormat());
                    fontheight = mListPrint[i].font.GetHeight(args.Graphics);
                    n_yPos += fontheight;

                }
                linesprinted += 1;
                if (inx_in + 1 > linesprinted)
                {
                    args.HasMorePages = true;
                    header.Printed = false;
                }
                else
                {
                    args.HasMorePages = false;
                    mListPrint_Final = new List<textandfont>();
                    contCount = 0;
                    header.Printed = false;
                    slatesPrinted = false;
                    bmp.Dispose();
                    tempGr.Dispose();
                    gOut.Dispose();
                    tempGr_pdf.Dispose();
                }

            }
            else
            {
                if (contCount == eballot.ContestsList.Count || eballot.slatesDefinition.Data.SlateId > 0)
                {
                    args.HasMorePages = false;
                    Logger.Instance.Write("printing complete");
                    header.Printed = false;
                    slatesPrinted = false;
                    contCount = 0;
                    bmp.Dispose();
                    tempGr.Dispose();
                    gOut.Dispose();
                    tempGr_pdf.Dispose();
                }
            }

            EndPrinting:
            PrintingCompleted = true;
        }

        private void PrintRecord_to_PDF_Printer_NotEnabled()
        {
            //argsMain = args;

            //Graphics gOut = args.Graphics;
            //Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);
            Bitmap bmp = new Bitmap(2400, 6000);
            mainbmp = bmp;

            //bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            bmp.SetResolution(2400, 6000);
            Graphics tempGr = Graphics.FromImage(bmp);
            tempGr_pdf = tempGr;
            //export = new ExcelExport();
            //export.gr = tempGr_pdf= tempGr;
            string bflname = session.Id + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
            //export.CreateRecordsExcel(bflname);
            //grFontSiz = args.Graphics;

            Logger.Instance.Write("PrintRecord");
            addInPrintList = true;
            mListPrint = new List<textandfont>();
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            StringFormat format = new StringFormat();
            if (fileprintcomplete)
                goto PrintToPrinter;

            Logger.Instance.Write("start printing record");
            export = new ExcelExport();
            //export.gr = tempGr_pdf = args.Graphics;// tempGr_pdf;

            export.CreateRecordsExcel(bflname);

            if (!header.Printed)
            {
                Logger.Instance.Write("printing header");
                //  Header

                // temp, adding space

                export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                export.AddRowData(cells, "");

                export.AddRowData(cells, header.Board, bold: true, cellsUnderline: true, centerAlign: false);

                if (eballot.HasCounties)
                {
                    export.AddRowData(cells, eballot.Address, bold: true, cellsUnderline: true, centerAlign: false);

                }

                if (header.Title != null)
                    export.AddRowData(cells, header.Title, bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Ballot.Split('\n')[0], bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, header.Ballot.Split('\n')[1], bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);

                export.AddRowData(cells, header.Voter, bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, header.Machine, bold: true, cellsUnderline: true, centerAlign: false);
                export.AddRowData(cells, "======VERIFIED BY VOTER======", bold: true, cellsUnderline: false, centerAlign: false);

                header.Printed = true;
            }

            //slates
            if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                export.AddRowData(cells, "SLATES", bold: true, cellsUnderline: true, centerAlign: true);

                Slate slate = eballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == eballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    export.AddRowData(cells, "");
                    export.AddRowData(cells, slate.Name, bold: true, cellsUnderline: true, centerAlign: true);
                }
                export.AddRowData(cells, "");

                slatesPrinted = true;
            }
            //slates
            ContestDefinition contest;
            string text;
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            for (int i = contCount; i < eballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + contCount);
                contest = eballot.ContestsList[i];

                if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    export.AddRowData(cells, "");
                    export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], bold: true, cellsUnderline: false, centerAlign: false);
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"], bold: true, cellsUnderline: false, centerAlign: false);

                    if (contest.Selected == 0)
                    {
                        //export.AddRowData(cells, (string)vars["no_selection"], bold: true, cellsUnderline: false, centerAlign: false);
                        export.AddRowData(cells, "No selection made", bold: true, cellsUnderline: false, centerAlign: false);

                    }
                    else
                    {
                        if (eballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (eballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] +
                                               ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                    }
                                }
                            }

                        }

                    }
                }
                else
                {

                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        export.AddRowData(cells, proposition.Title.Trim(), bold: true, cellsUnderline: false, centerAlign: false);

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            //export.AddRowData(cells, (string)vars["no_selection"], bold: false, cellsUnderline: false, centerAlign: false);
                            export.AddRowData(cells, "No selection made", bold: false, cellsUnderline: false, centerAlign: false);
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;


            }

            string fileName = session.Id + "_" + imgpg + ".png";

            try
            {
                export.SaveAndClose();
                export.Dispose();
                pdfCreate(bflname, "records");
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            try
            {
                string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\records\\";

                if (!Directory.Exists(reportsFolderPath))
                    Directory.CreateDirectory(reportsFolderPath);

                // File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\records", bflname + ".xlsx"), Path.Combine(reportsFolderPath, bflname + ".xlsx"));
                File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\records", bflname + ".pdf"), Path.Combine(reportsFolderPath, bflname + ".pdf"));
            }
            catch (Exception)
            {

            }

            //   PrintingCompleted = true;
            /// Jim Kapsis.
            /// Printer Settings
            //tempGr_pdf.Dispose();
            
            fileName = session.Id + "_" + imgpg + ".png";
            int xcontCount = contCount;
            contCount = 0;
            if (!fileprintcomplete)
            {
                PrintRecord_img();
                PrintRecord_img_English();
            }
            fileprintcomplete = true;

            contCount = xcontCount;
            //bool vbal = false;
            //if (!bool.TryParse(AppManager.Configuration["Printer"]["Enabled"], out vbal))
            //    vbal = false;
            PrintToPrinter:
            
            PrintingCompleted = true;
        }

        private void PrintRecord_img()//(object sender, PrintPageEventArgs args)
        {
            PrintPageEventArgs args = argsMain;
            Logger.Instance.Write("PrintRecord");
            addInPrintList = false;
            //mListPrint = new List<textandfont>();
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            StringFormat format = new StringFormat();

            Logger.Instance.Write("start printing record");

            if (AppManager.Instance.Session != null)
            {
                session = AppManager.Instance.Session;
                Logger.Instance.Write("start printing for session: " + session.Id);
            }
            

            BallotDefinition ballot = AppManager.Instance.ballot;
            vars.Clear();

            if (session != null)
            {
                vars.Add("void", session.CurrentLocale.Void);
                vars.Add("voter", session.CurrentLocale.Voter);
                vars.Add("machine", session.CurrentLocale.Machine);
                vars.Add("no_selection", session.CurrentLocale.NoSelection);
                vars.Add("you_wrote", string.Empty);

                header.Board = session.Ballot.Board;
                header.Address = session.Ballot.Address;
                header.Voter = (string)vars["voter"] +
                               ": " + Convert.ToString(session.Id);
                header.Ballot = session.Ballot.Name + "\n" + session.Ballot.Title;
                header.Date = session.Ballot.Date;

                Logger.Instance.Write("printing " + header.Ballot);
            }
            else
            {
                vars.Add("ballot_board", ballot.Board);
                vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                vars.Add("ballot_address", ballot.Address);
                vars.Add("ballot_date", ballot.Date);
                vars.Add("machine", "Machine");

                header.Board = (string)vars["ballot_board"];
                header.Address = (string)vars["ballot_address"];
                header.Title = header.Address;
                header.Ballot = (string)vars["ballot_name"];
                header.Date = (string)vars["ballot_date"];

                if (header.Date == "")
                    header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                Logger.Instance.Write("printing " + header.Ballot);
            }

            var dataCount = 0;
            var selectedDataCount = 0;

            ContestDefinition condef = null;

            foreach (var contest1 in ballot.ContestsList)
            {
                condef = contest1;
                List<Locale> locales;
                if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                    locales = new List<Locale>();
                Locale reportLocale;
                int currentReportLocaleId = -1;
                if (ballot.ReportLocaleId != null)
                    currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);
                if (currentReportLocaleId > -1)
                    reportLocale = locales[currentReportLocaleId];
                else
                    reportLocale = locales[0];

                Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);
                if (defaultLocale == null)
                    return;

               
                if (session != null)
                {
                    defaultLocale = session.CurrentLocale;
                }

                Race currentRaceInstance = null;
                Proposition currentPropositionInstance = null;

                if (ballot.HasSlates)
                {
                    ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale, null, true);
                }
                else
                {
                    ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale);
                }

                ContestDefinition.FillContestData(ref condef, currentPropositionInstance, defaultLocale);

                dataCount += contest1.Data.Count;

                var selectedDataCountLocal = contest1.Data.Where(x => x.State == VrSelection.SelectionState.SELECTED ? true : x.Preference > 0).Count();
                selectedDataCount += selectedDataCountLocal < 1 ? 1 : selectedDataCountLocal;

                if (contest1.Propositions == null)
                {
                    var currentKey = "$b" + ballot.Id + "_g" + contest1.Group + "_name";
                    if (!vars.Contains(currentKey))
                        vars.Add(currentKey, contest1.GroupName);

                    currentKey = "$b" + ballot.Id + "_c" + contest1.Id + "_name";

                    if (!vars.Contains(currentKey))
                        vars.Add(currentKey, contest1.Name);

                    foreach (DataDefinition data in contest1.Data)
                    {
                        currentKey = "$b" + ballot.Id + "_c" + contest1.Id + "_g" + contest1.Group + "_d" + data.Id;

                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, data.Text);
                    }
                }
            }

            header.Machine = (string)vars["machine"] +
                             ": " + AppManager.Configuration["System"]["Machine"];

            //Logger.Instance.Write("printing " + doc);


            if (contCount < session.Ballot.ContestsList.Count)
                args.HasMorePages = true;

            Bitmap bmp = mainbmp;

            Graphics tempGr = Graphics.FromImage(bmp);
            addInPrintList = false;
           
            string bflname = session.Id + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
           
            //if (!header.Printed )
            //{
                Logger.Instance.Write("printing header");
                //  Header

                // temp, adding space
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
                WriteText(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

                WriteText(ref tempGr, ref args, header.Board, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

                if (session.Ballot.HasCounties)
                {
                    WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, session.Ballot.Address, bold: true, cellsUnderline: true, centerAlign: false);
                    yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
                }

                WriteText(ref tempGr, ref args, header.Title, ref yPos,
                          leftMargin, hdrFont, format);
                
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

                WriteText(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(grFontSiz);

                WriteText(ref tempGr, ref args, header.Date, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

                WriteText(ref tempGr, ref args, header.Voter, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

                WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE * 10 + titleFont.GetHeight(grFontSiz);
                WriteText(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

                //  End of header

                header.Printed = true;
            //}

            yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

            //slates
            if (session.Ballot.slatesDefinition.Slates.Count > 0 && session.Ballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                WriteText(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                //export.AddRowData(cells, "SLATES", bold: true, cellsUnderline: true, centerAlign: true);

                Slate slate = session.Ballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == session.Ballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;
                    WriteText(ref tempGr, ref args, slate.Name, ref yPos, leftMargin, recFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, slate.Name, bold: true, cellsUnderline: true, centerAlign: true);
                }
                yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;
                tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);
                //export.AddRowData(cells, "");

                slatesPrinted = true;
            }
            //slates

            yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

            ContestDefinition contest;
            string text;
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            for (int i = contCount; i < session.Ballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + contCount);
                contest = session.Ballot.ContestsList[i];

                if (session.Ballot.slatesDefinition.Slates.Count > 0 && session.Ballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    if (contest.Propositions == null)
                    {
                        var contestHeight = contest.Selected > 0
                                                ? contest.Selected *
                                                  (V_SPACE + recFont.GetHeight(grFontSiz) + recFont.Height)
                                                : V_SPACE + recFont.GetHeight(grFontSiz) + recFont.Height;
                    }

                    WriteText(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], bold: true, cellsUnderline: false, centerAlign: false);

                    yPos += hdrFont.GetHeight(grFontSiz) + LARGE_V_SPACE * 2;
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    WriteText(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"],
                              ref yPos,
                              leftMargin, titleFont, format);
                    ////export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"], bold: true, cellsUnderline: false, centerAlign: false);
                    yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

                    if (contest.Selected == 0)
                    {
                        WriteText(ref tempGr, ref args,
                                  (string)vars["no_selection"], ref yPos, leftMargin, recFont, format);
                        //export.AddRowData(cells, (string)vars["no_selection"], bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(grFontSiz) + V_SPACE;
                    }
                    else
                    {
                        if (session.Ballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (session.Ballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] +
                                               ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);
                                    //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                    yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        WriteText(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);
                                        //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                        yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                                    }
                                }
                            }

                        }

                    }
                }
                else
                {
                    if (((yPos + titleFont.GetHeight(grFontSiz) +
                          (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5))
                    {
                        //break;                        
                    }
                    addInPrintList = false;
                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        addInPrintList = false;
                        WriteText(ref tempGr, ref args, proposition.Title.Trim(), ref yPos, leftMargin, titleFont, format);
                        //export.AddRowData(cells, proposition.Title.Trim(), bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            WriteText(ref tempGr, ref args, (string)vars["no_selection"], ref yPos, leftMargin, recFont,
                                      format);
                            //export.AddRowData(cells, (string)vars["no_selection"], bold: false, cellsUnderline: false, centerAlign: false);

                            yPos += titleFont.GetHeight(grFontSiz) + V_SPACE;
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;

                if (contCount < session.Ballot.ContestsList.Count && contest.Propositions == null)
                {
                    tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);

                    yPos += V_SPACE;
                }
            }

            string fileName = session.Id + "_" + imgpg + ".png";
            if (linesprinted == 0)
            {
                Logger.Instance.Write("printing into temp image complete");
                try
                {
                    //export.SaveAndClose();
                    //export.Dispose();
                    //pdfCreate(bflname,"\\records");
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string recordsFolderPath = Global.Instance.APP_PATH + "\\records\\";
                    if (!Directory.Exists(recordsFolderPath))
                        Directory.CreateDirectory(recordsFolderPath);

                    bmp.Save(recordsFolderPath + fileName);
                    //  outputImage = recordsFolderPath + bflname;
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\records\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    bmp.Save(reportsFolderPath + fileName);
                }
                catch (Exception e) { Logger.Instance.Write(e); }

                try
                {
                    string backupFlashDriveRecordsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] +
                                                         "\\records\\";
                    if (!Directory.Exists(backupFlashDriveRecordsPath))
                        Directory.CreateDirectory(backupFlashDriveRecordsPath);

                    bmp.Save(backupFlashDriveRecordsPath + fileName);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                Logger.Instance.Write("draw printable image complete");
                //PrintingCompleted = true;
            }

            tempGr.Clear(Color.Transparent);
        }

        private void PrintRecord_img_English()//(object sender, PrintPageEventArgs args)
        {
            PrintPageEventArgs args = argsMain;
            Logger.Instance.Write("PrintRecord");
            addInPrintList = false;
            //mListPrint = new List<textandfont>();
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            cells = 6;
            imgpg = 1;
            folder = "\\records";
            StringFormat format = new StringFormat();

            Logger.Instance.Write("start printing record");

            if (AppManager.Instance.Session != null)
            {
                session = AppManager.Instance.Session;
                Logger.Instance.Write("start printing for session: " + session.Id);
            }


            BallotDefinition ballot = AppManager.Instance.ballot;
            vars.Clear();

            
                vars.Add("ballot_board", ballot.Board);
                vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                vars.Add("ballot_address", ballot.Address);
                vars.Add("ballot_date", ballot.Date);
                vars.Add("machine", "Machine");

                header.Board = (string)vars["ballot_board"];
                header.Address = (string)vars["ballot_address"];
                header.Title = header.Address;
                header.Ballot = (string)vars["ballot_name"];
            header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");// (string)vars["ballot_date"];
            header.Voter = "Voter: " + Convert.ToString(session.Id);

            if (header.Date == "")
                    header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                Logger.Instance.Write("printing " + header.Ballot);
            

            var dataCount = 0;
            var selectedDataCount = 0;

            ContestDefinition condef = null;
            
            foreach (var contest1 in ballot.ContestsList)
            {
                condef = contest1;
                List<Locale> locales;
                if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                    locales = new List<Locale>();
                Locale reportLocale;
                int currentReportLocaleId = -1;
                if (ballot.ReportLocaleId != null)
                    currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);

                 reportLocale = locales[0];

                Locale defaultLocale = locales[0];

                Race currentRaceInstance = null;
                Proposition currentPropositionInstance = null;

                if (ballot.HasSlates)
                {
                    ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale, null, true);
                }
                else
                {
                    ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale);
                }

                ContestDefinition.FillContestData(ref condef, currentPropositionInstance, defaultLocale);

                dataCount += contest1.Data.Count;

                var selectedDataCountLocal = contest1.Data.Where(x => x.State == VrSelection.SelectionState.SELECTED ? true : x.Preference > 0).Count();
                selectedDataCount += selectedDataCountLocal < 1 ? 1 : selectedDataCountLocal;

                if (contest1.Propositions == null)
                {
                    var currentKey = "$b" + ballot.Id + "_g" + contest1.Group + "_name";
                    if (!vars.Contains(currentKey))
                        vars.Add(currentKey, contest1.GroupName);

                    currentKey = "$b" + ballot.Id + "_c" + contest1.Id + "_name";

                    if (!vars.Contains(currentKey))
                        vars.Add(currentKey, contest1.Name);

                    foreach (DataDefinition data in contest1.Data)
                    {
                        currentKey = "$b" + ballot.Id + "_c" + contest1.Id + "_g" + contest1.Group + "_d" + data.Id;

                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, data.Text);
                    }
                }
            }
            
            header.Machine = (string)vars["machine"] +
                             ": " + AppManager.Configuration["System"]["Machine"];

            //Logger.Instance.Write("printing " + doc);


            if (contCount < eballot.ContestsList.Count)
                args.HasMorePages = true;

            args.Graphics.Clear(Color.Transparent);
            // mainbmp.
            Bitmap bmp = mainbmp;// new Bitmap( mainbmp.Width, mainbmp.Height, args.Graphics);

            Graphics tempGr = Graphics.FromImage(bmp);
            addInPrintList = false;

            string bflname = session.Id + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");

            //if (!header.Printed )
            //{
            Logger.Instance.Write("printing header");
            //  Header

            // temp, adding space
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
            WriteText_English(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                      leftMargin, hdrFont, format);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

            WriteText_English(ref tempGr, ref args, header.Board, ref yPos,
                      leftMargin, hdrFont, format);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

            if (eballot.HasCounties)
            {
                WriteText_English(ref tempGr, ref args, eballot.Address, ref yPos,
                          leftMargin, hdrFont, format);
                //export.AddRowData(cells, eballot.Address, bold: true, cellsUnderline: true, centerAlign: false);
                yPos += V_SPACE + titleFont.GetHeight(grFontSiz);
            }

            WriteText_English(ref tempGr, ref args, header.Title, ref yPos,
                      leftMargin, hdrFont, format);

            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

            WriteText_English(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
            yPos += V_SPACE * 10 + titleFont.GetHeight(grFontSiz);

            WriteText_English(ref tempGr, ref args, header.Date, ref yPos,
                      leftMargin, hdrFont, format);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

            WriteText_English(ref tempGr, ref args, header.Voter, ref yPos,
                      leftMargin, hdrFont, format);
            yPos += V_SPACE + titleFont.GetHeight(grFontSiz);

            WriteText_English(ref tempGr, ref args, header.Machine, ref yPos,
                      leftMargin, hdrFont, format);
            yPos += V_SPACE * 10 + titleFont.GetHeight(grFontSiz);
            WriteText_English(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

            //  End of header

            header.Printed = true;
            //}

            yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

            //slates
            if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && !slatesPrinted)
            {
                WriteText_English(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                //export.AddRowData(cells, "SLATES", bold: true, cellsUnderline: true, centerAlign: true);

                Slate slate = eballot.slatesDefinition.Slates.FirstOrDefault(s => s.Id == eballot.slatesDefinition.Data.SlateId);
                if (slate != null)
                {
                    yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;
                    WriteText_English(ref tempGr, ref args, slate.Name, ref yPos, leftMargin, recFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, slate.Name, bold: true, cellsUnderline: true, centerAlign: true);
                }
                yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;
                tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);
                //export.AddRowData(cells, "");

                slatesPrinted = true;
            }
            //slates

            yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

            ContestDefinition contest;
            string text;
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            for (int i = 0; i < eballot.ContestsList.Count; i++)
            {
                Logger.Instance.Write("printing contest " + i+1);
                contest = eballot.ContestsList[i];

                if (eballot.slatesDefinition.Slates.Count > 0 && eballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }

                var appManager = AppManager.Instance;

                if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    if (contest.Propositions == null)
                    {
                        var contestHeight = contest.Selected > 0
                                                ? contest.Selected *
                                                  (V_SPACE + recFont.GetHeight(grFontSiz) + recFont.Height)
                                                : V_SPACE + recFont.GetHeight(grFontSiz) + recFont.Height;
                    }

                    WriteText_English(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_g" + contest.Group + "_name"], bold: true, cellsUnderline: false, centerAlign: false);

                    yPos += hdrFont.GetHeight(grFontSiz) + LARGE_V_SPACE * 2;
                }

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    WriteText_English(ref tempGr, ref args,
                              (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"],
                              ref yPos,
                              leftMargin, titleFont, format);
                    ////export.AddRowData(cells, "");
                    //export.AddRowData(cells, (string)vars["$b" + appManager.ballot.Id + "_c" + contest.Id + "_name"], bold: true, cellsUnderline: false, centerAlign: false);
                    yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

                    if (contest.Selected == 0)
                    {
                        //WriteText_English(ref tempGr, ref args,(string)vars["no_selection"], ref yPos, leftMargin, recFont, format);
                        WriteText_English(ref tempGr, ref args, "No selection made", ref yPos, leftMargin, recFont, format);
                        //export.AddRowData(cells, (string)vars["no_selection"], bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(grFontSiz) + V_SPACE;
                    }
                    else
                    {
                        if (eballot.HasSlates)
                        {
                            var candidates = Slate.GetCandidates(contest.Selected, contest.Id, session.CurrentLocale.Id);

                            foreach (var cand in candidates)
                            {
                                Logger.Instance.Write("printing contest data " + cand.Id);

                                text = cand.Name.ToUpper();

                                WriteText_English(ref tempGr, ref args, text, ref yPos,leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data)
                            {
                                Logger.Instance.Write("printing contest data " + data.Id);

                                if (eballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    if (data.WriteIn)
                                        text = (string)vars["you_wrote"] + ": " + data.Text + " Preference " + data.Preference.ToString();
                                    else
                                        text = data.Text.ToUpper() + " Preference " + data.Preference.ToString();

                                    WriteText_English(ref tempGr, ref args, text, ref yPos,leftMargin, recFont, format);
                                    //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                    yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                                }
                                else
                                {
                                    if (data.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (data.WriteIn)
                                            text = (string)vars["you_wrote"] +
                                                   ": " + data.Text;
                                        else
                                            text = data.Text.ToUpper();
                                        if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                            text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                        WriteText_English(ref tempGr, ref args, text, ref yPos,leftMargin, recFont, format);
                                        //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                        yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                                    }
                                }
                            }

                        }

                    }
                }
                else
                {
                    if (((yPos + titleFont.GetHeight(grFontSiz) +
                          (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5))
                    {
                        //break;                        
                    }
                    addInPrintList = false;
                    showGroupForMassPropositions = false;
                    var idx = 0;
                    var startY = yPos;
                    foreach (var proposition in contest.Propositions)
                    {
                        addInPrintList = false;
                        WriteText_English(ref tempGr, ref args, proposition.Title.Trim(), ref yPos, leftMargin, titleFont, format);
                        //export.AddRowData(cells, proposition.Title.Trim(), bold: true, cellsUnderline: false, centerAlign: false);

                        yPos += titleFont.GetHeight(grFontSiz) + LARGE_V_SPACE;

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            //WriteText_English(ref tempGr, ref args, (string)vars["no_selection"], ref yPos, leftMargin, recFont,format);
                            WriteText_English(ref tempGr, ref args, "No selection made", ref yPos, leftMargin, recFont, format);
                            //export.AddRowData(cells, (string)vars["no_selection"], bold: false, cellsUnderline: false, centerAlign: false);

                            yPos += titleFont.GetHeight(grFontSiz) + V_SPACE;
                        }
                        else
                        {
                            foreach (
                                var data in
                                    contest.Data.Where(
                                        data =>
                                        data.State == VrSelection.SelectionState.SELECTED &&
                                        contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                text = data.Text.ToUpper().Trim();
                                if (appManager.ballot.ElectionType != ElectionTypes.standard)
                                    text += " - " + appManager.TenantVoiceWeight.ToString(AppManager.DefaultCultureInfo);

                                WriteText_English(ref tempGr, ref args, text, ref yPos,leftMargin, recFont, format);
                                //export.AddRowData(cells, text, bold: false, cellsUnderline: false, centerAlign: false);

                                yPos += V_SPACE + recFont.GetHeight(grFontSiz);
                            }
                        }
                        idx++;
                    }
                    var endY = yPos;
                    propositionsHeight = endY - startY;
                }

                groupId = contest.Group;
                contCount++;

                if (contCount < eballot.ContestsList.Count && contest.Propositions == null)
                {
                    tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);

                    yPos += V_SPACE;
                }
            }

            string fileName = session.Id + "_" + imgpg + "_English.png";
            if (linesprinted == 0)
            {
                Logger.Instance.Write("printing into temp image complete");
                try
                {
                    //export.SaveAndClose();
                    //export.Dispose();
                    //pdfCreate(bflname,"\\records");
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string recordsFolderPath = Global.Instance.APP_PATH + "\\records\\";
                    if (!Directory.Exists(recordsFolderPath))
                        Directory.CreateDirectory(recordsFolderPath);

                    bmp.Save(recordsFolderPath + fileName);
                    //  outputImage = recordsFolderPath + bflname;
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\records\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    bmp.Save(reportsFolderPath + fileName);
                }
                catch (Exception e) { Logger.Instance.Write(e); }

                try
                {
                    string backupFlashDriveRecordsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] +
                                                         "\\records\\";
                    if (!Directory.Exists(backupFlashDriveRecordsPath))
                        Directory.CreateDirectory(backupFlashDriveRecordsPath);

                    bmp.Save(backupFlashDriveRecordsPath + fileName);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                Logger.Instance.Write("draw printable image English complete");
                //PrintingCompleted = true;
            }

        }

        private void PrintRecord_Audit(object sender, PrintPageEventArgs args)
        {
            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            int cells = 5;
            StringFormat format = new StringFormat();
            if (contCount < contests.Count)
                args.HasMorePages = true;

            Graphics gOut = args.Graphics;
            Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX),
                                    (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);

            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            Graphics tempGr = Graphics.FromImage(bmp);

            hdrFont = new Font("Calibri", HDR_FONT_SZ, FontStyle.Bold);
            titleFont = new Font("Calibri", TITLE_FONT_SZ, FontStyle.Regular);
            recFont = new Font("Calibri", RECORD_FONT_SZ + 1, FontStyle.Regular);
            voidFont = new Font("Calibri", VOID_FONT_SZ, FontStyle.Regular);

            var dt_counter = DataManager.executeResultDBQuery("Select * from counter_bkup where voter='" + voterid + "' and bid='" + ballotid + "'");

            if (!header.Printed)
            {

                // temp, adding space
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, ballot.Board, ref yPos,
                          leftMargin, hdrFont, format);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                //if (ballot.coun.HasCounties)
                //{
                //    WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                //              leftMargin, hdrFont, format);
                //    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                //}

                WriteText(ref tempGr, ref args, ballot.TopHeading, ref yPos,
                          leftMargin, hdrFont, format);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, ballot.ElectionName, ref yPos,
                          leftMargin, hdrFont, format);

                yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, dt_counter.Rows[0]["vdate"].ToString(), ref yPos,
                          leftMargin, hdrFont, format);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, voterid, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, "Machine:", ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                WriteText(ref tempGr, ref args, "VoterID: " + voterid, ref yPos,
                          leftMargin, hdrFont, format);

                //  End of header

                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                header.Printed = true;
            }

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            //slates
            if (slates.Count > 0 && slates[0].Id > 0 && !slatesPrinted)
            {
                WriteText(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);

                Slate slate = slates.FirstOrDefault(s => s.Id == Convert.ToInt32(dt_counter.Rows[0]["slate_id"]));
                if (slate != null)
                {
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                    WriteText(ref tempGr, ref args, slate.Name, ref yPos, leftMargin, recFont, format);
                    //WriteText(ref tempGr, ref args, slates[0].SlateName, ref yPos, leftMargin, recFont, format);

                }
                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                //tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                //                    args.PageBounds.Left + leftMargin, yPos,
                //                    args.PageBounds.Right - leftMargin, yPos);

                slatesPrinted = true;
            }
            //slates
            else
            {

                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                ContestDefinition contest;
                string text;
                var showGroupForMassPropositions = true;
                float propositionsHeight = 0;

                for (int i = contCount; i < contests.Count; i++)
                {
                    contest = contests[i];

                    Slate slate = slates.FirstOrDefault(s => s.Id == contest.Id);
                    if (slate != null)
                    {
                        if (contest.Type == ContestTypes.R)
                        { continue; }
                    }
                    // var appManager = AppManager.Instance;

                    if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                    {
                        if (contest.Propositions == null)
                        {
                            var contestHeight = contest.Selected > 0
                                                    ? contest.Selected *
                                                      (V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height)
                                                    : V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height;
                        }

                        WriteText(ref tempGr, ref args,
                                  (string)vars["$b" + ballotid + "_g" + contest.Group + "_name"], ref yPos,
                                  leftMargin, hdrFont, format);

                        yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;
                    }

                    if (contest.Propositions.Count == 0)
                    {
                        showGroupForMassPropositions = true;

                        WriteText(ref tempGr, ref args,
                                  (string)contest.Name,
                                  ref yPos,
                                  leftMargin, hdrFont, format);
                        //export.AddRowData(cells, "");
                        yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                        WriteText(ref tempGr, ref args,
                                  (string)contest.GroupName,
                                  ref yPos,
                                  leftMargin, titleFont, format);
                        //export.AddRowData(cells, "");
                        yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;

                        if (contest.Selected > 0)
                        {
                            WriteText(ref tempGr, ref args,
                                      (string)vars["no_selection"], ref yPos, leftMargin, recFont, format);

                            yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                        }
                        else
                        {
                            if (slates.Count > 0)
                            {
                                var candidates = Slate.GetCandidates(contest.Selected, contest.Id, Convert.ToInt32(ballot.ReportLocaleId));

                                foreach (var cand in candidates)
                                {

                                    text = (string)DataManager.GetScalarData_n("Select cand_name from candidate_name where cand_id=" + cand.ToString());// ..ToUpper(); Candidateid 

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                            }
                            else
                            {


                                if (ballot.ElectionType == ElectionTypes.ranking_choice || contest.Type == ContestTypes.V)
                                {
                                    List<int> candlistid = new List<int>();
                                    foreach (DataDefinition data in contest.Data.Distinct())
                                    {
                                        if (candlistid.Contains(data.Id))
                                            continue;
                                        else
                                            candlistid.Add(data.Id);
                                        var dt_pref = DataManager.executeResultDBQuery("Select preference from counter_bkup where voter='" + voterid + "' and bid='" + ballotid + "' and cid='" + contest.Id + "' and did='" + data.Id + "'");
                                        string pref = "0";
                                        if (dt_pref.Rows.Count > 0)
                                        {

                                            pref = dt_pref.Rows[0][0].ToString();
                                            if (data.WriteIn)
                                                text = (string)vars["you_wrote"] +
                                                       ": " + data.Text + " Preference " + pref;
                                            else
                                                text = data.Text.ToUpper() + " Preference " + pref;

                                            WriteText(ref tempGr, ref args, text, ref yPos,
                                                      leftMargin, recFont, format);

                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }
                                    }
                                }
                                else
                                {

                                    var dt_pref = DataManager.executeResultDBQuery("Select did from counter_bkup where voter='" + voterid + "' and bid='" + ballotid + "' and cid='" + contest.Id + "'");
                                    string pref = "0";
                                    if (dt_pref.Rows.Count > 0)
                                    {
                                        pref = dt_pref.Rows[0][0].ToString();
                                        var dtc = DataManager.GetData_audit("Select distinct b.cand_name from candidate_name b where b.cand_id=" + pref);
                                        foreach (DataRow dr in dtc.Rows)
                                        {

                                            WriteText(ref tempGr, ref args, VotriteCrypto.Decrypt(dr["cand_name"].ToString()), ref yPos,
                                                  leftMargin, recFont, format);
                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }
                                    }
                                    //if (data.State == VrSelection.SelectionState.SELECTED)
                                    //{
                                    //if (data.WriteIn)
                                    //    text = (string)vars["you_wrote"] +
                                    //           ": " + data.Text;
                                    //else
                                    //{
                                    //    text = data.Text.ToUpper();
                                    //    //if (ballot.ElectionType != ElectionTypes.standard)
                                    //    //    text += " - " + appManager.TenantVoiceWeight.ToString();

                                    //    WriteText(ref tempGr, ref args, text, ref yPos,
                                    //              leftMargin, recFont, format);

                                    //    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    //}
                                }

                                // }

                            }

                        }
                    }
                    else
                    {
                        if (((yPos + titleFont.GetHeight(args.Graphics) +
                              (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5))
                        {
                            break;
                        }
                        showGroupForMassPropositions = false;
                        var idx = 0;
                        var startY = yPos;
                        foreach (var proposition in contest.Propositions)
                        {
                            WriteText(ref tempGr, ref args, proposition.Title, ref yPos, leftMargin, titleFont, format);

                            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                            if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                            {
                                WriteText(ref tempGr, ref args, (string)vars["no_selection"], ref yPos, leftMargin, recFont,
                                          format);
                                yPos += titleFont.GetHeight(args.Graphics) + V_SPACE;
                            }
                            else
                            {
                                foreach (
                                    var data in
                                        contest.Data.Where(
                                            data =>
                                            data.State == VrSelection.SelectionState.SELECTED &&
                                            contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                                {
                                    text = data.Text.ToUpper();
                                    //if (ballot.ElectionType != ElectionTypes.standard)
                                    //    text += " - " + appManager.TenantVoiceWeight.ToString();

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                            }
                            idx++;
                        }
                        var endY = yPos;
                        propositionsHeight = endY - startY;
                    }

                    groupId = contest.Group;
                    contCount++;

                    if (contCount < contests.Count)
                    {
                        tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);

                        yPos += V_SPACE;
                    }
                }

            }
            // string fileName = session.Id + ".png";
            try
            {

            }
            catch (Exception e)
            {

            }

            try
            {
                string reportsFolderPath = Global.Instance.APP_PATH + "\\records\\";

                if (!Directory.Exists(reportsFolderPath))
                    Directory.CreateDirectory(reportsFolderPath);

                // File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\records", bflname + ".xlsx"), Path.Combine(reportsFolderPath, bflname + ".xlsx"));
            }
            catch (Exception)
            {

            }

            try
            {
                string recordsFolderPath = Global.Instance.APP_PATH + "\\records\\";
                if (!Directory.Exists(recordsFolderPath))
                    Directory.CreateDirectory(recordsFolderPath);

                bmp.Save(recordsFolderPath + bflname);
                outputImage = recordsFolderPath + bflname;
            }
            catch (Exception e)
            {

            }

            /// Jim Kapsis.
            /// Printer Settings

            gOut.DrawImage(bmp, 0f, 0f);


            if (contCount == contests.Count)
            {
                args.HasMorePages = false;

                header.Printed = false;
                slatesPrinted = false;
                contCount = 0;
                bmp.Dispose();
                tempGr.Dispose();
                gOut.Dispose();
            }
        }

        int pdfPrint = 0;
        private void PrintReport(object sender, PrintPageEventArgs args)
        {
            try
            {


                Logger.Instance.Write("printing report");
                addInPrintList = true;

                float leftMargin = LeftMargin;
                float yPos = TopMargin;
                StringFormat format = new StringFormat();
                imgpg = 1;
                folder = "\\reports";
                noVoid = true;
                string cand_separator = ":";

                /// Jim Kapsis 18-06-2016 Excel export 
                cells = 6;
                export = new ExcelExport();

                //export.Create();


                /*if (contCount < AppManager.Instance.ballot.ContestsList.Count)
                    args.HasMorePages = true;
                else
                    return;*/

                Graphics gOut = args.Graphics;
                //Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX),
                //    (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);               // (int)(args.PageBounds.Height / 100 * gOut.DpiY * 2), gOut);
                Bitmap bmp = null;

                if (AppManager.PrinterEnabled)
                {
                    bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);
                }
                else
                {
                    // IGM 2016/1/31, temporary fix to address the png report height issue when printer is not enabled
                    // https://votrite.fogbugz.com/f/cases/5/Png-cut-off-when-there-are-less-than-6-candidates
                    // add extra 300
                    //bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY * 3.0), gOut);
                    bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY * 3.0 + 300), gOut);
                }

                bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
                mainbmp = bmp;
                Graphics tempGr = Graphics.FromImage(bmp);
                export.gr = tempGr;
                //tempGr.FillRegion(new SolidBrush(Color.Green), new Region(new Rectangle(0, 0, (int)(args.Graphics.DpiX), (int)(args.Graphics.DpiY))));
                string temp = "";

                Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);

                if (defaultLocale == null)
                    return;

                BallotDefinition ballot = null;

                if (AppManager.Instance.ballot != null)
                    ballot = AppManager.Instance.ballot;
                else
                    ballot = DefinitionParser.Instance.FillBallotContent(null);


                string bflname = ballot.Name.Replace("/", " ") + "_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
                reportname = bflname;
                export.Create(bflname);

                //if (linesprinted == 0)
                //    goto PDFPrint;

                mListPrint = new List<textandfont>();

                if (ballot.HasSlates)
                {
                    DefinitionParser.Instance.FillSlatesContent(ballot, defaultLocale, null);
                }

                if (!header.Printed)
                {
                    Logger.Instance.Write("Printing header for record");
                    //  Header

                    // temp, adding space to preserve when cutting
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    //mListPrint_Final.Insert(0, "==PAPER AUDIT TRAIL==");

                    WriteText(ref tempGr, ref args, "==PAPER AUDIT TRAIL==", ref yPos, leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    if (header.Board.Length > charsPerLine && printnewline)
                    {
                        WriteText(ref tempGr, ref args, header.Board.Substring(0, charsPerLine) + Environment.NewLine + header.Board.Substring(charsPerLine, header.Board.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);

                    }
                    else
                    {
                        WriteText(ref tempGr, ref args, header.Board, ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    }
                    export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                    export.AddRowData(cells, "ReportID:" + Guid.NewGuid().ToString(), bold: true);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, header.Board, bold: true);



                    if (header.Title.Length > charsPerLine && printnewline)
                    {
                        WriteText(ref tempGr, ref args, header.Title.Substring(0, charsPerLine) + Environment.NewLine + header.Title.Substring(charsPerLine, header.Title.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                    }
                    else
                        WriteText(ref tempGr, ref args, header.Title, ref yPos, leftMargin, hdrFont, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, header.Title, bold: true);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    if (header.Ballot.Length > charsPerLine && printnewline)
                    {
                        WriteText(ref tempGr, ref args, header.Ballot.Substring(0, charsPerLine) + Environment.NewLine + header.Ballot.Substring(charsPerLine, header.Ballot.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                    }
                    else
                        WriteText(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    string[] tmpText = header.Ballot.Split('\n');

                    for (int t = 0; t < tmpText.Length; t++)
                    {
                        export.AddRowData(cells, tmpText[t], bold: true);
                    }

                    ////temp = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                    var query = "select vdate from counter where bid=" + ballot.Id + ";";
                    DataTable tbDate = DataManager.VotingResultsDataInstance.GetDataV2(query);
                    string[] sReportDate = GetDateRange(tbDate);     // new string[]{ "Start: " + DateTime.Parse(tbDate.Rows[0]["StartDate"].ToString()).ToString("dddd, MMMM dd, yyyy"),
                                                                     //  "End: " + DateTime.Parse(tbDate.Rows[0]["EndDate"].ToString()).ToString("dddd, MMMM dd, yyyy")};

                    for (int k = 0; k < sReportDate.Length; k++)
                    {
                        if (k == 0)
                        {
                            yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);
                        }
                        else
                        {
                            yPos += titleFont.GetHeight(args.Graphics);
                        }
                        temp = sReportDate[k];

                        if (temp.Length > charsPerLine && printnewline)
                        {
                            WriteText(ref tempGr, ref args, temp.Substring(0, charsPerLine) + Environment.NewLine + temp.Substring(charsPerLine, temp.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                            yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                        }
                        else
                            WriteText(ref tempGr, ref args, temp, ref yPos, leftMargin, hdrFont, format);
                        /// Jim Kapsis 18-06-2016 Excel export 
                        export.AddRowData(cells, temp, bold: true);
                    }

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    if (header.Machine.Length > charsPerLine && printnewline)
                    {
                        WriteText(ref tempGr, ref args, header.Machine.Substring(0, charsPerLine) + Environment.NewLine + header.Machine.Substring(charsPerLine, header.Machine.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                    }
                    else
                        WriteText(ref tempGr, ref args, header.Machine, ref yPos, leftMargin, hdrFont, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, header.Machine, bold: true);

                    var mconfig = MachineConfig.GetMachineConfig(AppManager.Configuration["System"]["Machine"]);

                    if (mconfig.Count > 0)
                        export.AddRowData(cells, mconfig[0].MainLocation, bold: true);
                    else
                        export.AddRowData(cells, "");

                    export.AddRowData(cells, "");

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    var defaulLineAlignment = format.Alignment;
                    format.Alignment = StringAlignment.Center;

                    temp = "END OF DAY - REPORT";
                    tempGr.DrawString(temp, hdrFont, Brushes.Black, PAPER_WIDTH / 2, yPos, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, temp, bold: true, centerAlign: true, cellsUnderline: true);
                    export.AddRowData(cells, "");

                    //yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    var pen = new Pen(Brushes.Black);
                    pen.DashStyle = DashStyle.Dot;
                    // tempGr.DrawLine(pen, args.PageBounds.Left + leftMargin, yPos, args.PageBounds.Right - leftMargin, yPos);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    format.Alignment = defaulLineAlignment;
                    string castCounterString = AppManager.Instance.GetCastCount().ToString();
                    StringBuilder votesCountString = new StringBuilder("TOTAL CASTS IN BALLOT");

                    while (tempGr.MeasureString(votesCountString + castCounterString, hdrFont).Width <
                           PAPER_WIDTH - leftMargin)
                        votesCountString.Append(".");

                    votesCountString.Append(castCounterString);

                    if (votesCountString.Length > charsPerLine && printnewline)
                    {
                        WriteText(ref tempGr, ref args, votesCountString.ToString().Substring(0, charsPerLine) + Environment.NewLine +
                            votesCountString.ToString().Substring(charsPerLine, votesCountString.ToString().Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                        yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                    }
                    else
                        WriteText(ref tempGr, ref args, votesCountString.ToString(), ref yPos, leftMargin, hdrFont, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    //export.AddRowData(3, "TOTAL VOTES ON MACHINE", bold: true, cellsUnderline: true, centerAlign: true);
                    export.AddRowData(3, "TOTAL CASTS IN BALLOT", bold: true, cellsUnderline: true, centerAlign: true);
                    export.SetValue(2, castCounterString, bold: true, cellsUnderline: true, centerAlign: true);
                    //yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    addlineFont(recFont); tempGr.DrawLine(pen, args.PageBounds.Left + leftMargin, yPos, args.PageBounds.Right - leftMargin, yPos);

                    //string castVotesString = AppManager.Instance.GetTotalVoteCount().ToString();
                    //export.AddRowData(3, "TOTAL VOTES ON MACHINE", bold: true, cellsUnderline: true, centerAlign: true);
                    //export.SetValue(2, castVotesString, bold: true, cellsUnderline: true, centerAlign: true);
                    //export.AddRowData(cells, "");
                    //yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    //yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    //tempGr.DrawLine(pen,
                    //                args.PageBounds.Left + leftMargin, yPos,
                    //                args.PageBounds.Right - leftMargin, yPos);

                    //  End of header

                    header.Printed = true;
                }

                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                //slates-->
                if (ballot.slatesDefinition.Slates.Count > 0 && !slatesPrinted)
                {
                    WriteText(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, "Title: SLATES", bold: true);

                    foreach (var slate in ballot.slatesDefinition.Slates)
                    {
                        var expWriteins = "bid=" + ballot.Id +
                                      " and slate_id=" + slate.Id;
                        int dataCount = reportData.Data.Select(expWriteins).Count();
                        if ((slate.Name + " - " + dataCount).Length > charsPerLine && printnewline)
                        {
                            WriteText(ref tempGr, ref args, (slate.Name + " - " + dataCount).Substring(0, charsPerLine) + Environment.NewLine +
                                (slate.Name + " - " + dataCount).Substring(charsPerLine, (slate.Name + " - " + dataCount).Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                        }
                        else
                            WriteText(ref tempGr, ref args, string.Format("{0} - {1}", slate.Name, dataCount), ref yPos, leftMargin, recFont, format);

                        /// Jim Kapsis 18-06-2016 Excel export 
                        export.AddRowData(4, slate.Name + cand_separator, size: 9D);
                        export.SetValue(1, dataCount.ToString(), centerAlign: true, size: 9D);

                        yPos += recFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                    }

                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, "", cellsUnderline: true);

                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                    addlineFont(recFont);
                    tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                        args.PageBounds.Left + leftMargin, yPos,
                                        args.PageBounds.Right - leftMargin, yPos);

                    slatesPrinted = true;
                }
                //<--slates

                ContestDefinition contest = null;
                Race currentRaceInstance = null;
                Proposition currentPropositionInstance = null;
                string text;
                string contHist = "";
                Logger.Instance.Write("Contest count: " + AppManager.Instance.ballot.ContestsList.Count);
                List<Locale> locales;

                if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                    locales = new List<Locale>();

                Locale reportLocale;
                int currentReportLocaleId = -1;

                if (ballot.ReportLocaleId != null)
                    currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);

                if (currentReportLocaleId > -1)
                    reportLocale = locales[currentReportLocaleId];
                else
                    reportLocale = locales[0];
                var showGroupForMassPropositions = true;
                float propositionsHeight = 0;

                //Jim Code -- Begin
                DataTable tbCorrectOrders = GetCorrectOrderBallot(ballot);
                //Jim Code -- End

                DataTable dtCounts = null;

                if (ballot.ElectionType == ElectionTypes.ranking_choice || ballot.ContestsList[0].Type == ContestTypes.V)
                {
                    //string query = string.Format("select did, preference, count(preference) as cnt from counter where bid = {0} and preference is not null group by did, preference order by preference, cnt desc", ballot.Id);
                    string query = string.Format("select did, preference from counter where bid={0}", ballot.Id);
                    dtCounts = DataManager.VotingResultsDataInstance.GetDataV2(query);


                }

                for (int t = 0; t < tbCorrectOrders.Rows.Count; t++)
                {
                    for (var i = 0; i < ballot.ContestsList.Count; i++)
                    {
                        if (ballot.ContestsList[i].GroupName != tbCorrectOrders.Rows[t]["GroupName"].ToString())
                        {
                            continue;
                        }

                        contest = ballot.ContestsList[i];

                        ContestDefinition.SetContestFields(ref contest, ref currentRaceInstance, ref currentPropositionInstance, reportLocale);
                        ContestDefinition.FillContestData(ref contest, currentPropositionInstance, reportLocale);

                        if (ballot.HasCounties)
                        {
                            string query = string.Format("select count(*) as cnt from counter where bid={0} and cid={1} and county_id={2}", ballot.Id, contest.Id, contest.CountyId);
                            DataTable dt = DataManager.VotingResultsDataInstance.GetDataV2(query);
                            if (dt.Rows.Count > 0)
                            {
                                if (Helper.Cast(dt.Rows[0]["cnt"], 0) < 1)
                                {
                                    contCount++;
                                    continue;
                                }

                                if (!countyPrinted)
                                {
                                    query = string.Format("select county from county where county_id={0}", contest.CountyId);
                                    dt = DataManager.VotingContentDataInstance.GetDataV2(query);
                                    if (dt.Rows.Count > 0)
                                    {
                                        string cnty = dt.Rows[0]["county"].ToString() + " County";
                                        if (cnty.Length > charsPerLine && printnewline)
                                        {
                                            WriteText(ref tempGr, ref args, cnty.Substring(0, charsPerLine) + Environment.NewLine + cnty.Substring(charsPerLine, cnty.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                                            yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                                        }
                                        else
                                            WriteText(ref tempGr, ref args, cnty, ref yPos, leftMargin, hdrFont, format);
                                        yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                                        yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                                        countyPrinted = true;
                                    }
                                }
                            }
                        }

                        if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                        {
                            if (contest.Propositions == null)
                            {
                                //var dataCount = contest.Data.Count(data => !data.ReadOnly && !data.WriteIn);
                                var dataCount = contest.Data.Count(data => !data.ReadOnly);
                                var expWriteins = "bid=" + ballot.Id +
                                          " and cid=" + contest.Id +
                                          " and did is null and cand_name is not null";
                                dataCount += reportData.Data.Select(expWriteins).Count();
                                var contestHeight = dataCount * (V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height);
                            }

                            if (contest.GroupName.Length > charsPerLine && printnewline)
                            {
                                WriteText(ref tempGr, ref args, contest.GroupName.Substring(0, charsPerLine) + Environment.NewLine + contest.GroupName.Substring(charsPerLine, contest.GroupName.Length - charsPerLine), ref yPos, leftMargin, hdrFont, format);
                                yPos += V_SPACE + hdrFont.GetHeight(args.Graphics);
                            }
                            else
                                WriteText(ref tempGr, ref args, contest.GroupName, ref yPos, leftMargin, hdrFont, format);

                            /// Jim Kapsis 18-06-2016 Excel export 
                            export.AddRowData(cells, "Title: " + contest.GroupName, bold: true);

                            yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;
                        }

                        if (contest.Propositions == null)
                        {
                            showGroupForMassPropositions = true;

                            if (contest.Name.Length > charsPerLine && printnewline)
                            {
                                WriteText(ref tempGr, ref args, contest.Name.Substring(0, charsPerLine) + Environment.NewLine + contest.Name.Substring(charsPerLine, contest.Name.Length - charsPerLine), ref yPos, leftMargin, titleFont, format);
                                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                            }
                            else
                                WriteText(ref tempGr, ref args, contest.Name, ref yPos, leftMargin, titleFont, format);

                            /// Jim Kapsis 18-06-2016 Excel export 
                            export.AddRowData(cells, "Name: " + contest.Name, bold: true);

                            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                            if (ballot.HasParty)
                            {
                                if (contest.Type == ContestTypes.P)
                                {
                                    foreach (DataDefinition dd in contest.Data)
                                    {
                                        if (dd.Voteable)
                                        {
                                            var exp = "bid=" + ballot.Id +
                                                      " and cid=" + contest.Id +
                                                      " and did=" + dd.Id;
                                            var count = reportData.Data.Select(exp).Count();

                                            text = dd.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                            if (text.Length > charsPerLine && printnewline)
                                            {
                                                WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                            }
                                            else
                                                WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                            /// Jim Kapsis 18-06-2016 Excel export 
                                            export.AddRowData(4, dd.Text.ToUpper() + cand_separator, size: 9D);
                                            export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }

                                    }
                                }
                                else
                                {
                                    foreach (var party in ballot.partiesDefinition.Parties)
                                    {
                                        foreach (var data in contest.Data.Where(data => !data.ReadOnly && !data.WriteIn && data.PartyId == party.Id))
                                        {
                                            Logger.Instance.Write("printing data: " + data.Text);

                                            var exp = "bid=" + ballot.Id +
                                                      " and cid=" + contest.Id +
                                                      " and did=" + data.Id;
                                            var count = 0f;
                                            var dr = reportData.Data.Select(exp);

                                            count = 0;

                                            if (dr.Length > 0)
                                            {
                                                for (int m = 0; m < dr.Length; m++)
                                                {
                                                    count += Helper.Cast(dr[m]["cnt"], 0f);
                                                }
                                            }

                                            text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                            if (text.Length > charsPerLine && printnewline)
                                            {
                                                WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                            }
                                            else
                                                WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                            /// Jim Kapsis 18-06-2016 Excel export 
                                            export.AddRowData(4, data.Text.ToUpper() + cand_separator, size: 9D);
                                            export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }

                                        //Jim Code -- Begin
                                        var expWriteins = "bid=" + ballot.Id +
                                                      " and pid=" + party.Id +
                                                      " and cid=" + contest.Id +
                                                      " and did is null and cand_name is not null";
                                        //Jim Code -- End

                                        foreach (var dataRow in reportData.Data.Select(expWriteins))
                                        {
                                            float countWriteins = Helper.Cast(dataRow["cnt"], 0f);
                                            text = dataRow["cand_name"] + " - " + countWriteins.ToString(AppManager.DefaultCultureInfo);

                                            if (text.Length > charsPerLine && printnewline)
                                            {
                                                WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                            }
                                            else
                                                WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                            /// Jim Kapsis 18-06-2016 Excel export 
                                            export.AddRowData(4, dataRow["cand_name"].ToString() + cand_separator, size: 9D);
                                            export.SetValue(1, countWriteins.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var count = 0f;
                                DataRow[] dr;

                                if (ballot.ElectionType == ElectionTypes.ranking_choice)// || contest.Type == ContestTypes.V)
                                {
                                    string query = string.Format("select cnt from cast where ballotId={0}", ballot.Id);
                                    int ballots = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(query), 0);
                                    Candidate candidate = null;
                                    int preference = 0;
                                    SortedList<string, int> iniCounters = new SortedList<string, int>();
                                    SortedList<string, int> counters = new SortedList<string, int>();
                                    SortedList<int, string> blt = new SortedList<int, string>();
                                    SortedList<string, int> winners = new SortedList<string, int>();
                                    decimal quota = Math.Ceiling((decimal)ballots / 2);

                                    query = string.Format("select did, preference, cid as sid from counter where bid={0}", ballot.Id);
                                    DataTable dt = DataManager.VotingResultsDataInstance.GetDataV2(query);

                                    if (dt == null || dt.Rows.Count < 1)
                                    {
                                        return;
                                    }

                                    int n = 1;
                                    string sid = "";

                                    foreach (DataRow drow in dt.Rows)
                                    {
                                        Console.WriteLine(drow["sid"]);

                                        if (sid == "")
                                        {
                                            sid = drow["sid"].ToString();
                                        }

                                        if (sid != (string)drow["sid"].ToString())
                                        {
                                            string ballotStr = "";

                                            foreach (KeyValuePair<int, string> pair in blt)
                                            {
                                                ballotStr += pair.Value + ',';
                                            }

                                            ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                            if (counters.ContainsKey(ballotStr))
                                            {
                                                counters[ballotStr]++;
                                            }
                                            else
                                            {
                                                if (!counters.ContainsKey(ballotStr))
                                                {
                                                    counters.Add(ballotStr, 1);
                                                }
                                            }

                                            sid = drow["sid"].ToString();
                                            blt = new SortedList<int, string>();
                                        }

                                        if (n == dt.Rows.Count)
                                        {
                                            foreach (Candidate c in contest.CandidatesList)
                                            {
                                                if (c.Id == (int)drow["did"])
                                                {
                                                    candidate = c;
                                                    preference = Helper.Cast(drow["preference"], 0);
                                                    break;
                                                }
                                            }

                                            if (!blt.ContainsKey(preference))
                                            {
                                                blt.Add(preference, candidate.Name);
                                            }

                                            string ballotStr = "";

                                            foreach (KeyValuePair<int, string> pair in blt)
                                            {
                                                ballotStr += pair.Value + ',';
                                            }

                                            ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                            if (counters.ContainsKey(ballotStr))
                                            {
                                                counters[ballotStr]++;
                                            }
                                            else
                                            {
                                                if (!counters.ContainsKey(ballotStr))
                                                {
                                                    counters.Add(ballotStr, 1);
                                                }
                                            }

                                            break;
                                        }

                                        foreach (Candidate c in contest.CandidatesList)
                                        {
                                            if (c.Id == (int)drow["did"])
                                            {
                                                candidate = c;
                                                preference = Helper.Cast(drow["preference"], 0);
                                                break;
                                            }
                                        }

                                        if (!blt.ContainsKey(preference))
                                        {
                                            blt.Add(preference, candidate.Name);
                                        }

                                        ++n;
                                    }
                                    Console.WriteLine(counters);

                                    iniCounters = counters;

                                    bool winnerFound = false;
                                    int it = 0;

                                    while (!winnerFound)
                                    {
                                        SortedList<string, int> firstPref = new SortedList<string, int>();

                                        foreach (KeyValuePair<string, int> bt in counters)
                                        {
                                            string[] fp = bt.Key.Split(new char[] { ',' });

                                            if (firstPref.ContainsKey(fp[0]))
                                            {
                                                firstPref[fp[0]] += bt.Value;
                                            }
                                            else
                                            {
                                                if (!firstPref.ContainsKey(fp[0]))
                                                {
                                                    firstPref.Add(fp[0], bt.Value);
                                                }
                                            }
                                        }
                                        Console.WriteLine(firstPref);

                                        foreach (KeyValuePair<string, int> c in firstPref)
                                        {
                                            if (c.Value > quota)
                                            {
                                                if (!winners.ContainsKey(c.Key))
                                                {
                                                    winners.Add(c.Key, c.Value);
                                                }
                                            }
                                        }

                                        if (firstPref.Count == 1)
                                        {
                                            if (!winners.ContainsKey(firstPref.Keys[0]))
                                            {
                                                winners.Add(firstPref.Keys[0], firstPref.Values[0]);
                                            }
                                        }

                                        if (winners.Count == 1)
                                        {
                                            winnerFound = true;
                                            Console.WriteLine("winner: " + winners);
                                            break;
                                        }

                                        int min = 0;
                                        string minCand = "";
                                        int j = 0;

                                        foreach (KeyValuePair<string, int> p in firstPref)
                                        {
                                            if (min == 0)
                                            {
                                                min = p.Value;
                                                minCand = p.Key;
                                            }
                                            else
                                            {
                                                if (j == firstPref.Count - 1)
                                                {
                                                    if (p.Value == min)
                                                    {
                                                        minCand = p.Key;
                                                    }
                                                }
                                                else
                                                {
                                                    if (p.Value <= min)
                                                    {
                                                        minCand = p.Key;
                                                    }
                                                }
                                            }

                                            ++j;
                                        }
                                        Console.WriteLine(minCand);

                                        SortedList<string, int> newCounters = new SortedList<string, int>();

                                        foreach (KeyValuePair<string, int> bt in counters)
                                        {
                                            string[] fp = bt.Key.Split(new char[] { ',' });
                                            string ballotStr = "";

                                            foreach (string c in fp)
                                            {
                                                if (c != minCand)
                                                {
                                                    ballotStr += c + ',';
                                                }
                                            }

                                            if (ballotStr != "")
                                            {
                                                ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                                if (newCounters.ContainsKey(ballotStr))
                                                {
                                                    newCounters[ballotStr] += bt.Value;
                                                }
                                                else
                                                {
                                                    if (!newCounters.ContainsKey(ballotStr))
                                                    {
                                                        newCounters.Add(ballotStr, bt.Value);
                                                    }
                                                }
                                            }
                                        }
                                        Console.WriteLine(newCounters);

                                        counters = newCounters;
                                        ++it;
                                    }

                                    Logger.Instance.Write("printing data: " + winners.Keys[0]);
                                    text = "Winner   -   " + winners.Keys[0];

                                    if (text.Length > charsPerLine && printnewline)
                                    {
                                        WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                    else
                                        WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                    Logger.Instance.Write("printing data: " + ballots);
                                    text = "Total votes   -   " + ballots;

                                    if (text.Length > charsPerLine && printnewline)
                                    {
                                        WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                    else
                                        WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                    Logger.Instance.Write("printing data: " + quota);
                                    text = "Quota   -   " + quota;

                                    if (text.Length > charsPerLine && printnewline)
                                    {
                                        WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                    else
                                        WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                    text = "Ballot                                       Votes";

                                    if (text.Length > charsPerLine && printnewline)
                                    {
                                        WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                    else
                                        WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                    foreach (KeyValuePair<string, int> cand in iniCounters)
                                    {
                                        text = cand.Key + " " + cand.Value;

                                        if (text.Length > charsPerLine && printnewline)
                                        {
                                            WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }
                                        else
                                            WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                        // Excel export
                                        export.AddRowData(5, cand.Key + cand_separator, size: 9D);
                                        export.SetValue(1, cand.Value.ToString(), size: 9D, centerAlign: true);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;
                                    }
                                }
                                else
                                {
                                    if (contest.Type == ContestTypes.V)
                                    {
                                        export.AddRowData(2, "", size: 9D);
                                        //export.SetValue(3, "---Preferences---", bold: true, cellsUnderline: true, centerAlign: true, size: 9D);
                                        //export.AddRowData(2, "", size: 9D);
                                        //export.SetValue(1, "1st", size: 9D, centerAlign: true);
                                        //export.SetValue(1, "2nd", size: 9D, centerAlign: true);
                                        //export.SetValue(1, "3rd", size: 9D, centerAlign: true);
                                        //export.SetValue(1, "Sum (Preference)", size: 9D, centerAlign: true);

                                        export.SetValue(3, "--1st Preferences--", bold: true, cellsUnderline: true, centerAlign: true, size: 9D);
                                        foreach (DataRow row in dtCounts.Rows)
                                        {
                                            var dat = contest.Data.Find(a => a.Id == Convert.ToInt32(row[0]));
                                            if (dat == null)
                                            {
                                                var nam = DataManager.VotingContentDataInstance.GetScalarData("select cand_name  from candidate_name where cand_id=" + row[0]);

                                                contest.Data.Add(new DataDefinition { Id = Convert.ToInt32(row[0]), Text = VotriteCrypto.Decrypt(nam.ToString()), WriteIn = false });
                                            }
                                        }

                                    }
                                    foreach (var data in contest.Data.Where(data => !data.ReadOnly && !data.WriteIn))
                                    //foreach (var data in contest.Data.Where(data => !data.ReadOnly ))
                                    {
                                        Logger.Instance.Write("printing data: " + data.Text);
                                        count = 0;
                                        int cnt1 = 0;
                                        int cnt2 = 0;
                                        int cnt3 = 0;
                                        int cntTot = 0;
                                        var exp = "bid=" + ballot.Id +
                                                      " and cid=" + contest.Id +
                                                      " and did=" + data.Id;

                                        var nonexp = "bid=" + ballot.Id +
                                                       " and cid=" + contest.Id;
                                        //" and did=" + data.Id;
                                        if (contest.Type == ContestTypes.V) //updated new excel export of day end report
                                        {

                                            var votecnt = "0";
                                            string query = string.Format("select count( preference) as votecount, count( preference) * 100 /(select count( preference) from counter where " + nonexp + " and preference=1) as cnt from counter where {0} and preference=1", exp);
                                            DataTable dt = DataManager.VotingResultsDataInstance.GetDataV2(query);
                                            if (dt.Rows.Count > 0)
                                            {
                                                cnt1 = Helper.Cast(dt.Rows[0]["cnt"], 0);
                                                votecnt = dt.Rows[0]["votecount"].ToString();
                                            }

                                            text = data.Text.ToUpper() + cand_separator + votecnt + " :     " + cnt1.ToString() + "%";

                                            if (text.Length > charsPerLine && printnewline)
                                            {
                                                WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                            }
                                            else
                                                WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                            /// Jim Kapsis 18-06-2016 Excel export 
                                            export.AddRowData(2, data.Text.ToUpper() + cand_separator, size: 9D);
                                            export.SetValue(1, votecnt.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);
                                            export.SetValue(1, cnt1.ToString(AppManager.DefaultCultureInfo) + "%", size: 9D, centerAlign: true);

                                        }
                                        else
                                        {
                                            dr = reportData.Data.Select(exp);

                                            if (dr.Length > 0)
                                            {
                                                for (int m = 0; m < dr.Length; m++)
                                                {
                                                    count += Helper.Cast(dr[m]["cnt"], 0f);
                                                }
                                            }

                                            text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                            if (text.Length > charsPerLine && printnewline)
                                            {
                                                WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                            }
                                            else
                                                WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                            /// Jim Kapsis 18-06-2016 Excel export 
                                            export.AddRowData(5, data.Text.ToUpper() + cand_separator, size: 9D);
                                            export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);
                                        }


                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                }

                                var expWriteins = "bid=" + ballot.Id +
                                              " and cid=" + contest.Id +
                                              " and did is null and cand_name is not null";
                                foreach (var dataRow in reportData.Data.Select(expWriteins))
                                {
                                    float countWriteins = Helper.Cast(dataRow["cnt"], 0f);
                                    text = dataRow["cand_name"] + " - " + countWriteins.ToString(AppManager.DefaultCultureInfo);

                                    if (text.Length > charsPerLine && printnewline)
                                    {
                                        WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                    else
                                        WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                    /// Jim Kapsis 18-06-2016 Excel export 
                                    export.AddRowData(4, dataRow["cand_name"].ToString() + cand_separator, size: 9D);
                                    export.SetValue(1, countWriteins.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                            }
                        }
                        else
                        {
                            if (((yPos + titleFont.GetHeight(args.Graphics) +
                                      (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5) /*&&
                            (contCount < AppManager.Instance.ballot.ContestsList.Count - 1)*/)
                            {
                                // break; //Smita removed since it is restricting printing in image and excel
                            }

                            showGroupForMassPropositions = false;
                            var idx = 0;
                            var startY = yPos;

                            foreach (var proposition in contest.Propositions)
                            {
                                if (proposition.Title.Length > charsPerLine && printnewline)
                                {
                                    WriteText(ref tempGr, ref args, proposition.Title.Trim().Substring(0, charsPerLine) + Environment.NewLine + proposition.Title.Substring(charsPerLine, proposition.Title.Length - charsPerLine), ref yPos, leftMargin, titleFont, format);
                                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                                }
                                else
                                    WriteText(ref tempGr, ref args, proposition.Title.Trim(), ref yPos, leftMargin, titleFont, format);

                                /// Jim Kapsis 18-06-2016 Excel export 
                                export.AddRowData(cells, "Name: " + proposition.Title.Trim(), bold: true, centerAlign: false);

                                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                                foreach (
                                    var data in
                                        contest.Data.Where(data => !data.ReadOnly).Where(
                                            data => contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                                {
                                    Logger.Instance.Write("printing data: " + data.Text);
                                    string exp = "bid=" + ballot.Id +
                                                 " and cid=" + proposition.ContestId +
                                                 " and did=" + data.Id;
                                    var count = 0f;
                                    DataRow[] dr = reportData.Data.Select(exp);

                                    count = 0;

                                    if (dr.Length > 0)
                                    {
                                        for (int m = 0; m < dr.Length; m++)
                                        {
                                            count += Helper.Cast(dr[m]["cnt"], 0f);
                                        }
                                    }

                                    text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);
                                    if (text != "")
                                    {
                                        if (text.Length > charsPerLine && printnewline)
                                        {
                                            WriteText(ref tempGr, ref args, text.Substring(0, charsPerLine) + Environment.NewLine + text.Substring(charsPerLine, text.Length - charsPerLine), ref yPos, leftMargin, recFont, format);
                                            yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                        }
                                        else
                                            WriteText(ref tempGr, ref args, text, ref yPos, leftMargin, recFont, format);

                                        /// Jim Kapsis 18-06-2016 Excel export 
                                        export.AddRowData(5, " - " + data.Text.ToUpper() + cand_separator, size: 9D);
                                        export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: false);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                }
                                idx++;
                            }
                            var endY = yPos;
                            propositionsHeight = endY - startY;
                        }

                        groupId = contest.Group;
                        contCount++;

                        if (contest.Name != contHist && contest.Propositions == null)
                        {
                            addlineFont(recFont);
                            tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                args.PageBounds.Left + leftMargin, yPos,
                                args.PageBounds.Right - leftMargin, yPos);

                            /// Jim Kapsis 18-06-2016 Excel export 
                            export.AddRowData(cells, "", cellsUnderline: true);
                        }

                        yPos += V_SPACE;

                        if (((yPos + titleFont.GetHeight(args.Graphics) +
                            (float)V_SPACE) >= args.PageBounds.Bottom - V_SPACE) &&
                            (contCount < AppManager.Instance.ballot.ContestsList.Count - 1))
                            break;

                        contHist = contest.Name;
                    }
                }

                // Jim Kapsis 18-06-2016 Excel export 
                try
                {

                    if (pdfPrint == 0)
                    {
                        export.SaveAndClose();
                        export.Dispose();
                        pdfCreate(bflname, "reports");
                        pdfPrint++;
                    }
                    else
                    {
                        export.Dispose();
                        try
                        {
                            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", bflname + ".xlsx");
                            File.Delete(path);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write("Excel file handling error: " + ex);
                            System.Windows.Forms.MessageBox.Show("Excel file handling error: " + ex.Message);

                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.Write("Excel print error: " + ex);
                    System.Windows.Forms.MessageBox.Show("Excel print error: " + ex.Message);

                }
                //PDFPrint:
                if (linesprinted == 0)
                {
                    try
                    {


                        string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\reports\\";

                        if (!Directory.Exists(reportsFolderPath))
                            Directory.CreateDirectory(reportsFolderPath);

                        //File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\reports", bflname + ".xlsx"), Path.Combine(reportsFolderPath, bflname + ".xlsx"));
                        File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\reports", bflname + ".pdf"), Path.Combine(reportsFolderPath, bflname + ".pdf"));

                        //reportsFolderPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] + "\\reports\\";

                        //if (!Directory.Exists(reportsFolderPath))
                        //    Directory.CreateDirectory(reportsFolderPath);

                        //File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\reports", bflname), Path.Combine(reportsFolderPath, bflname));
                    }
                    catch (Exception exx)
                    {
                    }

                    string fileName = bflname + "_" + imgpg + ".png";// DateTime.Now.ToString("MMddyyyyHHmmss.ffff") + ".png";
                    Logger.Instance.Write("printing into temp image complete");

                    try
                    {
                        string reportsFolderPath = Global.Instance.APP_PATH + "\\reports\\";

                        if (!Directory.Exists(reportsFolderPath))
                            Directory.CreateDirectory(reportsFolderPath);

                        bmp.Save(reportsFolderPath + fileName);
                    }
                    catch (Exception e) { Logger.Instance.Write(e); }

                    try
                    {
                        string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\reports\\";

                        if (!Directory.Exists(reportsFolderPath))
                            Directory.CreateDirectory(reportsFolderPath);

                        bmp.Save(reportsFolderPath + fileName);
                    }
                    catch (Exception e) { Logger.Instance.Write(e); }

                    try
                    {
                        string backupFlashDriveReportsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] + "\\reports\\";

                        if (!Directory.Exists(backupFlashDriveReportsPath))
                            Directory.CreateDirectory(backupFlashDriveReportsPath);

                        bmp.Save(backupFlashDriveReportsPath + fileName);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Write(e);
                    }
                }

                fileprintcomplete = true;
                /// if printer enabled
                if (AppManager.PrinterEnabled)
                {
                    //gOut.DrawImage(bmp, 0f, 0f);

                    float fontheight = 0;
                    float n_finalpos = 0;
                    float n_leftMargin = 10;// args.MarginBounds.Left;
                    float topMargin = 10;// args.MarginBounds.Top;
                    String line = null;
                    float n_yPos = topMargin;
                    int totallines = mListPrint.Count;

                    List<textandfont> mListPrint_n = new List<textandfont>();
                    int inx_in = -1;
                    Dictionary<int, List<textandfont>> dictPrint = new Dictionary<int, List<textandfont>>();

                    if (linesprinted > 0)
                        mListPrint = mListPrint_Final;
                    else
                        mListPrint_Final = mListPrint;

                    for (int i = 0; i < mListPrint.Count; i++)
                    {

                        mListPrint_n.Add(mListPrint[i]);
                        line = mListPrint[i].text.Trim();
                        fontheight = mListPrint[i].font.GetHeight(args.Graphics);
                        n_yPos += fontheight;
                        //linesprinted += 1;
                        // if last line position exceeds the visible area
                        if (i + 1 < mListPrint.Count)
                        {
                            if (n_yPos + 10 + mListPrint[i + 1].font.GetHeight(args.Graphics) >= args.Graphics.VisibleClipBounds.Height)// && linesprinted < totallines)
                            {
                                n_yPos = topMargin;
                                inx_in++;
                                dictPrint.Add(inx_in, mListPrint_n);
                                mListPrint_n = new List<textandfont>();
                            }
                        }
                        //else
                        //{
                        //    args.HasMorePages = false;

                        //}
                    }
                    if (mListPrint_n.Count > 0)
                    {
                        n_yPos = topMargin;
                        inx_in++;
                        dictPrint.Add(inx_in, mListPrint_n);
                        mListPrint_n = new List<textandfont>();
                    }

                    //mListPrint = mListPrint_n;

                    //if (linesprinted > 0)
                    //{
                    //    mListPrint = mListPrint.Skip(linesprinted).ToList();
                    //    topMargin = n_yPos = 10;
                    //}
                    if (linesprinted > 0)
                    {
                        for (int d = 0; d < linesprinted; d++)
                        {
                            dictPrint.Remove(dictPrint.Count - 1);
                        }
                    }

                    mListPrint = dictPrint[dictPrint.Count - 1];

                    for (int i = 0; i < mListPrint.Count; i++)
                    {
                        line = mListPrint[i].text.Trim();

                        int ix = line.IndexOf("  ");

                        if (ix > 0)
                        {
                            line = line.Insert(ix, " / ");

                            var words = line.Split(' ');
                            string txt = "";
                            foreach (string word in words)
                            {
                                txt += (word == " " || word == "" ? "" : word + " ");
                            }
                            line = txt;
                        }

                        if (mListPrint[i].text == "--Line--")
                            args.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)), args.PageBounds.Left + leftMargin, n_yPos, args.PageBounds.Right - leftMargin, n_yPos);
                        else
                            args.Graphics.DrawString(line, mListPrint[i].font, System.Drawing.Brushes.Black, n_leftMargin, n_yPos, new System.Drawing.StringFormat());
                        fontheight = mListPrint[i].font.GetHeight(args.Graphics);
                        n_yPos += fontheight;
                        //linesprinted += 1;
                        // if last line position exceeds the visible area
                        //if (i + 1 < mListPrint.Count && dictPrint.Count > linesprinted)
                        //{
                        //    if (n_yPos + 10 + mListPrint[i + 1].font.GetHeight(args.Graphics) >= args.Graphics.VisibleClipBounds.Height)// && linesprinted < totallines)
                        //    {
                        //        linesprinted += 1;
                        //        args.HasMorePages = true;
                        //        header.Printed = false;
                        //        break;
                        //    }
                        //}
                        //else
                        //{
                        //    args.HasMorePages = false;

                        //}
                    }
                    linesprinted += 1;
                    if (inx_in + 1 > linesprinted)
                    {
                        args.HasMorePages = true;
                        header.Printed = false;
                    }
                    else
                    {
                        args.HasMorePages = false;
                        mListPrint_Final = new List<textandfont>();
                        contCount = 0;
                        header.Printed = false;
                        slatesPrinted = false;
                        bmp.Dispose();
                        tempGr.Dispose();
                        gOut.Dispose();
                    }


                }
                else
                {
                    if (contCount == AppManager.Instance.ballot.ContestsList.Count - 1)
                    {
                        args.HasMorePages = false;
                        contCount = 0;
                        header.Printed = false;
                        slatesPrinted = false;
                        bmp.Dispose();
                        tempGr.Dispose();
                        gOut.Dispose();
                    }
                }
                PrintingCompleted = true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(ex);
                System.Windows.Forms.MessageBox.Show(ex.Message);

            }

        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void pdfCreate(string filename, string folder)
        {
            //ExportWorkbookToPdf(filename, folder);
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, filename + ".xlsx");

                GemBox.Spreadsheet.ExcelFile file = GemBox.Spreadsheet.ExcelFile.Load(path);

                GemBox.Spreadsheet.ExcelWorksheet excelWorksheet = file.Worksheets[0];

                excelWorksheet.PrintOptions.FitWorksheetWidthToPages = 1;
                //   excelWorksheet.PrintOptions.FitToPage = false;

                file.Save(path.Replace(".xlsx", ".pdf"), new GemBox.Spreadsheet.PdfSaveOptions()
                {
                    SelectionType = GemBox.Spreadsheet.SelectionType.EntireFile
                });

                string backupFlashDriveReportsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] + "\\"+ folder+"\\";

                if (!Directory.Exists(backupFlashDriveReportsPath))
                    Directory.CreateDirectory(backupFlashDriveReportsPath);

                File.Copy(path.Replace(".xlsx", ".pdf"), Path.Combine(backupFlashDriveReportsPath, filename + ".pdf"));

                File.Delete(path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public static bool ExportWorkbookToPdf(string filename, string folder)
        {
            // If either required string is null or empty, stop and bail out


            string workbookPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, filename + ".xlsx");
            string outputPath = workbookPath.Replace(".xlsx", ".pdf");
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
                // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;

                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close();
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

            // You can use the following method to automatically open the PDF after export if you wish
            // Make sure that the file actually exists first...


            return exportSuccessful;
        }


        private void PrintBallotReport(object sender, PrintPageEventArgs args)
        {
            Logger.Instance.Write("printing report");

            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            StringFormat format = new StringFormat();

            /// Jim Kapsis 18-06-2016 Excel export 
            int cells = 5;
            ExcelExport export = new ExcelExport();
            export.Create();

            /*if (contCount < AppManager.Instance.ballot.ContestsList.Count)
                args.HasMorePages = true;
            else
                return;*/

            Graphics gOut = args.Graphics;
            //Bitmap bmp = new Bitmap((int)(4 * gOut.DpiX),
            //    (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);               // (int)(args.PageBounds.Height / 100 * gOut.DpiY * 2), gOut);
            Bitmap bmp = null;

            if (AppManager.PrinterEnabled)
            {
                bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY), gOut);
            }
            else
            {
                // IGM 2016/1/31, temporary fix to address the png report height issue when printer is not enabled
                // https://votrite.fogbugz.com/f/cases/5/Png-cut-off-when-there-are-less-than-6-candidates
                // add extra 300
                //bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY * 3.0), gOut);
                bmp = new Bitmap((int)(4 * gOut.DpiX), (int)(args.PageBounds.Height / 100 * gOut.DpiY * 3.0 + 300), gOut);
            }

            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));
            Graphics tempGr = Graphics.FromImage(bmp);
            //tempGr.FillRegion(new SolidBrush(Color.Green), new Region(new Rectangle(0, 0, (int)(args.Graphics.DpiX), (int)(args.Graphics.DpiY))));
            string temp = "";

            Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);

            if (defaultLocale == null)
                return;

            BallotDefinition ballot = null;

            if (AppManager.Instance.ballot != null)
                ballot = AppManager.Instance.ballot;
            else
                ballot = DefinitionParser.Instance.FillBallotContent(null);

            if (ballot.HasSlates)
            {
                DefinitionParser.Instance.FillSlatesContent(ballot, defaultLocale, null);
            }

            if (!header.Printed)
            {
                Logger.Instance.Write("Printing header for record");
                //  Header

                // temp, adding space to preserve when cutting
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Board, ref yPos,
                          leftMargin, hdrFont, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, header.Board, bold: true);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Title, ref yPos,
                          leftMargin, hdrFont, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, header.Title, bold: true);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Ballot, ref yPos,
                          leftMargin, hdrFont, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                string[] tmpText = header.Ballot.Split('\n');

                for (int t = 0; t < tmpText.Length; t++)
                {
                    export.AddRowData(cells, tmpText[t], bold: true);
                }

                ////temp = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                var query = "select vdate from counter where bid=" + ballot.Id + ";";
                DataTable tbDate = DataManager.VotingResultsDataInstance.GetDataV2(query);
                string[] sReportDate = GetDateRange(tbDate);     // new string[]{ "Start: " + DateTime.Parse(tbDate.Rows[0]["StartDate"].ToString()).ToString("dddd, MMMM dd, yyyy"),
                                                                 //  "End: " + DateTime.Parse(tbDate.Rows[0]["EndDate"].ToString()).ToString("dddd, MMMM dd, yyyy")};

                for (int k = 0; k < sReportDate.Length; k++)
                {
                    if (k == 0)
                    {
                        yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);
                    }
                    else
                    {
                        yPos += titleFont.GetHeight(args.Graphics);
                    }
                    temp = sReportDate[k];
                    WriteText(ref tempGr, ref args,
                              temp, ref yPos,
                              leftMargin, hdrFont, format);
                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(cells, temp, bold: true);
                }

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                          leftMargin, hdrFont, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, header.Machine, bold: true);

                export.AddRowData(cells, "");

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                var defaulLineAlignment = format.Alignment;
                format.Alignment = StringAlignment.Center;

                temp = "END OF DAY - REPORT";
                tempGr.DrawString(temp, hdrFont, Brushes.Black, PAPER_WIDTH / 2, yPos, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, temp, bold: true, centerAlign: true, cellsUnderline: true);
                export.AddRowData(cells, "");

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                var pen = new Pen(Brushes.Black);
                pen.DashStyle = DashStyle.Dot;
                tempGr.DrawLine(pen,
                                args.PageBounds.Left + leftMargin, yPos,
                                args.PageBounds.Right - leftMargin, yPos);

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                format.Alignment = defaulLineAlignment;
                string castCounterString = AppManager.Instance.GetCastCount().ToString();
                StringBuilder votesCountString = new StringBuilder("TOTAL VOTES ON MACHINE");

                while (tempGr.MeasureString(votesCountString + castCounterString, hdrFont).Width <
                       PAPER_WIDTH - leftMargin)
                    votesCountString.Append(".");

                votesCountString.Append(castCounterString);

                WriteText(ref tempGr, ref args, votesCountString.ToString(), ref yPos,
                          leftMargin, hdrFont, format);
                /// Jim Kapsis 18-06-2016 Excel export 
                //export.AddRowData(3, "TOTAL VOTES ON MACHINE", bold: true, cellsUnderline: true, centerAlign: true);
                export.AddRowData(3, "TOTAL CASTS ON MACHINE", bold: true, cellsUnderline: true, centerAlign: true);
                export.SetValue(2, castCounterString, bold: true, cellsUnderline: true, centerAlign: true);
                export.AddRowData(cells, "");

                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                tempGr.DrawLine(pen,
                                args.PageBounds.Left + leftMargin, yPos,
                                args.PageBounds.Right - leftMargin, yPos);
                //  End of header

                header.Printed = true;
            }

            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

            //slates
            if (ballot.slatesDefinition.Slates.Count > 0 && !slatesPrinted)
            {
                WriteText(ref tempGr, ref args, "SLATES", ref yPos, leftMargin, hdrFont, format);
                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, "SLATES", bold: true);

                foreach (var slate in ballot.slatesDefinition.Slates)
                {
                    var expWriteins = "bid=" + ballot.Id +
                                  " and slate_id=" + slate.Id;
                    int dataCount = reportData.Data.Select(expWriteins).Count();
                    WriteText(ref tempGr, ref args, string.Format("{0} - {1}", slate.Name, dataCount), ref yPos, leftMargin, recFont, format);

                    /// Jim Kapsis 18-06-2016 Excel export 
                    export.AddRowData(4, slate.Name, size: 9D);
                    export.SetValue(1, dataCount.ToString(), centerAlign: true, size: 9D);

                    yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                }

                /// Jim Kapsis 18-06-2016 Excel export 
                export.AddRowData(cells, "", cellsUnderline: true);

                yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;
                tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                                    args.PageBounds.Left + leftMargin, yPos,
                                    args.PageBounds.Right - leftMargin, yPos);

                slatesPrinted = true;
            }
            //slates

            ContestDefinition contest = null;
            Race currentRaceInstance = null;
            Proposition currentPropositionInstance = null;
            string text;
            string contHist = "";
            Logger.Instance.Write("Contest count: " +
                AppManager.Instance.ballot.ContestsList.Count);
            List<Locale> locales;

            if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                locales = new List<Locale>();

            Locale reportLocale;
            int currentReportLocaleId = -1;

            if (ballot.ReportLocaleId != null)
                currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);

            if (currentReportLocaleId > -1)
                reportLocale = locales[currentReportLocaleId];
            else
                reportLocale = locales[0];
            var showGroupForMassPropositions = true;
            float propositionsHeight = 0;

            //Jim Code -- Begin
            DataTable tbCorrectOrders = GetCorrectOrderBallot(ballot);
            //Jim Code -- End

            DataTable dtCounts = null;

            if (ballot.ElectionType == ElectionTypes.ranking_choice)
            {
                //string query = string.Format("select did, preference, count(preference) as cnt from counter where bid = {0} and preference is not null group by did, preference order by preference, cnt desc", ballot.Id);
                string query = string.Format("select did, preference from counter where bid={0}", ballot.Id);
                dtCounts = DataManager.VotingResultsDataInstance.GetDataV2(query);
            }

            for (int t = 0; t < tbCorrectOrders.Rows.Count; t++)
            {
                for (var i = 0; i < ballot.ContestsList.Count; i++)
                {
                    if (ballot.ContestsList[i].GroupName != tbCorrectOrders.Rows[t]["GroupName"].ToString())
                    {
                        continue;
                    }

                    contest = ballot.ContestsList[i];

                    ContestDefinition.SetContestFields(ref contest, ref currentRaceInstance, ref currentPropositionInstance, reportLocale);
                    ContestDefinition.FillContestData(ref contest, currentPropositionInstance, reportLocale);

                    if (ballot.HasCounties)
                    {
                        string query = string.Format("select count(*) as cnt from counter where bid={0} and cid={1} and county_id={2}", ballot.Id, contest.Id, contest.CountyId);
                        DataTable dt = DataManager.VotingResultsDataInstance.GetDataV2(query);
                        if (dt.Rows.Count > 0)
                        {
                            if (Helper.Cast(dt.Rows[0]["cnt"], 0) < 1)
                            {
                                contCount++;
                                continue;
                            }

                            if (!countyPrinted)
                            {
                                query = string.Format("select county from county where county_id={0}", contest.CountyId);
                                dt = DataManager.VotingContentDataInstance.GetDataV2(query);
                                if (dt.Rows.Count > 0)
                                {
                                    WriteText(ref tempGr, ref args, dt.Rows[0]["county"].ToString() + " County", ref yPos,
                                        leftMargin, hdrFont, format);
                                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                                    countyPrinted = true;
                                }
                            }
                        }
                    }

                    if (!(groupId == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                    {
                        if (contest.Propositions == null)
                        {
                            var dataCount = contest.Data.Count(data => !data.ReadOnly && !data.WriteIn);
                            var expWriteins = "bid=" + ballot.Id +
                                      " and cid=" + contest.Id +
                                      " and did is null and cand_name is not null";
                            dataCount += reportData.Data.Select(expWriteins).Count();
                            var contestHeight = dataCount * (V_SPACE + recFont.GetHeight(args.Graphics) + recFont.Height);
                        }

                        WriteText(ref tempGr, ref args, contest.GroupName, ref yPos,
                            leftMargin, hdrFont, format);

                        /// Jim Kapsis 18-06-2016 Excel export 
                        export.AddRowData(cells, contest.GroupName, bold: true);

                        yPos += hdrFont.GetHeight(args.Graphics) + LARGE_V_SPACE * 2;
                    }

                    if (contest.Propositions == null)
                    {
                        showGroupForMassPropositions = true;
                        WriteText(ref tempGr, ref args, contest.Name, ref yPos,
                                  leftMargin, titleFont, format);

                        /// Jim Kapsis 18-06-2016 Excel export 
                        export.AddRowData(cells, contest.Name, bold: true);

                        yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                        if (ballot.HasParty)
                        {
                            if (contest.Type == ContestTypes.P)
                            {
                                foreach (DataDefinition dd in contest.Data)
                                {
                                    if (dd.Voteable)
                                    {
                                        var exp = "bid=" + ballot.Id +
                                                  " and cid=" + contest.Id +
                                                  " and did=" + dd.Id;
                                        var count = reportData.Data.Select(exp).Count();

                                        text = dd.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                        WriteText(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);

                                        /// Jim Kapsis 18-06-2016 Excel export 
                                        export.AddRowData(4, dd.Text.ToUpper(), size: 9D);
                                        export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }

                                }
                            }
                            else
                            {
                                foreach (var party in ballot.partiesDefinition.Parties)
                                {
                                    foreach (var data in contest.Data.Where(data => !data.ReadOnly && !data.WriteIn && data.PartyId == party.Id))
                                    {
                                        Logger.Instance.Write("printing data: " + data.Text);

                                        var exp = "bid=" + ballot.Id +
                                                  " and cid=" + contest.Id +
                                                  " and did=" + data.Id;
                                        var count = 0f;
                                        var dr = reportData.Data.Select(exp);

                                        count = 0;

                                        if (dr.Length > 0)
                                        {
                                            for (int m = 0; m < dr.Length; m++)
                                            {
                                                count += Helper.Cast(dr[m]["cnt"], 0f);
                                            }
                                        }

                                        text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                        WriteText(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);

                                        /// Jim Kapsis 18-06-2016 Excel export 
                                        export.AddRowData(4, data.Text.ToUpper(), size: 9D);
                                        export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }

                                    //Jim Code -- Begin
                                    var expWriteins = "bid=" + ballot.Id +
                                                  " and pid=" + party.Id +
                                                  " and cid=" + contest.Id +
                                                  " and did is null and cand_name is not null";
                                    //Jim Code -- End

                                    foreach (var dataRow in reportData.Data.Select(expWriteins))
                                    {
                                        float countWriteins = Helper.Cast(dataRow["cnt"], 0f);
                                        text = dataRow["cand_name"] + " - " + countWriteins.ToString(AppManager.DefaultCultureInfo);

                                        WriteText(ref tempGr, ref args, text, ref yPos,
                                                  leftMargin, recFont, format);

                                        /// Jim Kapsis 18-06-2016 Excel export 
                                        export.AddRowData(4, dataRow["cand_name"].ToString(), size: 9D);
                                        export.SetValue(1, countWriteins.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                        yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var count = 0f;
                            DataRow[] dr;

                            if (ballot.ElectionType == ElectionTypes.ranking_choice)
                            {
                                string query = string.Format("select cnt from cast where ballotId={0}", ballot.Id);
                                int ballots = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(query), 0);
                                Candidate candidate = null;
                                int preference = 0;
                                SortedList<string, int> iniCounters = new SortedList<string, int>();
                                SortedList<string, int> counters = new SortedList<string, int>();
                                SortedList<int, string> blt = new SortedList<int, string>();
                                SortedList<string, int> winners = new SortedList<string, int>();
                                decimal quota = Math.Ceiling((decimal)ballots / 2);

                                query = string.Format("select did, preference, sid from counter where bid={0}", ballot.Id);
                                DataTable dt = DataManager.VotingResultsDataInstance.GetDataV2(query);

                                if (dt == null || dt.Rows.Count < 1)
                                {
                                    return;
                                }

                                int n = 1;
                                string sid = "";

                                foreach (DataRow drow in dt.Rows)
                                {
                                    Console.WriteLine(drow["sid"]);

                                    if (sid == "")
                                    {
                                        sid = (string)drow["sid"];
                                    }

                                    if (sid != (string)drow["sid"])
                                    {
                                        string ballotStr = "";

                                        foreach (KeyValuePair<int, string> pair in blt)
                                        {
                                            ballotStr += pair.Value + ',';
                                        }

                                        ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                        if (counters.ContainsKey(ballotStr))
                                        {
                                            counters[ballotStr]++;
                                        }
                                        else
                                        {
                                            if (!counters.ContainsKey(ballotStr))
                                            {
                                                counters.Add(ballotStr, 1);
                                            }
                                        }

                                        sid = (string)drow["sid"];
                                        blt = new SortedList<int, string>();
                                    }

                                    if (n == dt.Rows.Count)
                                    {
                                        foreach (Candidate c in contest.CandidatesList)
                                        {
                                            if (c.Id == (int)drow["did"])
                                            {
                                                candidate = c;
                                                preference = Helper.Cast(drow["preference"], 0);
                                                break;
                                            }
                                        }

                                        if (!blt.ContainsKey(preference))
                                        {
                                            blt.Add(preference, candidate.Name);
                                        }

                                        string ballotStr = "";

                                        foreach (KeyValuePair<int, string> pair in blt)
                                        {
                                            ballotStr += pair.Value + ',';
                                        }

                                        ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                        if (counters.ContainsKey(ballotStr))
                                        {
                                            counters[ballotStr]++;
                                        }
                                        else
                                        {
                                            if (!counters.ContainsKey(ballotStr))
                                            {
                                                counters.Add(ballotStr, 1);
                                            }
                                        }

                                        break;
                                    }

                                    foreach (Candidate c in contest.CandidatesList)
                                    {
                                        if (c.Id == (int)drow["did"])
                                        {
                                            candidate = c;
                                            preference = Helper.Cast(drow["preference"], 0);
                                            break;
                                        }
                                    }

                                    if (!blt.ContainsKey(preference))
                                    {
                                        blt.Add(preference, candidate.Name);
                                    }

                                    ++n;
                                }
                                Console.WriteLine(counters);

                                iniCounters = counters;

                                bool winnerFound = false;
                                int it = 0;

                                while (!winnerFound)
                                {
                                    SortedList<string, int> firstPref = new SortedList<string, int>();

                                    foreach (KeyValuePair<string, int> bt in counters)
                                    {
                                        string[] fp = bt.Key.Split(new char[] { ',' });

                                        if (firstPref.ContainsKey(fp[0]))
                                        {
                                            firstPref[fp[0]] += bt.Value;
                                        }
                                        else
                                        {
                                            if (!firstPref.ContainsKey(fp[0]))
                                            {
                                                firstPref.Add(fp[0], bt.Value);
                                            }
                                        }
                                    }
                                    Console.WriteLine(firstPref);

                                    foreach (KeyValuePair<string, int> c in firstPref)
                                    {
                                        if (c.Value > quota)
                                        {
                                            if (!winners.ContainsKey(c.Key))
                                            {
                                                winners.Add(c.Key, c.Value);
                                            }
                                        }
                                    }

                                    if (firstPref.Count == 1)
                                    {
                                        if (!winners.ContainsKey(firstPref.Keys[0]))
                                        {
                                            winners.Add(firstPref.Keys[0], firstPref.Values[0]);
                                        }
                                    }

                                    if (winners.Count == 1)
                                    {
                                        winnerFound = true;
                                        Console.WriteLine("winner: " + winners);
                                        break;
                                    }

                                    int min = 0;
                                    string minCand = "";
                                    int j = 0;

                                    foreach (KeyValuePair<string, int> p in firstPref)
                                    {
                                        if (min == 0)
                                        {
                                            min = p.Value;
                                            minCand = p.Key;
                                        }
                                        else
                                        {
                                            if (j == firstPref.Count - 1)
                                            {
                                                if (p.Value == min)
                                                {
                                                    minCand = p.Key;
                                                }
                                            }
                                            else
                                            {
                                                if (p.Value <= min)
                                                {
                                                    minCand = p.Key;
                                                }
                                            }
                                        }

                                        ++j;
                                    }
                                    Console.WriteLine(minCand);

                                    SortedList<string, int> newCounters = new SortedList<string, int>();

                                    foreach (KeyValuePair<string, int> bt in counters)
                                    {
                                        string[] fp = bt.Key.Split(new char[] { ',' });
                                        string ballotStr = "";

                                        foreach (string c in fp)
                                        {
                                            if (c != minCand)
                                            {
                                                ballotStr += c + ',';
                                            }
                                        }

                                        if (ballotStr != "")
                                        {
                                            ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                                            if (newCounters.ContainsKey(ballotStr))
                                            {
                                                newCounters[ballotStr] += bt.Value;
                                            }
                                            else
                                            {
                                                if (!newCounters.ContainsKey(ballotStr))
                                                {
                                                    newCounters.Add(ballotStr, bt.Value);
                                                }
                                            }
                                        }
                                    }
                                    Console.WriteLine(newCounters);

                                    counters = newCounters;
                                    ++it;
                                }

                                Logger.Instance.Write("printing data: " + winners.Keys[0]);
                                text = "Winner   -   " + winners.Keys[0];

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                      leftMargin, recFont, format);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                Logger.Instance.Write("printing data: " + ballots);
                                text = "Total votes   -   " + ballots;

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                      leftMargin, recFont, format);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                Logger.Instance.Write("printing data: " + quota);
                                text = "Quota   -   " + quota;

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                      leftMargin, recFont, format);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                text = "Ballot                                       Votes";

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                      leftMargin, recFont, format);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;

                                foreach (KeyValuePair<string, int> cand in iniCounters)
                                {
                                    text = cand.Key + "                                       " + cand.Value;

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);

                                    // Excel export
                                    export.AddRowData(4, cand.Key, size: 9D);
                                    export.SetValue(1, cand.Value.ToString(), size: 9D, centerAlign: true);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics) + 10;
                                }
                            }
                            else
                            {
                                foreach (var data in contest.Data.Where(data => !data.ReadOnly && !data.WriteIn))
                                {
                                    Logger.Instance.Write("printing data: " + data.Text);
                                    var exp = "bid=" + ballot.Id +
                                              " and cid=" + contest.Id +
                                              " and did=" + data.Id;

                                    dr = reportData.Data.Select(exp);

                                    count = 0;

                                    if (dr.Length > 0)
                                    {
                                        for (int m = 0; m < dr.Length; m++)
                                        {
                                            count += Helper.Cast(dr[m]["cnt"], 0f);
                                        }
                                    }

                                    text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                    WriteText(ref tempGr, ref args, text, ref yPos,
                                              leftMargin, recFont, format);

                                    /// Jim Kapsis 18-06-2016 Excel export 
                                    export.AddRowData(4, data.Text.ToUpper(), size: 9D);
                                    export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                    yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                                }
                            }

                            var expWriteins = "bid=" + ballot.Id +
                                          " and cid=" + contest.Id +
                                          " and did is null and cand_name is not null";
                            foreach (var dataRow in reportData.Data.Select(expWriteins))
                            {
                                float countWriteins = Helper.Cast(dataRow["cnt"], 0f);
                                text = dataRow["cand_name"] + " - " + countWriteins.ToString(AppManager.DefaultCultureInfo);

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);

                                /// Jim Kapsis 18-06-2016 Excel export 
                                export.AddRowData(4, dataRow["cand_name"].ToString(), size: 9D);
                                export.SetValue(1, countWriteins.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                        }
                    }
                    else
                    {
                        if (((yPos + titleFont.GetHeight(args.Graphics) +
                                  (float)V_SPACE) >= args.PageBounds.Bottom - propositionsHeight * 1.5) /*&&
                            (contCount < AppManager.Instance.ballot.ContestsList.Count - 1)*/)
                        {
                            break;
                        }

                        showGroupForMassPropositions = false;
                        var idx = 0;
                        var startY = yPos;

                        foreach (var proposition in contest.Propositions)
                        {
                            WriteText(ref tempGr, ref args, proposition.Title, ref yPos,
                                      leftMargin, titleFont, format);

                            /// Jim Kapsis 18-06-2016 Excel export 
                            export.AddRowData(cells, proposition.Title, bold: true, centerAlign: true);

                            yPos += titleFont.GetHeight(args.Graphics) + LARGE_V_SPACE;

                            foreach (
                                var data in
                                    contest.Data.Where(data => !data.ReadOnly).Where(
                                        data => contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                Logger.Instance.Write("printing data: " + data.Text);
                                string exp = "bid=" + ballot.Id +
                                             " and cid=" + proposition.ContestId +
                                             " and did=" + data.Id;
                                var count = 0f;
                                DataRow[] dr = reportData.Data.Select(exp);

                                count = 0;

                                if (dr.Length > 0)
                                {
                                    for (int m = 0; m < dr.Length; m++)
                                    {
                                        count += Helper.Cast(dr[m]["cnt"], 0f);
                                    }
                                }

                                text = data.Text.ToUpper() + " - " + count.ToString(AppManager.DefaultCultureInfo);

                                WriteText(ref tempGr, ref args, text, ref yPos,
                                          leftMargin, recFont, format);

                                /// Jim Kapsis 18-06-2016 Excel export 
                                export.AddRowData(4, data.Text.ToUpper(), size: 9D);
                                export.SetValue(1, count.ToString(AppManager.DefaultCultureInfo), size: 9D, centerAlign: true);

                                yPos += V_SPACE + recFont.GetHeight(args.Graphics);
                            }
                            idx++;
                        }
                        var endY = yPos;
                        propositionsHeight = endY - startY;
                    }

                    groupId = contest.Group;
                    contCount++;

                    if (contest.Name != contHist && contest.Propositions == null)
                    {
                        tempGr.DrawLine(new Pen(new SolidBrush(Color.Black)),
                            args.PageBounds.Left + leftMargin, yPos,
                            args.PageBounds.Right - leftMargin, yPos);

                        /// Jim Kapsis 18-06-2016 Excel export 
                        export.AddRowData(cells, "", cellsUnderline: true);
                    }

                    yPos += V_SPACE;

                    if (((yPos + titleFont.GetHeight(args.Graphics) +
                        (float)V_SPACE) >= args.PageBounds.Bottom - V_SPACE) &&
                        (contCount < AppManager.Instance.ballot.ContestsList.Count - 1))
                        break;

                    contHist = contest.Name;
                }
            }

            // Jim Kapsis 18-06-2016 Excel export 
            export.SaveAndClose();
            export.Dispose();

            string fileName = DateTime.Now.ToString("MMddyyyyHHmmss.ffff") + ".png";
            Logger.Instance.Write("printing into temp image complete");

            try
            {
                string reportsFolderPath = Global.Instance.APP_PATH + "\\reports\\";

                if (!Directory.Exists(reportsFolderPath))
                    Directory.CreateDirectory(reportsFolderPath);

                bmp.Save(reportsFolderPath + fileName);
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            try
            {
                string backupFlashDriveReportsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] + "\\reports\\";

                if (!Directory.Exists(backupFlashDriveReportsPath))
                    Directory.CreateDirectory(backupFlashDriveReportsPath);

                bmp.Save(backupFlashDriveReportsPath + fileName);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            /// Jim Kapsis. 
            /// if printer enabled
            if (AppManager.PrinterEnabled)
            {
                gOut.DrawImage(bmp, 0f, 0f);
            }

            if (contCount == AppManager.Instance.ballot.ContestsList.Count - 1)
            {
                args.HasMorePages = false;
                contCount = 0;
                header.Printed = false;
                slatesPrinted = false;
                bmp.Dispose();
                tempGr.Dispose();
                gOut.Dispose();
            }
        }

        public void PrintConsolidated()
        {
            //return;
            Logger.Instance.Write("printing consolidated report");

            if (AppManager.Instance.Session != null)
            {
                session = AppManager.Instance.Session;
                Logger.Instance.Write("start printing for session: " + session.Id);
            }


            BallotDefinition ballot = null;

            //if (AppManager.Instance.ballot != null)
            //    ballot = AppManager.Instance.ballot;
            //else
            ballot = DefinitionParser.Instance.FillBallotContent(null);

            if (session != null)
            {
                vars.Add("void", session.CurrentLocale.Void);
                vars.Add("voter", session.CurrentLocale.Voter);
                vars.Add("machine", session.CurrentLocale.Machine);
                vars.Add("no_selection", session.CurrentLocale.NoSelection);
                vars.Add("you_wrote", string.Empty);

                header.Board = session.Ballot.Board;
                header.Address = session.Ballot.Address;
                header.Voter = (string)vars["voter"] +
                               ": " + Convert.ToString(session.Id);
                header.Ballot = session.Ballot.Name + "\n" + session.Ballot.Title;
                header.Date = session.Ballot.Date;

                Logger.Instance.Write("printing " + header.Ballot);
            }
            else
            {
                vars.Add("ballot_board", ballot.Board);
                vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                vars.Add("ballot_address", ballot.Address);
                vars.Add("ballot_date", ballot.Date);
                vars.Add("machine", "Machine");

                header.Board = (string)vars["ballot_board"];
                header.Address = (string)vars["ballot_address"];
                header.Title = header.Address;
                header.Ballot = (string)vars["ballot_name"];
                header.Date = (string)vars["ballot_date"];

                if (header.Date == "")
                    header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                Logger.Instance.Write("printing " + header.Ballot);
            }

            float leftMargin = LeftMargin;
            float yPos = TopMargin;
            StringFormat format = new StringFormat();

            int cells = 5;
            ExcelExport export = new ExcelExport();
            //export.Create();          

            var list = VotRiteBallotDataManager.AppCode.Ballot.GetBallots();
            bool commonLoc = true;
            string finalLocation = "";

            foreach (var itm in list)
            {
                if (itm.Location != "")
                {
                    if (finalLocation != "" && itm.Location != finalLocation)
                    {
                        commonLoc = false;
                        break;
                    }
                    else
                        finalLocation = itm.Location;
                }
            }

            string bflname = "Consolidated_" + DateTime.Now.ToString("MM-dd-yyyy") + "_" + DateTime.Now.ToString("HHmmssfff");
            export.Create(bflname);

            var cnAllt = DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast");

            export.AddRowData(cells, ballot.Board, bold: true, cellsUnderline: true, centerAlign: true);
            export.AddRowData(cells, "Report ID: "+Guid.NewGuid().ToString(), bold: false, cellsUnderline: false, centerAlign: true);
            //export.AddRowData(cells, "");
            export.AddRowData(cells, "");
           // export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);
            export.AddRowData(cells, "Machine: " + AppManager.Configuration["System"]["Machine"], bold: true, cellsUnderline: true, centerAlign: false);
            if (commonLoc)
                export.AddRowData(cells, "Location: " + finalLocation, bold: true, cellsUnderline: true, centerAlign: false);
            else
                export.AddRowData(cells, "");

            var mconfig = MachineConfig.GetMachineConfig(AppManager.Configuration["System"]["Machine"]);

            if (mconfig.Count > 0)
                export.AddRowData(cells, "Main Location: "+mconfig[0].MainLocation, bold: true, cellsUnderline: true, centerAlign: false);
            else
                export.AddRowData(cells, "");

            export.AddRowData(cells, "");

            export.AddRowData(3, "CONSOLIDATED VOTES:", bold: true, cellsUnderline: true, centerAlign: false);
            export.SetValue(2, cnAllt.ToString(), bold: true, cellsUnderline: true, centerAlign: false);
            export.AddRowData(cells, "");
            export.AddRowData(cells, "");

            foreach (var item in list)
            {
                var cnt = DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast where ballotId=" + item.Id);
                //item.ElectionName = item.ElectionName + " (Total Cast: " + (cnt.ToString() == "" ? "0" : cnt.ToString()) + ")";
                export.AddRowData(3, item.ElectionName+":", bold: true, cellsUnderline: false, centerAlign: false);
                export.SetValue(2, cnt.ToString(), bold: true, cellsUnderline: false, centerAlign: false);
                export.AddRowData(cells, "");
            }

            export.SaveAndClose();
            export.Dispose();
            try
            {
                string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "\\reports\\";

                if (!Directory.Exists(reportsFolderPath))
                    Directory.CreateDirectory(reportsFolderPath);

                pdfCreate(bflname, "reports");
                File.Copy(Path.Combine(Global.Instance.APP_PATH + "\\reports", bflname + ".pdf"), Path.Combine(reportsFolderPath, bflname + ".pdf"));
            }
            catch (Exception)
            {

            }
        }

        private DataTable GetCorrectOrderBallot(BallotDefinition pBallot)
        {
            DataTable tbRet = new DataTable();
            tbRet.Columns.Add(new DataColumn("GroupID", typeof(System.Int32)));
            tbRet.Columns.Add(new DataColumn("GroupName", typeof(System.String)));

            DataTable tbPosition = Contest.GetOrderPositionRec();

            for (int i = 0; i < pBallot.ContestsList.Count; i++)
            {
                string sTemp = pBallot.ContestsList[i].GroupName;
                DataRow[] rowsTmp1 = tbRet.Select("GroupName='" + sTemp + "'");
                if (rowsTmp1.Length == 0)
                {
                    DataRow rowNew = tbRet.NewRow();
                    int iGroupID = -1;
                    DataRow[] rowsTmp2 = tbPosition.Select("contest_id=" + pBallot.ContestsList[i].Id.ToString());
                    if (rowsTmp2.Length > 0)
                    {
                        iGroupID = int.Parse(rowsTmp2[0]["rvo_order_position"].ToString());
                    }

                    rowNew["GroupID"] = iGroupID;
                    rowNew["GroupName"] = sTemp;
                    tbRet.Rows.Add(rowNew);
                }
            }
            tbRet.DefaultView.Sort = "GroupID";

            return tbRet.DefaultView.ToTable();
        }

        private List<ContestDefinition> GetCorrectOrderBallotList(BallotDefinition pBallot)
        {
            // wrapper around DataTable GetCorrectOrderBallot(BallotDefinition pBallot)
            // returns a List<ContestDefinition>

            // we are rebuilding contests list in a correct order, 
            // based on a tbCorrectOrders returned from GetCorrectOrderBallot
            List<ContestDefinition> contests = new List<ContestDefinition>();
            DataTable tbCorrectOrders = GetCorrectOrderBallot(pBallot);

            foreach (DataRow row in tbCorrectOrders.Rows)
            {
                foreach (var contest in pBallot.ContestsList)
                {
                    if (contest.GroupName == row["GroupName"].ToString())
                    {
                        contests.Add(contest);
                        break;
                    }
                }
            }

            return contests;
        }


        private void PrintVoid(object sender, PrintPageEventArgs args)
        {
            
            try
            {
                if (session.Id == null)
                    return;
                if (AppManager.Instance.PrintFinalReportStarted)
                    return;
            }
            catch (Exception)
            {
            }
            Pen pen = new Pen(Color.White, 6.00f);
            Graphics gOut = args.Graphics;
            Bitmap bmp = new Bitmap((int)(4 * args.Graphics.DpiX),
                (int)(args.PageBounds.Height / 100 * args.Graphics.DpiY),
                args.Graphics);

            bmp.SetResolution((args.Graphics.DpiX), (args.Graphics.DpiY));

            Graphics tempGr = Graphics.FromImage(bmp);
            string line = (string)vars["void"];
            SizeF lineSz = tempGr.MeasureString(line, voidFont);
            float yPos = 50f;
            float topMargin = 50f;
            float leftMargin = LeftMargin;
            int cells = 5;
            StringFormat format = new StringFormat();

            
            WriteText(ref tempGr, ref args, header.Board, ref yPos,
                          leftMargin, hdrFont, format);
            
            yPos += V_SPACE + titleFont.GetHeight(args.Graphics);


            if (session.Ballot.HasCounties)
            {
                WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                          leftMargin, hdrFont, format);
                yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
            }

            WriteText(ref tempGr, ref args, header.Title, ref yPos,
                      leftMargin, hdrFont, format);
            
            yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

            WriteText(ref tempGr, ref args, header.Ballot, ref yPos,
                      leftMargin, hdrFont, format);
            
            yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

            WriteText(ref tempGr, ref args, header.Date, ref yPos,
                      leftMargin, hdrFont, format);
            
            yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

            WriteText(ref tempGr, ref args, header.Voter, ref yPos,
                      leftMargin, hdrFont, format);
           
            yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

            WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                      leftMargin, hdrFont, format);

            yPos += V_SPACE + voidFont.GetHeight(args.Graphics);

            WriteText(ref tempGr, ref args, "======VOIDED BY VOTER======", ref yPos,
                          leftMargin, hdrFont, format);

            yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

            tempGr.FillRectangle(new SolidBrush(Color.DarkGray), 0, yPos- (lineSz.Height)/2,
                lineSz.Width, lineSz.Height);
            tempGr.DrawRectangle(pen, 0f, yPos- (lineSz.Height) / 2, lineSz.Width, lineSz.Height);

            // IGM 2015/3/1, Rotate VOID 180
            tempGr.DrawString(line, voidFont, Brushes.Black,
                                0f, yPos - (lineSz.Height) / 2, new StringFormat());

            //Jim Code -- Begin
            GraphicsPath capPath = new GraphicsPath();

            capPath.AddLine(-5, 0, 0, 5);
            capPath.AddLine(0, 5, 5, 0);
            Pen penLine = new Pen(Color.Black, 6.00f);
            penLine.CustomEndCap = new System.Drawing.Drawing2D.CustomLineCap(null, capPath);

            tempGr.DrawLine(penLine, lineSz.Width / 2, yPos+ (lineSz.Height) / 2, lineSz.Width / 2, yPos + (lineSz.Height) / 2 + 180);

            if (AppManager.PrinterEnabled)
            {
                gOut.DrawImage(bmp, 0f, 0f);
            }
            try
            {
                string fileName = ""+session.Id + "_VOID.png";
                string recordsFolderPath = Global.Instance.APP_PATH + "\\records\\";
                if (!Directory.Exists(recordsFolderPath))
                    Directory.CreateDirectory(recordsFolderPath);

                bmp.Save(recordsFolderPath + fileName);
                fileName = "" + session.Id + "_VOID_English.png";
                bmp.Save(recordsFolderPath + fileName);
            }
            catch (Exception)
            {

            }
            args.HasMorePages = false;
            bmp.Dispose();
            tempGr.Dispose();
            gOut.Dispose();
        }

        public void PrintSmit(PrintDoc doc, bool cut)
        {
            try
            {
                //if (doc == PrintDoc.REPORT && AppManager.Instance.FinalReportPrintCounter > 1)
                //{
                //    return;
                //}

                if (AppManager.Instance.Session != null)
                {
                    session = AppManager.Instance.Session;
                    Logger.Instance.Write("start printing for session: " + session.Id);
                }

                if (AppManager.Instance.Session != null)
                {
                    session = null;
                   // Logger.Instance.Write("start printing for session: " + session.Id);
                }

                BallotDefinition ballot = AppManager.Instance.ballot;

                if (session != null)
                {
                    vars.Add("void", session.CurrentLocale.Void);
                    vars.Add("voter", session.CurrentLocale.Voter);
                    vars.Add("machine", session.CurrentLocale.Machine);
                    vars.Add("no_selection", session.CurrentLocale.NoSelection);
                    vars.Add("you_wrote", string.Empty);

                    header.Board = session.Ballot.Board;
                    header.Address = session.Ballot.Address;
                    header.Voter = (string)vars["voter"] +
                                   ": " + Convert.ToString(session.Id);
                    header.Ballot = session.Ballot.Name + "\n" + session.Ballot.Title;
                    header.Date = session.Ballot.Date;

                    Logger.Instance.Write("printing " + header.Ballot);
                }
                else
                {
                    vars.Add("ballot_board", ballot.Board);
                    vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                    vars.Add("ballot_address", ballot.Address);
                    vars.Add("ballot_date", ballot.Date);
                    vars.Add("machine", "Machine");

                    header.Board = (string)vars["ballot_board"];
                    header.Address = (string)vars["ballot_address"];
                    header.Title = header.Address;
                    header.Ballot = (string)vars["ballot_name"];
                    header.Date = (string)vars["ballot_date"];

                    if (header.Date == "")
                        header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                    Logger.Instance.Write("printing " + header.Ballot);
                }

                var dataCount = 0;
                var selectedDataCount = 0;

                ContestDefinition condef = null;

                foreach (var contest in ballot.ContestsList)
                {
                    condef = contest;
                    List<Locale> locales;
                    if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                        locales = new List<Locale>();
                    Locale reportLocale;
                    int currentReportLocaleId = -1;
                    if (ballot.ReportLocaleId != null)
                        currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);
                    if (currentReportLocaleId > -1)
                        reportLocale = locales[currentReportLocaleId];
                    else
                        reportLocale = locales[0];

                    Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);
                    if (defaultLocale == null)
                        return;

                    defaultLocale = (doc == PrintDoc.REPORT) ? reportLocale : defaultLocale;
                    if (session != null)
                    {
                        defaultLocale = session.CurrentLocale;
                    }

                    Race currentRaceInstance = null;
                    Proposition currentPropositionInstance = null;

                    if (ballot.HasSlates)
                    {
                        ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale, null, true);
                    }
                    else
                    {
                        ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale);
                    }

                    ContestDefinition.FillContestData(ref condef, currentPropositionInstance, defaultLocale);

                    dataCount += contest.Data.Count;

                    var selectedDataCountLocal = contest.Data.Where(x => x.State == VrSelection.SelectionState.SELECTED).Count();
                    selectedDataCount += selectedDataCountLocal < 1 ? 1 : selectedDataCountLocal;

                    if (contest.Propositions == null)
                    {
                        var currentKey = "$b" + ballot.Id + "_g" + contest.Group + "_name";
                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, contest.GroupName);

                        currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_name";

                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, contest.Name);

                        foreach (DataDefinition data in contest.Data)
                        {
                            currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" + data.Id;

                            if (!vars.Contains(currentKey))
                                vars.Add(currentKey, data.Text);
                        }
                    }
                }

                header.Machine = (string)vars["machine"] +
                                 ": " + AppManager.Configuration["System"]["Machine"];

                Logger.Instance.Write("printing " + doc);

                pThread = null;

                int nWidth = 900;
                int margin = 5;
                int nHeight = 0;

                // adding contest count to account for contest titles
                selectedDataCount += ballot.ContestsList.Count();

                if (doc == PrintDoc.RECORD)
                {
                    nHeight = 45 * selectedDataCount;
                }
                else if (doc == PrintDoc.REPORT)
                {
                    nHeight = 24 * dataCount;
                }
                else
                {
                    nHeight = nWidth;
                }

                if (nHeight < 900)
                {
                    nHeight = 900;
                }

                // temp adding space
                nHeight += 125;

                PrintDocument document = null;

                if (AppManager.PrinterEnabled)
                {
                    document = new PrintDocument();
                    document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                }

                /// Jim Kapsis 18-06-2016
                if (AppManager.PrinterEnabled)
                {
                    switch (doc)
                    {
                        case PrintDoc.RECORD:
                            document.PrintPage += PrintRecord;
                            break;
                        case PrintDoc.REPORT:

                            // IGM 2016/1/22, temporary fix to address the png report height issue
                            // https://votrite.fogbugz.com/f/cases/5/Png-cut-off-when-there-are-less-than-6-candidates
                            // add extra 300
                            // document.DefaultPageSettings.PaperSize.Height = nHeight * 2;

                            // IGM 2016/3/9, fixed FB#23, report length is too big, has extra space at the bottom issue
                            //document.DefaultPageSettings.PaperSize.Height = nHeight + 300;
                            reportData = new ReportData(AppManager.Instance.ballot);
                            document.PrintPage += PrintReport;
                            break;
                        case PrintDoc.VOID:
                            //document.DefaultPageSettings.PaperSize.Height = 300;
                            document.PrintPage += PrintVoid;
                            break;
                        default:
                            break;
                    }

                    pThread = new Thread(document.Print);
                    pThread.Start();
                    pThread.Join();
                }
                else
                {
                    Bitmap B = new Bitmap(nWidth, nHeight);
                    PrintPageEventArgs ppeArgs = new PrintPageEventArgs(Graphics.FromImage(B),
                                                        new Rectangle(new Point(margin, margin), new Size(nWidth - margin * 2, nHeight - margin * 2)),
                                                        new Rectangle(new Point(0, 0), new Size(nWidth, nHeight)),
                                                        new PageSettings());
                    switch (doc)
                    {
                        case PrintDoc.RECORD:
                            PrintRecord(this, ppeArgs);
                            break;
                        case PrintDoc.REPORT:
                            reportData = new ReportData(AppManager.Instance.ballot);
                            PrintReport(this, ppeArgs);
                            break;
                        case PrintDoc.VOID:
                            PrintVoid(this, ppeArgs);
                            break;
                        default:
                            break;
                    }

                    B.Dispose();
                    B = null;
                    ppeArgs = null;
                }

                /// Jim Kapsis.
                if (AppManager.PrinterEnabled)
                {
                    document.Dispose();
                    document = null;
                    if (cut) CutPaper();
                }

                reportData = null;
                vars.Clear();

                Logger.Instance.Write("exit printing");

            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public void Print()
        {
            try
            {


                ballot = Ballot.GetBallot(Convert.ToInt32(ballotid));
                slates = Slate.GetSlates(Convert.ToInt32(ballotid), Convert.ToInt32(ballot.ReportLocaleId));
                header.Board = ballot.Board;
                header.Address = ballot.Location;
                //header.Voter = (string)vars["voter"] + ": " + Convert.ToString(session.Id);
                header.Ballot = ballot.ElectionName + "\n" + ballot.TopHeading;
               // header.Date = ballot.
                header.Printed = false;
               // header.Ballot = ballot.
               // slates = Slate.GetSlates(Convert.ToInt32(ballotid), ballot.);
                var dataCount = 0;
                var selectedDataCount = 0;


                pThread = null;

                int nWidth = 900;
                int margin = 5;
                int nHeight = 0;

                // adding contest count to account for contest titles
                var query = "select * from contest where ballot_id=" + ballotid; // "select bid, cid, did, cand_name, vdate, cnt, slate_id, pid from counter where bid=" + ballotid;

                DataTable Data = DataManager.GetData_audit(query);
                selectedDataCount += Data.Rows.Count;
                foreach (DataRow dr in Data.Rows)
                {
                    var cdef = new ContestDefinition();
                    cdef.Id = Convert.ToInt32(dr["contest_id"]);
                    Enum.TryParse(dr["contest_type"].ToString(), out ContestTypes myStatus);
                    cdef.Type = myStatus;
                    Race currentRaceInstance = (Race)null;
                    Proposition currentPropositionInstance = (Proposition)null;

                    ContestDefinition.SetContestFields(ref cdef, ref currentRaceInstance, ref currentPropositionInstance, Locale.DefaultLocale, Convert.ToInt32(ballotid));
                    ContestDefinition.FillContestData(ref cdef, currentPropositionInstance, Locale.DefaultLocale);

                    contests.Add(cdef);
                }


                nHeight = 45 * selectedDataCount;


                if (nHeight < 900)
                {
                    nHeight = 900;
                }

                // temp adding space
                nHeight += 125;

                PrintDocument document = null;


                Bitmap B = new Bitmap(nWidth, nHeight);
                PrintPageEventArgs ppeArgs = new PrintPageEventArgs(Graphics.FromImage(B),
                                                    new Rectangle(new Point(margin, margin), new Size(nWidth - margin * 2, nHeight - margin * 2)),
                                                    new Rectangle(new Point(0, 0), new Size(nWidth, nHeight)),
                                                    new PageSettings());
                //PrintRecord(this, ppeArgs);
                PrintRecord_Audit(this, ppeArgs);

                B.Dispose();
                B = null;
                ppeArgs = null;


                reportData = null;
                vars.Clear();

            }
            catch (Exception e)
            {

            }
        }


        public void Print(PrintDoc doc, bool cut)
        {
            try
            {
                if(doc == PrintDoc.VOID)
                {
                    try
                    {
                        if (AppManager.Instance.PrintFinalReportStarted)
                            return;
                    }
                    catch (Exception)
                    {
                    }
                    
                }
            }
            catch (Exception)
            {

            }
            try
            {
                //if (doc == PrintDoc.REPORT && AppManager.Instance.FinalReportPrintCounter > 1)
                //{
                //    return;
                //}
                if (doc == PrintDoc.RECORD)
                    PrintForPDFnPrinter(doc, false);
                else
                {
                    if (AppManager.Instance.Session != null)
                    {
                        session = AppManager.Instance.Session;
                        Logger.Instance.Write("start printing for session: " + session.Id);
                    }
                    if (doc == PrintDoc.REPORT)
                        session = null;

                    BallotDefinition ballot = AppManager.Instance.ballot;
                    vars.Clear();

                    if (session != null)
                    {
                        vars.Add("void", session.CurrentLocale.Void);
                        vars.Add("voter", session.CurrentLocale.Voter);
                        vars.Add("machine", session.CurrentLocale.Machine);
                        vars.Add("no_selection", session.CurrentLocale.NoSelection);
                        vars.Add("you_wrote", string.Empty);

                        header.Board = session.Ballot.Board;
                        header.Address = session.Ballot.Address;
                        header.Voter = (string)vars["voter"] +
                                       ": " + Convert.ToString(session.Id);
                        header.Ballot = session.Ballot.Name + "\n" + session.Ballot.Title;
                        header.Date = session.Ballot.Date;

                        Logger.Instance.Write("printing " + header.Ballot);
                    }
                    else
                    {
                        vars.Add("ballot_board", ballot.Board);
                        vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                        vars.Add("ballot_address", ballot.Address);
                        vars.Add("ballot_date", ballot.Date);
                        vars.Add("machine", "Machine");

                        header.Board = (string)vars["ballot_board"];
                        header.Address = (string)vars["ballot_address"];
                        header.Title = header.Address;
                        header.Ballot = (string)vars["ballot_name"];
                        header.Date = (string)vars["ballot_date"];

                        if (header.Date == "")
                            header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                        Logger.Instance.Write("printing " + header.Ballot);
                    }

                    var dataCount = 0;
                     int selectedDataCount = 0;

                    ContestDefinition condef = null;

                    foreach (var contest in ballot.ContestsList)
                    {
                        condef = contest;
                        List<Locale> locales;
                        if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                            locales = new List<Locale>();
                        Locale reportLocale;
                        int currentReportLocaleId = -1;
                        if (ballot.ReportLocaleId != null)
                            currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);
                        if (currentReportLocaleId > -1)
                            reportLocale = locales[currentReportLocaleId];
                        else
                            reportLocale = locales[0];

                        Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);
                        if (defaultLocale == null)
                            return;

                        defaultLocale = (doc == PrintDoc.REPORT) ? reportLocale : defaultLocale;
                        if (session != null)
                        {
                            defaultLocale = session.CurrentLocale;
                        }

                        Race currentRaceInstance = null;
                        Proposition currentPropositionInstance = null;

                        if (ballot.HasSlates)
                        {
                            ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale, null, true);
                        }
                        else
                        {
                            ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale);
                        }

                        ContestDefinition.FillContestData(ref condef, currentPropositionInstance, defaultLocale);

                        dataCount += contest.Data.Count;

                        var selectedDataCountLocal = contest.Data.Where(x => x.State == VrSelection.SelectionState.SELECTED ? true : x.Preference > 0).Count();
                        selectedDataCount += selectedDataCountLocal < 1 ? 1 : selectedDataCountLocal;

                        if (contest.Propositions == null)
                        {
                            var currentKey = "$b" + ballot.Id + "_g" + contest.Group + "_name";
                            if (!vars.Contains(currentKey))
                                vars.Add(currentKey, contest.GroupName);

                            currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_name";

                            if (!vars.Contains(currentKey))
                                vars.Add(currentKey, contest.Name);

                            foreach (DataDefinition data in contest.Data)
                            {
                                currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" + data.Id;

                                if (!vars.Contains(currentKey))
                                    vars.Add(currentKey, data.Text);
                            }
                        }
                    }

                    header.Machine = (string)vars["machine"] +
                                     ": " + AppManager.Configuration["System"]["Machine"];

                    Logger.Instance.Write("printing " + doc);
                }

                pThread = null;

                int nWidth = 900;
                int margin = 5;
                int nHeight = 1;

                // adding contest count to account for contest titles
            //  selectedDataCount = eballot.ContestsList.Count();

                //if (doc == PrintDoc.RECORD)
                //{
                //    nHeight = 45 * selectedDataCount;
                //}
                //else if (doc == PrintDoc.REPORT)
                //{
                //    nHeight = 24 * dataCount;
                //    //nHeight = 18 * dataCount;
                //}
                //else
                //{
                //    nHeight = nWidth;
                //}

                //if (nHeight < 900)
                //{
                //    nHeight = 900;
                //}

                //// temp adding space
                //nHeight += 12; //12.5;

                PrintDocument document = null;

                if (AppManager.PrinterEnabled)
                {
                    document = new PrintDocument();
                    try
                    {
                        PrinterSettings settings = new PrinterSettings();
                        //return settings.PrinterName;

                        document.PrinterSettings.PrinterName = settings.PrinterName;
                    }
                    catch (Exception)
                    {
                    }
                    
                    //document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);

                    //document.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                }
                
                linesprinted = 0;
                fileprintcomplete = false;
                mListPrint = new List<textandfont>();
                header.Printed = false;
                /// Jim Kapsis 18-06-2016
                if (AppManager.PrinterEnabled)
                {
                    switch (doc)
                    {
                        case PrintDoc.RECORD:
                            if (AppManager.Configuration["Printer"]["Enabled"] == "false")
                            {
                                //Bitmap B = new Bitmap(nWidth, nHeight);
                                Bitmap B = new Bitmap(document.DefaultPageSettings.Bounds.Width, document.DefaultPageSettings.Bounds.Height);
                                PrintPageEventArgs ppeArgs = new PrintPageEventArgs(Graphics.FromImage(B),
                                                                    new Rectangle(new Point(margin, margin), new Size(document.DefaultPageSettings.Bounds.Width - margin * 2, document.DefaultPageSettings.Bounds.Height - margin * 2)),
                                                                    new Rectangle(new Point(0, 0), new Size(document.DefaultPageSettings.Bounds.Width, document.DefaultPageSettings.Bounds.Height)),
                                                                    new PageSettings());

                                PrintRecord_to_PDF_Printer(this, ppeArgs);
                            }
                            else
                                document.PrintPage += PrintRecord_to_PDF_Printer;
                            //PrintRecord_img();
                            break;
                        case PrintDoc.REPORT:

                            // IGM 2016/1/22, temporary fix to address the png report height issue
                            // https://votrite.fogbugz.com/f/cases/5/Png-cut-off-when-there-are-less-than-6-candidates
                            // add extra 300
                            // document.DefaultPageSettings.PaperSize.Height = nHeight * 2;

                            // IGM 2016/3/9, fixed FB#23, report length is too big, has extra space at the bottom issue
                            //
                            if (AppManager.Instance.Session != null)
                                reportData = new ReportData();
                            else
                                reportData = new ReportData(AppManager.Instance.ballot);
                            //document.DefaultPageSettings.PaperSize.Height = 407;// nHeight + 400;
                            //document.PrintPage += PrintReport;


                            document.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
                            ////document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                            document.PrintPage += new PrintPageEventHandler(PrintReport);
                            // Print the document.
                            //document.Print();
                            
                            break;
                        case PrintDoc.VOID:
                            //document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                            //document.DefaultPageSettings.PaperSize.Height = 70;
                            document.PrintPage += PrintVoid;
                            
                            break;
                        default:
                            break;
                    }
                    if (AppManager.Configuration["Printer"]["Enabled"] == "false" && doc == PrintDoc.RECORD)
                    {

                    }
                    else
                    {
                        pThread = new Thread(document.Print);
                        pThread.Start();
                        pThread.Join();
                    }

                }
                else
                {
                    Bitmap B = new Bitmap(nWidth, nHeight);
                    PrintPageEventArgs ppeArgs = new PrintPageEventArgs(Graphics.FromImage(B),
                                                        new Rectangle(new Point(margin, margin), new Size(nWidth - margin * 2, nHeight - margin * 2)),
                                                        new Rectangle(new Point(0, 0), new Size(nWidth, nHeight)),
                                                        new PageSettings());
                    switch (doc)
                    {
                        case PrintDoc.RECORD:
                            PrintRecord(this, ppeArgs);
                            break;
                        case PrintDoc.REPORT:
                            reportData = new ReportData(AppManager.Instance.ballot);
                            PrintReport(this, ppeArgs);
                            break;
                        case PrintDoc.VOID:
                            PrintVoid(this, ppeArgs);
                            break;
                        default:
                            break;
                    }

                    B.Dispose();
                    B = null;
                    ppeArgs = null;
                }

                /// Jim Kapsis.
                if (AppManager.PrinterEnabled)
                {
                    document.Dispose();
                    document = null;
                    if (cut) CutPaper();
                }

                reportData = null;
                vars.Clear();

                

                Logger.Instance.Write("exit printing");

            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public void PrintForPDFnPrinter(PrintDoc doc, bool cut)
        {

            PrintDocument document = new PrintDocument();
                
            try
            {
                //if (doc == PrintDoc.REPORT && AppManager.Instance.FinalReportPrintCounter > 1)
                //{
                //    return;
                //}
               // document.
                if (AppManager.Instance.Session != null)
                {
                    session = AppManager.Instance.Session;
                    Logger.Instance.Write("start printing for session: " + session.Id);
                }
                if (doc == PrintDoc.REPORT)
                    session = null;

                BallotDefinition ballot = AppManager.Instance.ballot;

                
                    vars.Add("ballot_board", ballot.Board);
                    vars.Add("ballot_name", ballot.Name + "\n" + ballot.Title);
                    vars.Add("ballot_address", ballot.Address);
                    vars.Add("ballot_date", ballot.Date);
                    vars.Add("machine", "Machine");

                    header.Board = (string)vars["ballot_board"];
                    header.Address = (string)vars["ballot_address"];
                    header.Title = header.Address;
                    header.Ballot = (string)vars["ballot_name"];
                    header.Date = (string)vars["ballot_date"];

                    if (header.Date == "")
                        header.Date = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

                    Logger.Instance.Write("printing " + header.Ballot);
                

                var dataCount = 0;
                var selectedDataCount = 0;

                ContestDefinition condef = null;

                foreach (var contest in ballot.ContestsList)
                {
                    condef = contest;
                    List<Locale> locales;
                    if ((locales = Locale.FetchLocales(AppManager.BallotId)) == null)
                        locales = new List<Locale>();
                    Locale reportLocale;
                    int currentReportLocaleId = -1;
                    if (ballot.ReportLocaleId != null)
                        currentReportLocaleId = locales.FindIndex(l => l.Id == ballot.ReportLocaleId);
                    if (currentReportLocaleId > -1)
                        reportLocale = locales[currentReportLocaleId];
                    else
                        reportLocale = locales[0];

                    Locale defaultLocale = Locale.GetDefaultLocale(AppManager.BallotId);
                    if (defaultLocale == null)
                        return;

                    defaultLocale = (doc == PrintDoc.REPORT) ? reportLocale : defaultLocale;
                    //if (session != null)
                    //{
                    //    defaultLocale = session.CurrentLocale;
                    //}

                    Race currentRaceInstance = null;
                    Proposition currentPropositionInstance = null;

                    if (ballot.HasSlates)
                    {
                        ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale, null, true);
                    }
                    else
                    {
                        ContestDefinition.SetContestFields(ref condef, ref currentRaceInstance, ref currentPropositionInstance, defaultLocale);
                    }

                    ContestDefinition.FillContestData(ref condef, currentPropositionInstance, defaultLocale);

                    dataCount += contest.Data.Count;

                    var selectedDataCountLocal = contest.Data.Where(x => x.State == VrSelection.SelectionState.SELECTED ? true : x.Preference > 0).Count();
                    selectedDataCount += selectedDataCountLocal < 1 ? 1 : selectedDataCountLocal;

                    if (contest.Propositions == null)
                    {
                        var currentKey = "$b" + ballot.Id + "_g" + contest.Group + "_name";
                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, contest.GroupName);

                        currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_name";

                        if (!vars.Contains(currentKey))
                            vars.Add(currentKey, contest.Name);

                        foreach (DataDefinition data in contest.Data)
                        {
                            currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" + data.Id;

                            if (!vars.Contains(currentKey))
                                vars.Add(currentKey, data.Text);
                        }
                    }
                }

                header.Machine = (string)vars["machine"] +
                                 ": " + AppManager.Configuration["System"]["Machine"];

                Logger.Instance.Write("printing " + doc);

                pThread = null;

                int nWidth = 900;
                int margin = 5;
                int nHeight = 0;

                // adding contest count to account for contest titles
                selectedDataCount += ballot.ContestsList.Count();

                if (doc == PrintDoc.RECORD)
                {
                    nHeight = 45 * selectedDataCount;
                }
                else if (doc == PrintDoc.REPORT)
                {
                    nHeight = 24 * dataCount;
                    //nHeight = 18 * dataCount;
                }
                else
                {
                    nHeight = nWidth;
                }

                if (nHeight < 900)
                {
                    nHeight = 900;
                }

                // temp adding space
                nHeight += 12; //12.5;
                //if (AppManager.PrinterEnabled)
                //{
                //    document = new PrintDocument();
                // }
                // document.

                eballot = ballot;

                linesprinted = 0;
                fileprintcomplete = false;
                mListPrint = new List<textandfont>();
                header.Printed = false;
                /// Jim Kapsis 18-06-2016
                //if (AppManager.PrinterEnabled)
                //{
                //    switch (doc)
                //    {
                //        case PrintDoc.RECORD:
                //            document.PrintPage += PrintRecord_to_PDF_Printer;
                //            break;
                //        case PrintDoc.REPORT:                            
                //            //if (AppManager.Instance.Session != null)
                //            //    reportData = new ReportData();
                //            //else
                //            //    reportData = new ReportData(AppManager.Instance.ballot);
                            
                //            //document.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
                //            //////document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                //            //document.PrintPage += new PrintPageEventHandler(PrintReport);
                //            break;                       
                         
                //        default:
                //            break;
                //    }
                //    pThread = new Thread(document.Print);
                //    pThread.Start();
                //    pThread.Join();

                //}
                //Logger.Instance.Write("PDF printing");

            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }


        public void Print(PrintDoc doc, bool cut, bool pIsEmpty)
        {
            try
            {
                Logger.Instance.Write("start printing empty report: ");

                pThread = null;

                int nWidth = 900, margin = 5, nHeight = 100;

                // temp adding space
                nHeight += 125;

                PrintDocument document = null;


                if (AppManager.PrinterEnabled)
                {
                    document = new PrintDocument();
                    document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
                }

                /// Jim Kapsis 18-06-2016
                if (AppManager.PrinterEnabled)
                {
                    switch (doc)
                    {
                        case PrintDoc.REPORT:
                            document.PrintPage += PrintEmptyReport;
                            break;
                        default:
                            break;
                    }

                    pThread = new Thread(document.Print);
                    pThread.Start();
                    pThread.Join();
                }
                else
                {
                    Bitmap B = new Bitmap(nWidth, nHeight);
                    PrintPageEventArgs ppeArgs = new PrintPageEventArgs(Graphics.FromImage(B),
                                                        new Rectangle(new Point(margin, margin), new Size(nWidth - margin * 2, nHeight - margin * 2)),
                                                        new Rectangle(new Point(0, 0), new Size(nWidth, nHeight)),
                                                        new PageSettings());
                    switch (doc)
                    {
                        case PrintDoc.REPORT:
                            PrintEmptyReport(this, ppeArgs);
                            break;
                        default:
                            break;
                    }

                    B.Dispose();
                    B = null;
                    ppeArgs = null;
                }

                /// Jim Kapsis.
                if (AppManager.PrinterEnabled)
                {
                    document.Dispose();
                    document = null;
                    if (cut) CutPaper();
                }

                reportData = null;
                vars.Clear();

                Logger.Instance.Write("exit printing");

            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        private void WriteText(ref Graphics gr, ref PrintPageEventArgs args,
            string text, ref float top, float left, Font font, StringFormat format)
        {
            if (fileprintcomplete)
                return;
            if (text == null)
            {
                gr.DrawString("", font, Brushes.Black, left, top, format);
                if (addInPrintList)
                    mListPrint.Add(new textandfont { text = "", font = font });
                return;
            }

            createimgpage(ref gr, ref args,text, ref  top,  left,  font,  format);

            text = text.Replace("\n", " / ");
            string[] tmpText;
            string line = "";
            SizeF textSize = gr.MeasureString(text, font);
            float maxWidth = 400;

            int ix = text.IndexOf("  ");

            if (ix > 0)
            {
                line = text.Insert(ix, " / ");

                var words = line.Split(' ');
                string txt = "";
                foreach (string word in words)
                {
                    txt += (word == " " || word == "" ? "" : word + " ");
                }
                text = txt;
                line = "";
            }           

            if (textSize.Width > maxWidth)
            {
                tmpText = text.Split(new char[] { ' ' });
                float lineW = 0f;

                for (int i = 0; i < tmpText.Length; i++)
                {
                    line += tmpText[i] + " ";
                    lineW = gr.MeasureString(line, font).Width;

                    float nextWordW = 0;

                    if (i < tmpText.Length - 1)
                        nextWordW = gr.MeasureString(tmpText[i + 1] + " ", font).Width;
                    else
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        if (addInPrintList)
                            mListPrint.Add(new textandfont { text = line, font = font });
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        if (addInPrintList)
                            mListPrint.Add(new textandfont { text = line, font = font });
                        top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {                
                gr.DrawString(text, font, Brushes.Black, left, top, format);

                if (addInPrintList)
                    mListPrint.Add(new textandfont { text = text, font = font});
            }
        }

        private void WriteText_English(ref Graphics gr, ref PrintPageEventArgs args,
            string text, ref float top, float left, Font font, StringFormat format)
        {
            if (fileprintcomplete)
                return;
            if (text == null)
            {
                gr.DrawString("", font, Brushes.Black, left, top, format);
                if (addInPrintList)
                    mListPrint.Add(new textandfont { text = "", font = font });
                return;
            }

            createimgpage_English(ref gr, ref args, text, ref top, left, font, format);

            text = text.Replace("\n", " / ");
            string[] tmpText;
            string line = "";
            SizeF textSize = gr.MeasureString(text, font);
            float maxWidth = 400;

            int ix = text.IndexOf("  ");

            if (ix > 0)
            {
                line = text.Insert(ix, " / ");

                var words = line.Split(' ');
                string txt = "";
                foreach (string word in words)
                {
                    txt += (word == " " || word == "" ? "" : word + " ");
                }
                text = txt;
                line = "";
            }

            if (textSize.Width > maxWidth)
            {
                tmpText = text.Split(new char[] { ' ' });
                float lineW = 0f;

                for (int i = 0; i < tmpText.Length; i++)
                {
                    line += tmpText[i] + " ";
                    lineW = gr.MeasureString(line, font).Width;

                    float nextWordW = 0;

                    if (i < tmpText.Length - 1)
                        nextWordW = gr.MeasureString(tmpText[i + 1] + " ", font).Width;
                    else
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        if (addInPrintList)
                            mListPrint.Add(new textandfont { text = line, font = font });
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        if (addInPrintList)
                            mListPrint.Add(new textandfont { text = line, font = font });
                        top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {
                gr.DrawString(text, font, Brushes.Black, left, top, format);

                if (addInPrintList)
                    mListPrint.Add(new textandfont { text = text, font = font });
            }
        }
        private void WriteText_img(ref Graphics gr, ref PrintPageEventArgs args,
            string text, ref float top, float left, Font font, StringFormat format)
        {
            if (fileprintcomplete)
                return;
            if (text == null)
            {
                gr.DrawString("", font, Brushes.Black, left, top, format);
                return;
            }

            createimgpage(ref gr, ref args, text, ref top, left, font, format);

            text = text.Replace("\n", " / ");
            string[] tmpText;
            string line = "";
            SizeF textSize = gr.MeasureString(text, font);
            float maxWidth = 400;

            int ix = text.IndexOf("  ");

            if (ix > 0)
            {
                line = text.Insert(ix, " / ");

                var words = line.Split(' ');
                string txt = "";
                foreach (string word in words)
                {
                    txt += (word == " " || word == "" ? "" : word + " ");
                }
                text = txt;
                line = "";
            }

            if (textSize.Width > maxWidth)
            {
                tmpText = text.Split(new char[] { ' ' });
                float lineW = 0f;

                for (int i = 0; i < tmpText.Length; i++)
                {
                    line += tmpText[i] + " ";
                    lineW = gr.MeasureString(line, font).Width;

                    float nextWordW = 0;

                    if (i < tmpText.Length - 1)
                        nextWordW = gr.MeasureString(tmpText[i + 1] + " ", font).Width;
                    else
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        gr.DrawString(line, font, Brushes.Black, left, top, format);
                        top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {
                gr.DrawString(text, font, Brushes.Black, left, top, format);
            }
        }

        private void WriteText_toPrinter(ref Graphics gr, ref PrintPageEventArgs args,
           string text, ref float top, float left, Font font, StringFormat format)
        {
            if (fileprintcomplete)
                return;
            if (text == null)
            {
                mListPrint.Add(new textandfont { text = "", font = font });
                return;
            }

            text = text.Replace("\n", " / ");
            string[] tmpText;
            string line = "";
            SizeF textSize = gr.MeasureString(text, font);
            float maxWidth = 400;

            int ix = text.IndexOf("  ");

            if (ix > 0)
            {
                line = text.Insert(ix, " / ");

                var words = line.Split(' ');
                string txt = "";
                foreach (string word in words)
                {
                    txt += (word == " " || word == "" ? "" : word + " ");
                }
                text = txt;
                line = "";
            }

            if (textSize.Width > maxWidth)
            {
                tmpText = text.Split(new char[] { ' ' });
                float lineW = 0f;

                for (int i = 0; i < tmpText.Length; i++)
                {
                    line += tmpText[i] + " ";
                    lineW = gr.MeasureString(line, font).Width;

                    float nextWordW = 0;

                    if (i < tmpText.Length - 1)
                        nextWordW = gr.MeasureString(tmpText[i + 1] + " ", font).Width;
                    else
                    {
                        mListPrint.Add(new textandfont { text = line, font = font });
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        mListPrint.Add(new textandfont { text = line, font = font });
                        top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {
                mListPrint.Add(new textandfont { text = text, font = font });
            }
        }


        private void createimgpage(ref Graphics tempGr, ref PrintPageEventArgs args,
            string text, ref float yPos, float leftMargin, Font font, StringFormat format)
        {
            if (((yPos + titleFont.GetHeight(args.Graphics) +
                         2*(float)V_SPACE) >= args.PageBounds.Bottom))
            {

                //break;
                string filename = "";
                if (folder == "\\records")
                    filename = session.Id + "_" + imgpg + ".png";
                else
                {
                    filename = reportname + "_" + imgpg + ".png";
                }

                try
                {
                    string recordsFolderPath = Global.Instance.APP_PATH + folder+ "\\";
                    if (!Directory.Exists(recordsFolderPath))
                        Directory.CreateDirectory(recordsFolderPath);

                    mainbmp.Save(recordsFolderPath + filename);
                    //  outputImage = recordsFolderPath + bflname;
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + folder+ "\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    mainbmp.Save(reportsFolderPath + filename);
                }
                catch (Exception e) { Logger.Instance.Write(e); }

                try
                {
                    string backupFlashDriveRecordsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] +
                                                         folder + "\\";
                    if (!Directory.Exists(backupFlashDriveRecordsPath))
                        Directory.CreateDirectory(backupFlashDriveRecordsPath);

                    mainbmp.Save(backupFlashDriveRecordsPath + filename);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }
                imgpg++;
                yPos = TopMargin;

                tempGr.Clear(Color.Transparent);
                if (AppManager.Instance.headerInRecordImage)
                {
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    addInPrintList = false;
                    WriteText(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                    //export.AddRowData(cells, "");
                    WriteText(ref tempGr, ref args, header.Board, ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, header.Board, bold: true, cellsUnderline: true, centerAlign: false);

                    //if (session.Ballot.HasCounties)
                    //{
                    //    WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                    //              leftMargin, hdrFont, format);
                    //    //export.AddRowData(cells, session.Ballot.Address, bold: true, cellsUnderline: true, centerAlign: false);
                    //    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    //}

                    WriteText(ref tempGr, ref args, header.Title, ref yPos,
                              leftMargin, hdrFont, format);
                    //if (header.Title != null)
                    //    export.AddRowData(cells, header.Title, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                    yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, header.Ballot.Split('\n')[0], bold: true, cellsUnderline: true, centerAlign: false);
                    //export.AddRowData(cells, header.Ballot.Split('\n')[1], bold: true, cellsUnderline: true, centerAlign: false);

                    WriteText(ref tempGr, ref args, header.Date, ref yPos,
                              leftMargin, hdrFont, format);
                    // export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Voter, ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, header.Voter, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);
                    //export.AddRowData(cells, header.Machine, bold: true, cellsUnderline: true, centerAlign: false);
                    //export.AddRowData(cells, "======VERIFIED BY VOTER======", bold: true, cellsUnderline: false, centerAlign: false);
                    WriteText(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    WriteText(ref tempGr, ref args, "continued page " + imgpg + " --->", ref yPos, leftMargin, recFont, format);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                }
                addInPrintList = true;
            }

        }

        private void createimgpage_English(ref Graphics tempGr, ref PrintPageEventArgs args,
           string text, ref float yPos, float leftMargin, Font font, StringFormat format)
        {
            if (((yPos + titleFont.GetHeight(args.Graphics) +
                         2*(float)V_SPACE) >= args.PageBounds.Bottom))
            {

                //break;
                string filename = "";
                if (folder == "\\records")
                    filename = session.Id + "_" + imgpg + "_English.png";
                else
                {
                    filename = reportname + "_" + imgpg + ".png";
                }

                try
                {
                    string recordsFolderPath = Global.Instance.APP_PATH + folder + "\\";
                    if (!Directory.Exists(recordsFolderPath))
                        Directory.CreateDirectory(recordsFolderPath);

                    mainbmp.Save(recordsFolderPath + filename);
                    //  outputImage = recordsFolderPath + bflname;
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }

                try
                {
                    string reportsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + folder + "\\";

                    if (!Directory.Exists(reportsFolderPath))
                        Directory.CreateDirectory(reportsFolderPath);

                    mainbmp.Save(reportsFolderPath + filename);
                }
                catch (Exception e) { Logger.Instance.Write(e); }

                try
                {
                    string backupFlashDriveRecordsPath = AppManager.Configuration["System"]["BackupFlashDrivePath"] +
                                                         folder + "\\";
                    if (!Directory.Exists(backupFlashDriveRecordsPath))
                        Directory.CreateDirectory(backupFlashDriveRecordsPath);

                    mainbmp.Save(backupFlashDriveRecordsPath + filename);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }
                imgpg++;
                yPos = TopMargin;

                tempGr.Clear(Color.Transparent);
                if (AppManager.Instance.headerInRecordImage)
                {
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    addInPrintList = false;
                    WriteText(ref tempGr, ref args, "==PAPER AUDIT TRAIL(" + imgpg + ")==", ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, "==PAPER AUDIT TRAIL==", bold: true);
                    //export.AddRowData(cells, "");
                    WriteText(ref tempGr, ref args, header.Board, ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, header.Board, bold: true, cellsUnderline: true, centerAlign: false);

                    //if (session.Ballot.HasCounties)
                    //{
                    //    WriteText(ref tempGr, ref args, session.Ballot.Address, ref yPos,
                    //              leftMargin, hdrFont, format);
                    //    //export.AddRowData(cells, session.Ballot.Address, bold: true, cellsUnderline: true, centerAlign: false);
                    //    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    //}

                    WriteText(ref tempGr, ref args, header.Title, ref yPos,
                              leftMargin, hdrFont, format);
                    //if (header.Title != null)
                    //    export.AddRowData(cells, header.Title, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Ballot, ref yPos, leftMargin, hdrFont, format);
                    yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);

                    //export.AddRowData(cells, header.Ballot.Split('\n')[0], bold: true, cellsUnderline: true, centerAlign: false);
                    //export.AddRowData(cells, header.Ballot.Split('\n')[1], bold: true, cellsUnderline: true, centerAlign: false);

                    WriteText(ref tempGr, ref args, header.Date, ref yPos,
                              leftMargin, hdrFont, format);
                    // export.AddRowData(cells, header.Date, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Voter, ref yPos,
                              leftMargin, hdrFont, format);
                    //export.AddRowData(cells, header.Voter, bold: true, cellsUnderline: true, centerAlign: false);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);

                    WriteText(ref tempGr, ref args, header.Machine, ref yPos,
                              leftMargin, hdrFont, format);
                    yPos += V_SPACE * 10 + titleFont.GetHeight(args.Graphics);
                    //export.AddRowData(cells, header.Machine, bold: true, cellsUnderline: true, centerAlign: false);
                    //export.AddRowData(cells, "======VERIFIED BY VOTER======", bold: true, cellsUnderline: false, centerAlign: false);
                    WriteText(ref tempGr, ref args, "======VERIFIED BY VOTER======", ref yPos, leftMargin, hdrFont, format);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    WriteText(ref tempGr, ref args, "continued page " + imgpg + " --->", ref yPos, leftMargin, recFont, format);

                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                    yPos += V_SPACE + titleFont.GetHeight(args.Graphics);
                }
                //addInPrintList = true;
            }

        }
        private void addlineFont(Font font)
        {
            mListPrint.Add(new textandfont { text = "--Line--", font = font });
        }

        private string bettertext(string text)
        {
            try
            {
                int ix = text.IndexOf("  ");
                string line = "";

                if (ix > 0)
                {
                    line = text.Insert(ix, " / ");

                    var words = line.Split(' ');
                    string txt = "";
                    foreach (string word in words)
                    {
                        txt += (word == " " || word == "" ? "" : word + " ");
                    }
                    text = txt;
                    line = "";
                }
            }
            catch (Exception)
            {

            }
            return text;
        }
        private float IncreaseForOneThird(float number)
        {
            return number * 1;
        }

        
    }
}
