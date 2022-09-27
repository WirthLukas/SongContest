using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SongContest.Core.Contracts;
using SongContest.Mvvm.Persistence;

namespace SongContest.Mvvm.Common
{
    public class ValidateEntityViewModel<TEntity> : BaseViewModel, IValidatableObject, INotifyDataErrorInfo
        where TEntity : IEntityObject, new()
    {
        protected TEntity Entity;
        private bool _hasChanges;
        private string _lastChangedProperty;
        private readonly string[] _validatablePropertyNames;

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (_hasChanges == value) return;
                _hasChanges = value;
                NotifyAllCommands();
            }
        }

        public ValidateEntityViewModel(Func<IMvvmUnitOfWork> unitOfWorkCreator) : base(unitOfWorkCreator)
        {
            _validatablePropertyNames = typeof(TEntity).GetProperties()
                    .Select(pi => pi.Name)
                    .Concat(
                        GetType().GetProperties()
                            .Where(pi => pi.GetCustomAttribute<ValidateAttribute>() is not null)
                            .Select(pi => pi.Name)
                    )
                    .ToArray();
        }

        /// <summary>
        /// Meldet der View, dass das gegebene Property geändert wurde und validiert dieses
        /// sofern dessen Namen gleich dem Namen eines Properties des Entities ist oder es
        /// mit <see cref="ValidateAttribute"/> markiert wurde
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _lastChangedProperty = propertyName;
            HasChanges = UnitOfWork.ContextHasChanges;

            if (_validatablePropertyNames.Contains(_lastChangedProperty))
            { // wenn Änderung an Entity vorhanden ist ==> Validieren
                Validate();
            }

            base.OnPropertyChanged(propertyName);
        }

        protected virtual async void SaveAsync()
        {
            try
            {
                await UnitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                SetErrors(ex);
            }
        }

        protected void SetErrors(ValidationException ex)
        {
            HasErrors = true;

            if (ex.Value is IEnumerable<string> memberNames && memberNames.Count() == 1)  // gibt es genau ein betroffenes Property ==> zuordnen
            {
                foreach (var propertyName in memberNames)
                {
                    if (_validatablePropertyNames.Contains(propertyName))
                    {
                        Errors.Add(propertyName, new List<string> { ex.Message });
                    }
                    else
                    {
                        ModelError = ex.Message;
                    }
                    
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
            else  // kein oder mehrere Properties ==> ModelError als Validationsummary
            {
                ModelError = ex.ValidationResult.ToString();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(ModelError)));
            }
        }

        #region <<NotifyDataError Validation>>

        private bool _hasErrors;
        private string _modelError = "";
        protected readonly Dictionary<string, List<string>> Errors = new (); // Verwaltung der Fehlermeldungen für die Properties

        public bool HasErrors
        {
            get => _hasErrors;
            set { _hasErrors = value; NotifyAllCommands(); }
        }
        
        public string ModelError
        {
            get => _modelError;
            set { _modelError = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Fehlermeldungen für das Property zrückgeben
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Fehlermeldungen für das Property</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return propertyName != null && Errors.ContainsKey(propertyName)
                ? Errors[propertyName]
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Fehlerliste löschen und Properties verständigen
        /// </summary>
        protected void ClearErrors()
        {
            ModelError = "";
            foreach (var propertyName in Errors.Keys.ToList())
            {   // Errors löschen und Notification
                Errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Haben sich die Fehlermeldungen verändert?
        /// Verständigung des UI
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region <<Validation>>

        /// <summary>
        /// Aufruf der Validierung aller Properties, wenn sich Property geändert hat
        /// </summary>
        protected void Validate()
        {
            ClearErrors(); // alte Fehlermeldungen löschen
            if (Entity == null) return;

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(Entity, new ValidationContext(Entity),
                validationResults, validateAllProperties: true);

            // dann zusätzliche Validierungen aus dem ViewModel aufrufen
            Validator.TryValidateObject(this, new ValidationContext(this),
                validationResults, validateAllProperties: true);

            if (validationResults.Any())
            {
                var propertyNames = validationResults
                    .SelectMany(validationResult => validationResult.MemberNames)
                    .Distinct()
                    .ToList();

                foreach (var propertyName in propertyNames)
                {
                    Errors[propertyName] = validationResults
                        .Where(validationResult => validationResult.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct() // gleiche Fehlermeldungen unterdrücken
                        .ToList();

                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }

            HasErrors = Errors.Any() || !string.IsNullOrEmpty(ModelError);
        }

        /// <summary>
        /// Validierung der ViewModel-Properties über IValidatableObject
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            => Enumerable.Empty<ValidationResult>();

        #endregion
    }
}
