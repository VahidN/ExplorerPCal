using System;
using System.Text;
using System.Threading;
using EasyHook;
using ExplorerPCal.Hooks.Interceptors;
using ExplorerPCal.Hooks.Utils;
using ExplorerPCal.HookingInterface;

namespace ExplorerPCal.Hooks
{
    public class GetDateTimeFormatInjection : IEntryPoint
    {
        MessagesReceiverInterface _interface;
        private LocalHook _kernel32GetDateFormatWHook;
        private LocalHook _apiGetDateFormatWHook;
        private LocalHook _apiGetDateFormatExHook;

        private LocalHook _kernel32GetTimeFormatWHook;
        private LocalHook _apiGetTimeFormatWHook;
        private LocalHook _apiGetTimeFormatExHook;

        public GetDateTimeFormatInjection(RemoteHooking.IContext context, string channelName)
        {
            // connect to host...
            _interface = RemoteHooking.IpcConnectClient<MessagesReceiverInterface>(channelName);
            _interface.Ping();
        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            try
            {
                registerGetDateFormatHook();
                registerGetTimeFormatHook();
            }
            catch (Exception e)
            {
                _interface.ReportException(e);
                return;
            }

            RemoteHooking.WakeUpProcess();
            _interface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            NativeMethods.RefreshWindowsSettings();

            keepItInMemory();
            freeResources();

            NativeMethods.RefreshWindowsSettings();
        }

        private void registerGetTimeFormatHook()
        {
            _kernel32GetTimeFormatWHook = this.RegisterHook("kernel32.dll", "GetTimeFormatW",
                new DateTimeDelegates.GetTimeFormatWDelegate(getTimeFormatWInterceptor));

            // For Windows 8.x
            if (!WindowsVersion.IsWindows8Plus)
                return;

            _apiGetTimeFormatWHook = this.RegisterHook(
                "api-ms-win-core-datetime-l1-1-1.dll", "GetTimeFormatW",
                new DateTimeDelegates.GetTimeFormatWDelegate(getTimeFormatWInterceptor));
            _apiGetTimeFormatExHook = this.RegisterHook(
                "api-ms-win-core-datetime-l1-1-1.dll", "GetTimeFormatEx",
                new DateTimeDelegates.GetTimeFormatExDelegate(getTimeFormatExInterceptor));
        }

        private void registerGetDateFormatHook()
        {
            _kernel32GetDateFormatWHook = this.RegisterHook("kernel32.dll", "GetDateFormatW",
                new DateTimeDelegates.GetDateFormatWDelegate(getDateFormatWInterceptor));

            // For Windows 8.x
            if (!WindowsVersion.IsWindows8Plus)
                return;

            _apiGetDateFormatWHook = this.RegisterHook(
                "api-ms-win-core-datetime-l1-1-1.dll", "GetDateFormatW",
                new DateTimeDelegates.GetDateFormatWDelegate(getDateFormatWInterceptor));
            _apiGetDateFormatExHook = this.RegisterHook(
                "api-ms-win-core-datetime-l1-1-1.dll", "GetDateFormatEx",
                new DateTimeDelegates.GetDateFormatExDelegate(getDateFormatExInterceptor));
        }

        private void keepItInMemory()
        {
            // wait for host process termination...
            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                    _interface.Ping();
                }
            }
            catch
            {
                _interface = null;
                // .NET Remoting will raise an exception if host is unreachable
            }
        }

        private void freeResources()
        {
            _kernel32GetDateFormatWHook.ForceDispose();
            _kernel32GetTimeFormatWHook.ForceDispose();
            _apiGetDateFormatWHook.ForceDispose();
            _apiGetTimeFormatWHook.ForceDispose();
            _apiGetDateFormatExHook.ForceDispose();
            _apiGetTimeFormatExHook.ForceDispose();
        }

        private int getTimeFormatWInterceptor(uint locale,
                                        uint dwFlags,
                                        SystemTime lpTime,
                                        string lpFormat,
                                        StringBuilder lpTimeStr,
                                        int sbSize)
        {
            try
            {
                if (_interface != null)
                    _interface.Converted();

                return TimeFormatInterceptor.ModifyTimeFormat(locale, dwFlags, lpTime, lpFormat, lpTimeStr, sbSize);
            }
            catch (Exception ex)
            {
                if (_interface != null) _interface.ReportException(ex);
                if (_interface != null && lpTime != null) _interface.SendMessage(string.Format("GetTimeFormatW ->locale:{0}, dwFlags:{1}, lpFormat:{2}, sbSize:{3}, lpDate.Year:{4}, lpDate.Month:{5}, lpDate.Day:{6} lpDate.Minute:{7}, lpDateStr:{8}, CurrentProcessId:{9}, CurrentThreadId:{10} ",
                                                                     locale, dwFlags, lpFormat, sbSize, lpTime.Year, lpTime.Month,
                                                                     lpTime.Day, lpTime.Minute, lpTimeStr, RemoteHooking.GetCurrentProcessId(), RemoteHooking.GetCurrentThreadId()));
                // call the original API...
                return NativeMethods.GetTimeFormatW(locale, dwFlags, lpTime, lpFormat, lpTimeStr, sbSize);
            }
        }

        private int getTimeFormatExInterceptor(
            string lpLocaleName,
            uint dwFlags,
            SystemTime lpTime,
            string lpFormat,
            StringBuilder lpTimeStr,
            int sbSize)
        {
            return getTimeFormatWInterceptor(0, dwFlags, lpTime, lpFormat, lpTimeStr, sbSize);
        }

        private int getDateFormatWInterceptor(
                                        uint locale,
                                        uint dwFlags,
                                        SystemTime lpDate,
                                        string lpFormat,
                                        StringBuilder lpDateStr,
                                        int sbSize)
        {
            try
            {
                if (_interface != null)
                    _interface.Converted();

                return DateFormatInterceptor.ModifyDateFormat(locale, dwFlags, lpDate, lpFormat, lpDateStr, sbSize);
            }
            catch (Exception ex)
            {
                if (_interface != null) _interface.ReportException(ex);
                if (_interface != null && lpDate != null) _interface.SendMessage(string.Format("GetDateFormatW ->locale:{0}, dwFlags:{1}, lpFormat:{2}, sbSize:{3}, lpDate.Year:{4}, lpDate.Month:{5}, lpDate.Day:{6} lpDate.Minute:{7}, lpDateStr:{8}, CurrentProcessId:{9}, CurrentThreadId:{10}",
                                                                       locale, dwFlags, lpFormat, sbSize, lpDate.Year, lpDate.Month,
                                                                       lpDate.Day, lpDate.Minute, lpDateStr, RemoteHooking.GetCurrentProcessId(), RemoteHooking.GetCurrentThreadId()));
                // call the original API...
                return NativeMethods.GetDateFormatW(locale, dwFlags, lpDate, lpFormat, lpDateStr, sbSize);
            }
        }

        private int getDateFormatExInterceptor(
            string lpLocaleName,
            uint dwFlags,
            SystemTime lpDate,
            string lpFormat,
            StringBuilder lpDateStr,
            int sbSize,
            string lpCalendar)
        {
            return getDateFormatWInterceptor(0, dwFlags, lpDate, lpFormat, lpDateStr, sbSize);
        }
    }
}