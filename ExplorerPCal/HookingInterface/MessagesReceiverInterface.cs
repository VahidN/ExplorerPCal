using System;
using ExplorerPCal.Models;
using ExplorerPCal.Utils;

namespace ExplorerPCal.HookingInterface
{
    public class MessagesReceiverInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 inClientPid)
        {
            AppMessenger.Messenger.NotifyColleagues("AddLog",
                new Log { Text = string.Format("شمسی ساز تاریخ اکسپلورر ویندوز در پروسه {0} نصب شد.", inClientPid) });
        }

        public void OnCreateFile(Int32 inClientPid, String[] inFileNames)
        {
        }

        public void ReportException(Exception inInfo)
        {
            ExceptionLogger.LogExceptionToFile(inInfo);
            AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = string.Format("خطا:{0}", inInfo) });
        }

        public void SendMessage(string message)
        {
            AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = message });
        }

        public void Converted()
        {
            AppMessenger.Messenger.NotifyColleagues("Converted");
        }

        public void Ping()
        {
        }
    }
}