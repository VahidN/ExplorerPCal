using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using EasyHook;
using ExplorerPCal.Models;
using ExplorerPCal.Utils;

namespace ExplorerPCal.HookingInterface
{
    public static class HooksManager
    {
        private static IpcServerChannel _channel;
        private static string _channelName;
        private static readonly IList<int> _alreadyInjectedProcesses = new List<int>();
        private static readonly object _lockObject = new object();
        private static readonly string[] _processesList = { "explorer" };

        public static void StartChannel()
        {
            try
            {
                _channel = runGetTimeDateFormatHook();
                processMonitorStart();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogExceptionToFile(ex);
                AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = string.Format("خطا:{0}", ex) });
            }
        }

        /// <summary>
        /// روی ویندوز 7 می‌توان هر پنجره جدید اکسپلورر را در یک پروسه جدید اجرا کرد
        /// </summary>
        private static void processMonitorStart()
        {
            LightProcessMonitor.Start(names: _processesList);
            LightProcessMonitor.OnProcessCallback = explorers =>
            {
                if (explorers == null || !explorers.Any())
                {
                    AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = "explorer در حال اجرا نيست." });
                    return;
                }

                try
                {
                    injectHooks(explorers);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogExceptionToFile(ex);
                    AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = string.Format("خطا:{0}", ex) });
                }
            };
        }

        public static void UnloadChannel()
        {
            if (_channel == null)
                return;

            try
            {
                // cleanup library...
                _channel.StopListening(null);
                _channel = null;

                LightProcessMonitor.Stop();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogExceptionToFile(ex);
                AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = string.Format("خطا:{0}", ex) });
            }
        }

        private static IpcServerChannel runGetTimeDateFormatHook()
        {
            var explorers = LightProcessMonitor.FindProcesses(names: _processesList);
            if (explorers == null || !explorers.Any())
            {
                AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = "explorer در حال اجرا نيست." });
                return null;
            }

            var channel = RemoteHooking.IpcCreateServer<MessagesReceiverInterface>(ref _channelName, WellKnownObjectMode.SingleCall);
            injectHooks(explorers);
            return channel;
        }

        [HandleProcessCorruptedStateExceptions]
        private static void injectHooks(IEnumerable<Process> explorers)
        {
            lock (_lockObject)
            {
                foreach (var explorer in explorers)
                {
                    if (_alreadyInjectedProcesses.Contains(explorer.Id))
                        continue;

                    try
                    {
                        RemoteHooking.Inject(
                            explorer.Id,
                            InjectionOptions.Default | InjectionOptions.DoNotRequireStrongName,
                            getHookFilePath(), // 32-bit version (the same, because of using AnyCPU)
                            getHookFilePath(), // 64-bit version (the same, because of using AnyCPU)
                            _channelName
                            );

                        _alreadyInjectedProcesses.Add(explorer.Id);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogExceptionToFile(ex);
                        AppMessenger.Messenger.NotifyColleagues("AddLog", new Log { Text = string.Format("خطا:{0}", ex) });
                    }
                }
            }
        }

        private static string getHookFilePath()
        {
            var hookFilePath = Path.Combine(AppMessenger.Path, "ExplorerPCal.Hooks.dll");
            if (!File.Exists(hookFilePath))
                throw new FileNotFoundException(string.Format("{0} not found.", hookFilePath));

            return hookFilePath;
        }
    }
}