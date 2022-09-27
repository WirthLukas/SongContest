using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SongContest.Core.Contracts;
using SongContest.Core.Entities;
using SongContest.Web.Models;

namespace SongContest.Web.Controllers
{
    public class ParticipantComposerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantComposerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var participant = await _unitOfWork.Participants
                .GetByIdAsync(id, nameof(Participant.Country));

            var composerNames = await _unitOfWork.Composers.GetComposerNamesOfParticipant(id);

            return View(new CreateComposerViewModel(participant, composerNames));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateComposerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var composer = viewModel.GetComposer();
                await _unitOfWork.Composers.AddAsync(composer);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var participant = await _unitOfWork.Participants
                .GetByIdAsync(viewModel.ParticipantId, nameof(Participant.Country));

            var composerNames = await _unitOfWork.Composers.GetComposerNamesOfParticipant(viewModel.ParticipantId);

            return View(new CreateComposerViewModel(participant, composerNames));
        }
    }
}
