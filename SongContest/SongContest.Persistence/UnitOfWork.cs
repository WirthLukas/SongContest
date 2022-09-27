using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Persistence.Repositories;

namespace SongContest.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext DbContext;
        private bool _disposed;

        public IVoteRepository Votes { get; }
        public IComposerRepository Composers { get; }
        public ICountryRepository Countries { get; }
        public IParticipantRepository Participants { get; }

        public UnitOfWork()
        {
            DbContext = new ApplicationDbContext();
            Votes = new VoteRepository(DbContext);
            Composers = new ComposersRepository(DbContext);
            Countries = new CountryRepository(DbContext);
            Participants = new ParticipantRepository(DbContext);
        }

        public Task DeleteDatabaseAsync() => DbContext.Database.EnsureDeletedAsync();

        public Task MigrateDatabaseAsync() => DbContext.Database.MigrateAsync();

        public Task CreateDatabaseAsync() => DbContext.Database.EnsureCreatedAsync();

        public Task<int> SaveChangesAsync()
        {
            var entities = DbContext.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln

            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity, null, null);

                if (entity is IDatabaseValidatableObject) // UnitOfWork injizieren, wenn Interface implementiert ist
                {
                    validationContext.InitializeServiceProvider(_ => this);
                }

                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(entity, validationContext, validationResults,
                    validateAllProperties: true);

                if (!isValid)
                {
                    var memberNames = new List<string>();
                    var validationExceptions = new List<ValidationException>();

                    foreach (ValidationResult validationResult in validationResults)
                    {
                        validationExceptions.Add(new ValidationException(validationResult, null,
                                validationResult.MemberNames));
                        memberNames.AddRange(validationResult.MemberNames);
                    }

                    if (validationExceptions.Count == 1)  // eine Validationexception werfen
                    {
                        throw validationExceptions.Single();
                    }

                    // AggregateException mit allen ValidationExceptions als InnerExceptions werfen
                    throw new ValidationException($"Entity validation failed for {string.Join(", ", memberNames)}",
                        new AggregateException(validationExceptions));
                }
            }

            return  DbContext.SaveChangesAsync();
        }

        #region <<IDisposable>>

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                DbContext.Dispose();
            }

            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed && disposing)
            {
                await DbContext.DisposeAsync();
            }

            _disposed = true;
        }

        #endregion
    }
}
