using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.models;

namespace RaceDay.API.Controllers
{//start of namespace
    
    // Manages race results, finishing times, and positions for enrolled participants.
   
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        
        // Retrieves all recorded race results.
        
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.User)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Event)
                .ToListAsync();

            return Ok(results);
        }

        
        // Records or updates a race result for a specific enrolment.
        
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateResult([FromBody] CreateResultDto dto)
        {
            var enrolment = await _context.Enrolments
                .Include(e => e.Result)
                .FirstOrDefaultAsync(e => e.Id == dto.EnrolmentId);

            if (enrolment == null)
            {
                return BadRequest(new { message = "Enrolment record not found." });
            }

            // Check if a result already exists for this enrolment
            if (enrolment.Result != null)
            {
                // Update existing result
                enrolment.Result.FinishTime = dto.FinishTime;
                enrolment.Result.Position = dto.Position;
            }
            else
            {
                // Create new result
                var result = new Result
                {
                    EnrolmentId = dto.EnrolmentId,
                    FinishTime = dto.FinishTime,
                    Position = dto.Position
                };
                _context.Results.Add(result);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Race result saved successfully!" });
        }
    }

    public class CreateResultDto
    {//start of class

        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }

    }//end of class
}//end of namespace