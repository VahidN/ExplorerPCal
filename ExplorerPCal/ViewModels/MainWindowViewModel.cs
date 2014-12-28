using System.Windows;
using ExplorerPCal.Models;
using System.ComponentModel;
using ExplorerPCal.Utils;
using ExplorerPCal.HookingInterface;

namespace ExplorerPCal.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowGui GuiData { set; get; }
        public MainWindowViewModel()
        {
            initData();
            if (Designer.IsInDesignModeStatic) return;
            loadSettings();
            manageAppExit();
        }

        private void initData()
        {
            GuiData = new MainWindowGui();
            GuiData.PropertyChanged += guiDataPropertyChanged;
            AppMessenger.Messenger.Register<Log>("AddLog", addLog);
            AppMessenger.Messenger.Register("Converted", converted);
        }

        void converted()
        {
            GuiData.NumberOfCallbacks++;
        }

        void addLog(Log log)
        {
            GuiData.LogsData.Add(log);
        }

        private static void manageAppExit()
        {
            Application.Current.Exit += currentExit;
            Application.Current.SessionEnding += currentSessionEnding;
        }

        static void currentExit(object sender, ExitEventArgs e)
        {
            HooksManager.UnloadChannel();
        }

        static void currentSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            HooksManager.UnloadChannel();
        }

        void guiDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RunOnStartup")
            {
                saveSettings();
            }
        }

        private void saveSettings()
        {
            if (GuiData.RunOnStartup)
                RunOnWindowsStartup.Do();
            else
                RunOnWindowsStartup.Undo();

            ConfigSetGet.SetConfigData("RunOnStartup", GuiData.RunOnStartup.ToString());
        }

        private void loadSettings()
        {
            GuiData.RunOnStartup = bool.Parse(ConfigSetGet.GetConfigData("RunOnStartup"));
        }
    }
}