
namespace ExplorerPCal.Utils
{
    public class AppMessenger    
    {
        readonly static Messenger _messenger = new Messenger();

        public static Messenger Messenger
        {
            get { return _messenger; }
        }

        public static string Path
        {
            get { return System.Windows.Forms.Application.StartupPath; }
        }
    }
}