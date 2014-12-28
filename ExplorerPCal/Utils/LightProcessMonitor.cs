using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ThreadTimer = System.Threading.Timer;

namespace ExplorerPCal.Utils
{
    public static class LightProcessMonitor
    {
        private static ThreadTimer _threadTimer; //keep it alive    
        private static string[] _names;

        public static void Start(string[] names, long startAfter = 1 * 60 * 1000, long interval = 3000)
        {
            _names = names;
            _threadTimer = new ThreadTimer(timerCallback, null, Timeout.Infinite, 1000);
            _threadTimer.Change(startAfter, interval);
        }

        public static void Stop()
        {
            if (_threadTimer != null)
                _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static Action<IList<Process>> OnProcessCallback { set; get; }

        static void timerCallback(object state)
        {
            if (OnProcessCallback != null)
                OnProcessCallback(FindProcesses(_names));
        }

        public static IList<Process> FindProcesses(string[] names)
        {
            var processes = Process.GetProcesses();
            return (from process in processes
                    from name in names
                    where process.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select process).ToList();
        }
    }
}