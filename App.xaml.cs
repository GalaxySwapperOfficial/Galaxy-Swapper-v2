using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Views;
using Serilog;
using System.Windows;

namespace Galaxy_Swapper_v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Config.Initialize();
            Output.Initialize();
            Settings.Initialize();
            SwapLogs.Initialize();
            SwapData.Initialize();
            UEFN.Initialize();
            DLLS.Initialize();

            Log.Information("Successfully initialized properties");
            Log.Information("This version is open source! If you have been sent this file be carful of any code modifcation that could have been done. https://galaxyswapperv2.com/Guilded");

            new MainView().ShowDialog();

            Log.Information("Shutting down..");
            Application.Current.Shutdown();
        }
    }
}