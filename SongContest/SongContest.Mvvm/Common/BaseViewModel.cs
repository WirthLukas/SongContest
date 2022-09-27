using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using SongContest.Mvvm.Persistence;

namespace SongContest.Mvvm.Common
{
    public abstract class BaseViewModel : NotifyPropertyChanged
    {
        protected IMvvmUnitOfWork UnitOfWork { get; }

        protected BaseViewModel(Func<IMvvmUnitOfWork> unitOfWorkCreator)
        {
            UnitOfWork = unitOfWorkCreator();
        }

        /// <summary>
        /// Aktiviert das PropertyChanged Event für alle Properties der Instanz, die
        /// über kein <see cref="NoNotifyAttribute"/> verfügen 
        /// </summary>
        protected void NotifyAllProperties()
        {
            var propertyNames = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.GetCustomAttribute<NoNotifyAttribute>() is null)
                .Select(pi => pi.Name);

            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Die Übergebene Funktion wird für jeden Command des ViewModels ausgeführt
        /// </summary>
        /// <param name="commandHandler"></param>
        protected void NotifyAllCommands()
        {
            var propertyInfos = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.PropertyType.GetInterfaces().Contains(typeof(ICommand)))
                .ToArray();

            foreach (var pi in propertyInfos)
            {
                var command = (DelegateCommand)pi.GetValue(this);
                command?.RaiseCanExecuteChanged();
            }
        }

        public void DisposeUnitOfWork() => UnitOfWork.Dispose();
        public ValueTask DisposeUnitOfWorkAsync() => UnitOfWork.DisposeAsync();

        public virtual Task LoadDataAsync() => Task.CompletedTask;
    }
}
