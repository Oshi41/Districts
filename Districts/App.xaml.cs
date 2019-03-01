using System.Windows;
using System.Windows.Threading;

namespace Districts
{
    /// <summary>
    ///     Логика взаимодействия для App.xaml
    /// </summary>
    public class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnError;
        }

        private void OnError(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Tracer.Tracer.WriteError(e.Exception);
        }
    }
}