using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GemBox.Spreadsheet;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class RecordReportPrint : Form
    {
        string reportsPath = Global.Instance.APP_PATH  + "reports\\";
        string recordsPath = Global.Instance.APP_PATH  + "records\\";
        public RecordReportPrint()
        {
            InitializeComponent();
        }

        private void RecordReportPrint_Load(object sender, EventArgs e)
        {
            var dates = Directory.GetFiles(recordsPath, "*.png").OrderByDescending(d => new FileInfo(d).CreationTime).
                Select(d => new FileInfo(d).CreationTime.Date.ToString("dd-MMM-yyyy")).Distinct().ToList();

            dates.Insert(0, "--SELECT--");
            comboRecords_dates.DataSource = dates;
        }

        private void comboRecords_dates_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboRecords_dates.SelectedIndex == 0)
                {
                    gvRecord.DataSource = null;
                    gvRecord.Refresh();
                    return;
                }
                var date = comboRecords_dates.Text;
                var files = Directory.GetFiles(recordsPath, "*.png").
               Where(d => new FileInfo(d).CreationTime.Date.ToString("dd-MMM-yyyy") == date).OrderBy(d => new FileInfo(d).CreationTime);
                var records = new List<RecordReportFiles>();
                int sn = 1;
                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    if (radioLanguage.Checked)

                    {
                        if (fi.Name.EndsWith("_English.png"))
                            continue;
                        else
                            records.Add(new RecordReportFiles() { SN = sn++, RecordSession = fi.Name.Replace(".png", ""), RecordTime = fi.CreationTime });
                    }
                    if (radioeEnglish.Checked)
                    {
                        if (!fi.Name.EndsWith("_English.png"))
                            continue;
                        else
                            records.Add(new RecordReportFiles() { SN = sn++, RecordSession = fi.Name.Replace(".png", ""), RecordTime = fi.CreationTime });
                    }
                }
                gvRecord.DataSource = records;
                gvRecord.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private void btnRecord_print_Click(object sender, EventArgs e)
        {
            for (int r = gvRecord.Rows.Count - 1; r >= 0; r--)
            {
                try
                {
                    printimg = Path.Combine(recordsPath, gvRecord.Rows[r].Cells[1].Value.ToString() + ".png");
                    try
                    {
                        PrintDocument document = new PrintDocument();
                        document.PrintPage += new PrintPageEventHandler(PrintRecord_to_PDF_Printer);
                        printerName = document.PrinterSettings.PrinterName;
                        document.Print();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                catch (Exception)
                {

                }
            }


            

        }
        string printerName = "";
        string printimg = "";
        private void PrintRecord_to_PDF_Printer(object sender, PrintPageEventArgs args)
        {
            Graphics gOut = args.Graphics;
            Bitmap bmp = null;
            
                try
                {
                    bmp = new Bitmap(printimg);
                    gOut.DrawImage(bmp, 0f, 0f);

                }
                catch (Exception ex)
                {
                }
            
        }

        public bool PrintPDF(string printer,string paperName,string filename,  int copies)
        {
            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printer,
                    Copies = (short)copies,
                };

                // Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0), 
                };
                

                // Now print the PDF document
                using (var document = PdfiumViewer.PdfDocument.Load(filename))
                {
                     pageSettings.PaperSize = new PaperSize("User", 400, 2400);
                    int cnt = document.PageCount;
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void radioLanguage_CheckedChanged(object sender, EventArgs e)
        {
            comboRecords_dates_SelectedIndexChanged(null, null);
        }

        private void radioeEnglish_CheckedChanged(object sender, EventArgs e)
        {
            comboRecords_dates_SelectedIndexChanged(null, null);
        }

        private void btn_excl_browse_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lines = new List<string>();
                Thread thread = new Thread((ThreadStart)(() =>
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = true;
                    ofd.Filter = "Excel Files(.xlsx)|*.xlsx";
                    ofd.ShowDialog();
                    if (ofd.FileNames.Length == 0)
                        MessageBox.Show("Please choose a file");
                    else
                    {
                        foreach (var file in ofd.FileNames)
                        {
                            printGem(file);
                            //lines = ReadExcel(ofd.FileName);
                        }
                    }
                }));

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {

            }
        }

        private void ReadExcel(string file)
        {
            var table = new List<string>();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(file, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();
                //foreach (Cell cell in rows.ElementAt(0))
                //{
                //    table.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                //}
                //this will also include your header row...
                foreach (Row row in rows)
                {
                    string item = "";
                    string txt = "";
                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        txt = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                        item += (txt != "" ? txt + " " : "");
                    }
                    if (item.EndsWith(" "))
                        item = item.Substring(0, item.Length - 1);
                    table.Add(item);
                }
            }
            //table.Rows.RemoveAt(0);
            mListReviewDetail = table;
            PrintReview();
          
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            try
            {
                SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
                if (cell.CellValue == null)
                    return "";
                string value = cell.CellValue.InnerXml;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                return "";
            }
           
        }
        public static int linesprinted = 0;
        private static List<string> mListReviewDetail = new List<string>();

        private static List<string> mListPrint_Final = new List<string>();
        internal static void PrintReview()
        {
            linesprinted = 0;
            //mListPrint_Final.Insert(0, "==PAPER AUDIT TRAIL==");
            PrintDocument pd = new PrintDocument();
            Margins margins = new Margins(10, 10, 10, 10);
            pd.DefaultPageSettings.Margins = margins;
            ////document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // Print the document.
            pd.Print();
            AppManager.Instance.reviewprinted = true;
            //System.Threading.Thread oThread = new System.Threading.Thread(document.Print);
            //oThread.Start();
            //pThread.Join();
        }

        private static void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linesPerPage = 0;

            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float fontheight = 0;
            String line = null;
            float yPos = topMargin;

            System.Drawing.Font printFont = new System.Drawing.Font("Arial", AppManager.Instance.reviewResultFontSize - 8);

            List<string> mListPrint_n = new List<string>();
            int inx_in = -1;
            Dictionary<int, List<string>> dictPrint = new Dictionary<int, List<string>>();

            if (linesprinted > 0)
                mListReviewDetail = mListPrint_Final;
            else
            {
                List<string> printedFormatList = new List<string>();
                foreach (var item in mListReviewDetail)
                {
                    var lines = WriteText(e.Graphics, item, printFont);
                    foreach (var nline in lines)
                    {
                        printedFormatList.Add(nline);
                    }
                }
                mListReviewDetail = mListPrint_Final = printedFormatList;
            }

            for (int i = 0; i < mListReviewDetail.Count; i++)
            {

                mListPrint_n.Add(mListReviewDetail[i]);

                fontheight = printFont.GetHeight(e.Graphics);
                yPos += fontheight;
                // if last line position exceeds the visible area
                if (i + 1 < mListReviewDetail.Count)
                {
                    if (yPos + 10 + fontheight >= e.Graphics.VisibleClipBounds.Height)// && linesprinted < totallines)
                    {
                        yPos = topMargin;
                        inx_in++;
                        dictPrint.Add(inx_in, mListPrint_n);
                        mListPrint_n = new List<string>();
                    }
                }

            }
            if (mListPrint_n.Count > 0)
            {
                yPos = topMargin;
                inx_in++;
                dictPrint.Add(inx_in, mListPrint_n);
                mListPrint_n = new List<string>();
            }


            if (linesprinted > 0)
            {
                for (int d = 0; d < linesprinted; d++)
                {
                    dictPrint.Remove(dictPrint.Count - 1);
                }
            }

            mListReviewDetail = dictPrint[dictPrint.Count - 1];

            for (int i = 0; i < mListReviewDetail.Count; i++)
            {
                line = mListReviewDetail[i].Trim();

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


                e.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black, leftMargin, yPos, new System.Drawing.StringFormat());
                fontheight = printFont.GetHeight(e.Graphics);
                yPos += fontheight;

            }
            linesprinted += 1;
            if (inx_in + 1 > linesprinted)
            {
                e.HasMorePages = true;

            }
            else
            {
                e.HasMorePages = false;
                mListPrint_Final = new List<string>();

            }


        }

        public static List<string> WriteText(Graphics gr, string text, System.Drawing.Font font)
        {
            text = text.Replace("\n", " / ");
            string[] tmpText;
            List<string> nlist = new List<string>();
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

                        nlist.Add(line);
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        nlist.Add(line);
                        //top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {


                nlist.Add(text);
            }

            return nlist;
        }

        private System.Drawing.Font printFont;
        private StreamReader streamToPrint;
        static string filePath;
        // The PrintPage event is raised for each page to be printed.

        public void printGem(string file)
        {
            try
            {
                ExcelFile workbook = ExcelFile.Load(file);
                if (workbook.Worksheets.Count == 0)
                    return;
                int totalpg = 0;
                int st = 0;
                int en = 0;
                // Set sheets print options.
                en = workbook.GetPaginator().Pages.Count;
                //foreach (ExcelWorksheet worksheet in workbook.Worksheets)
                //{
                ExcelWorksheet worksheet = workbook.Worksheets[0];
                ExcelPrintOptions sheetPrintOptions = worksheet.PrintOptions;

                sheetPrintOptions.Portrait = true;
                sheetPrintOptions.HorizontalCentered = false;
                sheetPrintOptions.VerticalCentered = false;
                sheetPrintOptions.BottomMargin = 0;
                sheetPrintOptions.TopMargin = 0;
                sheetPrintOptions.RightMargin = 0;
                sheetPrintOptions.LeftMargin = 0.07;
                sheetPrintOptions.PrintHeadings = false;
                sheetPrintOptions.PrintGridlines = true;
                //sheetPrintOptions.PaperType = PaperType.Custom;
                sheetPrintOptions.FitWorksheetWidthToPages = 1;

                // Create spreadsheet's print options. 
                GemBox.Spreadsheet.PrintOptions printOptions = new GemBox.Spreadsheet.PrintOptions();
                printOptions.SelectionType = SelectionType.EntireFile;
            pageprint:
                if (en > 0)
                {
                    printOptions.FromPage = en - 1;
                    printOptions.ToPage = en - 1;
                    // printOptions.MetafileScaleFactor = 0.83;

                    // Print Excel workbook to default printer (e.g. 'Microsoft Print to Pdf').
                    string printerName = null;

                    workbook.Print(printerName, printOptions);
                    if (en > 0)
                    {
                        en--;
                        goto pageprint;
                    }
                }
            }
            catch (Exception ex)
            {
                ReadExcel(file);
            }
        }

    }

    public class RecordReportFiles
    {
        public int SN { get; set; }
        public string RecordSession { get; set; }
        public DateTime RecordTime { get; set; }
    }
}
