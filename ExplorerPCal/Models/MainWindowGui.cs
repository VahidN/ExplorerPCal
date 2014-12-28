using System.ComponentModel;

namespace ExplorerPCal.Models
{
    public class MainWindowGui : INotifyPropertyChanged
    {
        long _numberOfCallbacks;
        public long NumberOfCallbacks 
        {
            set
            {
                _numberOfCallbacks = value;
                notifyPropertyChanged("NumberOfCallbacks");
            }
            get { return _numberOfCallbacks; }
        }

        bool _runOnStartup;
        public bool RunOnStartup 
        {
            set
            {
                _runOnStartup = value;
                notifyPropertyChanged("RunOnStartup");
            }
            get { return _runOnStartup; } 
        }

        Logs _logsData = new Logs();
        public Logs LogsData 
        {
            set
            {
                _logsData = value;
                notifyPropertyChanged("LogsData");
            }
            get { return _logsData; } 
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}