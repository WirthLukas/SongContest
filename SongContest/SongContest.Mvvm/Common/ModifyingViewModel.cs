using System;
using System.Threading.Tasks;
using SongContest.Core.Contracts;
using SongContest.Mvvm.Persistence;

namespace SongContest.Mvvm.Common
{
    public class ModifyingViewModel<TParameter, TEntity> : ValidateEntityViewModel<TEntity>
        where TEntity : IEntityObject, new()
    {
        public ModifyingViewModel(Func<IMvvmUnitOfWork> unitOfWorkCreator) : base(unitOfWorkCreator)
        {
        }

        public virtual Task LoadDataAsync(TParameter parameter) => Task.CompletedTask;
    }
}
