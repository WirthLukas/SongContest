using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SongContest.Core.Entities;

namespace SongContest.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<ParticipantComposer> ParticipantComposers { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public ApplicationDbContext() {}
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Debug.Write(configuration.ToString());
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))

            optionsBuilder
                //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog()))
                .UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        ///// <summary>
        ///// NuGet: Microsoft.Extensions.Logging.Console
        ///// </summary>
        ///// <returns></returns>
        //private ILoggerFactory GetLoggerFactory()
        //{
        //    IServiceCollection serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(builder =>
        //        builder.AddConsole()
        //            .AddFilter(DbLoggerCategory.Database.Command.Name,
        //                LogLevel.Information));
        //    return serviceCollection.BuildServiceProvider()
        //        .GetService<ILoggerFactory>();
        //}
    }
}
