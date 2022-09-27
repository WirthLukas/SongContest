using System.Windows;
using Autofac;
using SongContest.Mvvm.Views;

namespace SongContest.Mvvm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var container = Mvvm.Startup.ConfigureServices();
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
