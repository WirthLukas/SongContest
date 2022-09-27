using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;
using SongContest.Core.Validations;
using SongContest.Mvvm.Common;
using SongContest.Mvvm.Persistence;

namespace SongContest.Mvvm.ViewModels
{
    public class EditVotesWindowViewModel : ModifyingViewModel<int, Country>
    {
        private Country _selectedCountry;
        private int _points;
        private IEnumerable<Country> _countries;
        private IEnumerable<CountryVotesDto> _votes;
        private CountryVotesDto _selectedVote;

        public string Title => $"Bewertungen abgeben von: {Entity?.Name}";

        [CorrectPointValue]
        [Validate]
        public int Points
        {
            get => _points;
            set { _points = value; OnPropertyChanged(); }
        }

        [Required]
        [Validate]
        public Country SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value; OnPropertyChanged();
                // Command Notification wird vom ValidateVieModel beim Vlaidieren übernommen
            }
        }

        public IEnumerable<Country> Countries
        {
            get => _countries;
            set { _countries = value; OnPropertyChanged(); }
        }

        public IEnumerable<CountryVotesDto> Votes
        {
            get => _votes;
            set { _votes = value; OnPropertyChanged(); }
        }

        public CountryVotesDto SelectedVote
        {
            get => _selectedVote;
            set
            {
                _selectedVote = value;
                OnPropertyChanged();
                CommandRemoveVoting.RaiseCanExecuteChanged();
            }
        }

        [NoNotify] public DelegateCommand CommandAddVoting { get; }
        [NoNotify] public DelegateCommand CommandRemoveVoting { get; }
        [NoNotify] public DelegateCommand CommandFillVotes { get; }

        public EditVotesWindowViewModel(Func<IMvvmUnitOfWork> unitOfWorkCreator) : base(unitOfWorkCreator)
        {
            CommandAddVoting = new DelegateCommand(AddVoting, () => SelectedCountry is not null && !HasErrors);
            CommandRemoveVoting = new DelegateCommand(RemoveVoting, () => SelectedVote is not null);
            CommandFillVotes = new DelegateCommand(FillVotes);
        }

        private async void AddVoting()
        {
            var vote = new Vote
            {
                Country = Entity,
                Participant = await UnitOfWork.Participants.GetParticipantByCountry(_selectedCountry.Id),
                Points = Points
            };

            await UnitOfWork.Votes.AddAsync(vote);

            try
            {
                await UnitOfWork.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (ValidationException ex)
            {
                UnitOfWork.Votes.Remove(vote);
                SetErrors(ex);
            }
        }

        private async void RemoveVoting()
        {
            UnitOfWork.Votes.Remove(SelectedVote.VoteId);

            try
            {
                await UnitOfWork.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (ValidationException ex)
            {
                SetErrors(ex);
            }
        }

        private async void FillVotes()
        {
            UnitOfWork.ClearChangeTracker();
            var participants = await UnitOfWork.Participants
                .GetNonVotedParticipantsForCountry(Entity.Id);

            var newVotes = participants.Select(p => new Vote
            {
                CountryId = Entity.Id,      // Country = ENtity geht irgendwie ned wegen: SqlException: Cannot insert explicit value for identity column in table 'Countries' when IDENTITY_INSERT is set to OFF
                Participant = p,
                Points = 0
            });

            await UnitOfWork.Votes.AddRangeAsync(newVotes);

            try
            {
                await UnitOfWork.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (ValidationException ex)
            {
                SetErrors(ex);
            }
        }

        public override async Task LoadDataAsync(int countryId)
        {
            Entity = await UnitOfWork.Countries.GetByIdAsync(countryId);
            _countries = await UnitOfWork.Countries.GetVotableCountries();
            await LoadDataAsync();
        }

        public override async Task LoadDataAsync()
        {
            _votes = await UnitOfWork.Votes.GetVotesOfCountryAsync(Entity.Id);
            NotifyAllProperties();
        }
    }
}
