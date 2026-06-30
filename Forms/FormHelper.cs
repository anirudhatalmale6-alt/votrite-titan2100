using System;
using System.Runtime.InteropServices;

namespace VotRite.Forms
{
    internal class FormHelper
    {
        // Fields
        private static IntPtr HWND_NOTOPMOST = new IntPtr(1);
        private static IntPtr HWND_TOP = IntPtr.Zero;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        private const short SWP_NOSIZE = 1;
        private const short SWP_NOZORDER = 4;
        private const int SWP_SHOWWINDOW = 0x40;

        // Methods
        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowW([In, MarshalAs(UnmanagedType.LPTStr)] string lpClassName,
                                                [In, MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int which);

        public static void HideTray()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", ""), 0);
        }

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int width, int height,
                                               uint flags);

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, ScreenX, ScreenY, 0x40);
        }

        public static void ShowScreenKeyboard()
        {
            SetWindowPos(FindWindowW("OSKMainClass", null), HWND_TOP, 0, 0, ScreenX, ScreenY - 650, 4);
        }

        public static void showOnScreenKeyboard()
        {
            System.Diagnostics.Process.Start("osk.exe");
        }

        public static void ShowTray()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", ""), 1);
        }

        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int command);

        // Properties
        public static int ScreenX
        {
            get { return GetSystemMetrics(0); }
        }

        public static int ScreenY
        {
            get { return GetSystemMetrics(1); }
        }
    }
}
