using System;
using System.ComponentModel;

namespace ExplorerPCal.Models
{
    public class Log : INotifyPropertyChanged
    {
        #region Fields (2)

        DateTime _date;
        string _text;

        #endregion Fields

        #region Properties (2)

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                notifyPropertyChanged("Date");
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                notifyPropertyChanged("Text");
            }
        }

        #endregion Properties



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