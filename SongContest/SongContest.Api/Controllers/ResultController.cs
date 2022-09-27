using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SongContest.Core.Contracts;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SongContest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResultController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // GET: api/<ResultController>
        [HttpGet("countrystats")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public Task<CountryStatisticsDto[]> GetCountryStats() => _unitOfWork.Countries.GetCountryStatisticsAsync();

        [HttpGet("ParticipantWithMost12Points")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public Task<Participant> GetParticipantWithMost12Points() => _unitOfWork.Participants.GetParticipantWithMost12Points();
    }
}
