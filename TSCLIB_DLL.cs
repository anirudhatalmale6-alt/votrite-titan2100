using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Runtime.InteropServices;
namespace VotRite
{
    public class TSCLIB_DLL
    {
        private readonly string FULL_CUT = "CUT";
        private readonly string PRINTER_NAME;

        private bool Is64BitProcess
        {
            get { return System.Environment.Is64BitProcess; }
        }

        public TSCLIB_DLL(string printerName)
        {
            PRINTER_NAME = printerName;
        }

        public void CutPaper()
        {
            openport(PRINTER_NAME);
            sendcommand(FULL_CUT);
            closeport();
        }

        private void about()
        {
            if (Is64BitProcess)
            {
                about64();
            }
            else
            {
                about86();
            }
        }

        private void openport(string printername)
        {
            if (Is64BitProcess)
            {
                openport64(printername);
            }
            else
            {
                openport86(printername);
            }
        }

        private void closeport()
        {
            if (Is64BitProcess)
            {
                closeport64();
            }
            else
            {
                closeport86();
            }
        }

        private void sendcommand(string printercommand)
        {
            if (Is64BitProcess)
            {
                sendcommand64(printercommand);
            }
            else
            {
                sendcommand86(printercommand);
            }
        }

        // 86
        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "about")]
        public static extern int about86();

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "openport")]
        public static extern int openport86(string printername);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "barcode")]
        public static extern int barcode86(string x, string y, string type,
                    string height, string readable, string rotation,
                    string narrow, string wide, string code);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int clearbuffer86();

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int closeport86();

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "downloadpcx")]
        public static extern int downloadpcx86(string filename, string image_name);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "formfeed")]
        public static extern int formfeed86();

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "nobackfeed")]
        public static extern int nobackfeed86();

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "printerfont")]
        public static extern int printerfont86(string x, string y, string fonttype,
                        string rotation, string xmul, string ymul,
                        string text);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "printlabel")]
        public static extern int printlabel86(string set, string copy);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int sendcommand86(string printercommand);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "setup")]
        public static extern int setup86(string width, string height,
                  string speed, string density,
                  string sensor, string vertical,
                  string offset);

        [DllImport("\\x86\\TSCLIB.dll", EntryPoint = "windowsfont")]
        public static extern int windowsfont86(int x, int y, int fontheight,
                        int rotation, int fontstyle, int fontunderline,
                        string szFaceName, string content);

        // 64
        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "about")]
        public static extern int about64();

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "openport")]
        public static extern int openport64(string printername);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "barcode")]
        public static extern int barcode64(string x, string y, string type,
                    string height, string readable, string rotation,
                    string narrow, string wide, string code);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int clearbuffer64();

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int closeport64();

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "downloadpcx")]
        public static extern int downloadpcx64(string filename, string image_name);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "formfeed")]
        public static extern int formfeed64();

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "nobackfeed")]
        public static extern int nobackfeed64();

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "printerfont")]
        public static extern int printerfont64(string x, string y, string fonttype,
                        string rotation, string xmul, string ymul,
                        string text);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "printlabel")]
        public static extern int printlabel64(string set, string copy);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int sendcommand64(string printercommand);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "setup")]
        public static extern int setup64(string width, string height,
                  string speed, string density,
                  string sensor, string vertical,
                  string offset);

        [DllImport("\\x64\\TSCLIB.dll", EntryPoint = "windowsfont")]
        public static extern int windowsfont64(int x, int y, int fontheight,
                        int rotation, int fontstyle, int fontunderline,
                        string szFaceName, string content);

    }
}
