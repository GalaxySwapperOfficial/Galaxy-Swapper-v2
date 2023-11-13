using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        public static void SetProcessHigher(Process[] process)
        {
            foreach (var proc in process)
            {
                SetForegroundWindow(proc.MainWindowHandle);
            }
        }
    }
}