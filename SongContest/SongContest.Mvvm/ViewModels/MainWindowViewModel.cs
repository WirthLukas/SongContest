using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using SongContest.Core.DataTransferObjects;
using SongContest.Mvvm.Common;
using SongContest.Mvvm.Persistence;
using SongContest.Mvvm.Views;

namespace SongContest.Mvvm.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly Func<int, EditVotesWindow> _editVotesWindowCreator;
        private IEnumerable<CountryStatisticsDto> _countries;
        private CountryStatisticsDto _selectedCountry;
        private string _countryNameFilter = "";
        private bool _onlyCountriesWithSongs;

        public IEnumerable<CountryStatisticsDto> Countries
        {
            get => _countries?
                .Where(c => c.Country.StartsWith(_countryNameFilter))
                .Where(cs => !_onlyCountriesWithSongs || cs.Points is not null);
            set { _countries = value; OnPropertyChanged(); }
        }

        public CountryStatisticsDto SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged();
                CommandEditVotes.RaiseCanExecuteChanged();
            }
        }

        public string CountryNameFilter
        {
            get => _countryNameFilter;
            set
            {
                if (value is not null && value != _countryNameFilter)
                {
                    _countryNameFilter = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Countries));
                }
            }
        }

        public bool OnlyCountriesWithSongs
        {
            get => _onlyCountriesWithSongs;
            set
            {
                if (_onlyCountriesWithSongs != value)
                {
                    _onlyCountriesWithSongs = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Countries));
                }
            }
        }

        [NoNotify] public DelegateCommand CommandEditVotes { get; }

        public MainWindowViewModel(
            Func<IMvvmUnitOfWork> unitOfWorkCreator,
            Func<int, EditVotesWindow> editVotesWindowCreator)
            : base(unitOfWorkCreator)
        {
            _editVotesWindowCreator = editVotesWindowCreator ?? throw new ArgumentNullException(nameof(editVotesWindowCreator));
            CommandEditVotes = new DelegateCommand(EditVotes, () => _selectedCountry is not null);
        }

        private async void EditVotes()
        {
            var editVotesWindow = _editVotesWindowCreator(_selectedCountry.Id);
            editVotesWindow.ShowDialog();
            await LoadDataAsync();
        }

        public override async Task LoadDataAsync()
        {
            UnitOfWork.ClearChangeTracker();
            Countries = await UnitOfWork.Countries.GetCountryStatisticsAsync();
        }
    }
}
