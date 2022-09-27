using System;
using System.Linq;
using System.Threading.Tasks;
using SongContest.Core.Contracts;
using SongContest.Core.Entities;
using SongContest.Persistence;
using Utils;

namespace SongContest.ImportConsole
{
    internal class Program
    {
        private const string ParticipantsFileName = "Participants.csv";
        private const string VotesFileName = "TeleVotes.csv";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Import der SongContest Daten in die Datenbank");
            Console.WriteLine("=============================================\n");
            await using IUnitOfWork unitOfWork = new UnitOfWork();

            Console.WriteLine("Datenbank löschen");
            await unitOfWork.DeleteDatabaseAsync();
            Console.WriteLine("Datenbank migrieren");
            await unitOfWork.MigrateDatabaseAsync();

            Console.WriteLine($"Einlesen der Daten aus {ParticipantsFileName}");
            var (composers, votes) = await ImportAsync(ParticipantsFileName, VotesFileName);
            await unitOfWork.Composers.AddRangeAsync(composers);
            await unitOfWork.Votes.AddRangeAsync(votes);

            int numberOfCountries = votes
                .Select(v => v.Country)
                .GroupBy(c => c.Name)
                .Count();

            int numberOfParticipants = votes
                .Select(v => v.Participant)
                .GroupBy(p => p.ActorName)
                .Count();

            int numberOfVotes = votes.Length;
            int numberOfComposers = composers.Length;

            Console.WriteLine($"\n> Importiere {numberOfCountries} Länder!");
            Console.WriteLine($"> Importiere {numberOfParticipants} Teilnehmer!");
            Console.WriteLine($"> Importiere {numberOfComposers} Komponisten/Autoren!");
            Console.WriteLine($"> Importiere {numberOfVotes} Votes!");

            int persistedData = await unitOfWork.SaveChangesAsync();
            Console.WriteLine($"Es wurden {persistedData} Daten importiert\n");

            if (persistedData != numberOfVotes + numberOfComposers + numberOfCountries + numberOfParticipants)
            {
                Console.Error.WriteLine("Fehler beim Import! Nicht alle Daten wurden importiert!");
            }

            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static async Task<(ParticipantComposer[] composers, Vote[] votes)> ImportAsync(
            string participantsFilename,
            string votesFilename)
        {
            var participantsLines = await MyFile.ReadStringMatrixFromCsvAsync(participantsFilename, overreadTitleLine: true);
            var votesLines = await MyFile.ReadStringMatrixFromCsvAsync(votesFilename, overreadTitleLine: true);

            var countries = votesLines
                .GroupBy(line => line[1])
                .Select(grp => new Country
                {
                    Name = grp.Key
                })
                .ToArray();

            var participants = participantsLines
                .GroupBy(line => line[2])
                .Select(grp => new Participant
                {
                    ActorName = grp.Key,
                    SongTitle = grp.First()[3],
                    StartNumber = int.Parse(grp.First()[0]),
                    Country = countries.First(c => c.Name == grp.First()[1])
                })
                .ToArray();

            var composers = participantsLines
                .Select(line => (ActorName: line[2], Composers: line[4].Split(',')))
                .SelectMany(item =>
                {
                    var (actorName, composerNames) = item;
                    var participant = participants.First(p => p.ActorName == actorName);

                    return composerNames
                        .Select(composerName => new ParticipantComposer
                        {
                            Name = composerName,
                            Participant = participant
                        }).ToList();
                })
                .ToArray();

            var votes = votesLines
                .Select(line => new Vote
                {
                    Points = int.Parse(line[3]),
                    Country = countries.First(c => c.Name == line[1]),
                    Participant = participants.First(p => p.Country.Name == line[2])
                })
                .ToArray();

            return (composers, votes);
        }
    }
}
