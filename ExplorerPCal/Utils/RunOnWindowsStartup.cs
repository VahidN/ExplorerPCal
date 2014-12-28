using Microsoft.Win32;
using System.Windows.Forms;

namespace ExplorerPCal.Utils
{
    public static class RunOnWindowsStartup
    {
        public static void Do()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.SetValue("ExplorerPCal", Application.ExecutablePath);
            }
        }

        public static void Undo()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.DeleteValue("ExplorerPCal", false);
            }
        }
    }
}