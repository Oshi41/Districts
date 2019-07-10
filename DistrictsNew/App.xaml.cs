using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using DistrictsLib.Implementation;
using DistrictsNew.ViewModel;

namespace DistrictsNew
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += TraceUnhandledEx;
        }

        private void TraceUnhandledEx(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Trace.WriteLine($"!!! UNHANDLED !!!\n{e.Exception}");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = DistrictsNew.Properties.Settings.Default;

            Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(settings.LogFolder, "log.txt")));

            Trace.WriteLine("\n\n\n************************\nApplication is started!");

            var loader = new LoadingManager(
                settings.BuildingFolder,
                settings.HomeInfoFolder,
                settings.RestrictionsFolder,
                settings.CardsFolder,
                settings.ManageFolder);

            var window = new MainWindow
            {
                DataContext = new MainViewModel(loader, loader)
            };

            window.ShowDialog();

            App.Current.Shutdown(0);
        }
    }
}
