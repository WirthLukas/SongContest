using Microsoft.AspNetCore.Mvc;
using SongContest.Web.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SongContest.Core.Contracts;
using SongContest.Core.Entities;
using SongContest.Web.DataTransferObjects;

namespace SongContest.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var participants = await _unitOfWork.Participants.GetAsync(
                filter: null,
                orderBy: query => query.OrderBy(p => p.Country.Name),
                includeProperties: nameof(Participant.Country)
            );

            return View(participants.Select(p => new ParticipantOverviewDto(
                    Id: p.Id,
                    Country: p.Country.Name,
                    Song: p.SongTitle,
                    Name: p.ActorName
                )));
        }

        [HttpGet]
        public async Task<IActionResult> GetResultOverview()
        {
            var countryNames = await _unitOfWork.Countries.GetAsync(
                    orderBy: query => query.OrderBy(c => c.Name)
                );

            var viewModel = new ParticipantResultsViewModel
            {
                CountryNames = countryNames.Select(c => c.Name),
                Participants = await _unitOfWork.Participants.GetParticipantResults()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
