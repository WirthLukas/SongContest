using MahApps.Metro.Controls;
using SongContest.Mvvm.ViewModels;

namespace SongContest.Mvvm.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow() => InitializeComponent();

        public MainWindow(MainWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += async (_, _) => await viewModel.LoadDataAsync();
            Closed += async (_, _) => await viewModel.DisposeUnitOfWorkAsync();
        }
    }
}
