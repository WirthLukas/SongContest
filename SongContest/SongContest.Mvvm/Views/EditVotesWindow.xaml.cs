using MahApps.Metro.Controls;
using SongContest.Mvvm.ViewModels;

namespace SongContest.Mvvm.Views
{
    /// <summary>
    /// Interaction logic for EditVotesWindow.xaml
    /// </summary>
    public partial class EditVotesWindow : MetroWindow
    {
        public EditVotesWindow() => InitializeComponent();

        public EditVotesWindow(EditVotesWindowViewModel viewModel, int countryId) : this()
        {
            DataContext = viewModel;
            Loaded += async (_, _) => await viewModel.LoadDataAsync(countryId);
            Closed += async (_, _) => await viewModel.DisposeUnitOfWorkAsync();
        }
    }
}
