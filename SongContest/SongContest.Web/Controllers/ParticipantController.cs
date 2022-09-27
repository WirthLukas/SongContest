using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SongContest.Core.Contracts;
using SongContest.Web.Models;

namespace SongContest.Web.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateParticipantViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateParticipantViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var participant = viewModel.GetParticipant();
                await _unitOfWork.Participants.AddAsync(participant);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (ValidationException ex)
                {
                    var validationResult = ex.ValidationResult;

                    if (validationResult.MemberNames.Any())
                    {
                        foreach (var memberName in validationResult.MemberNames)
                        {
                            ModelState.AddModelError(memberName == "Name" ? "CountryName" : memberName, validationResult.ToString());
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", validationResult.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(viewModel);
        }
    }
}
