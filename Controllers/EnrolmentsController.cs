using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.models;

namespace RaceDay.API.Controllers
{//start of namespace
    
    // Manages participant registrations/enrolments into race events and categories.
   
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

    
        // Retrieves all participant enrolments.
       
        [HttpGet]
        public async Task<IActionResult> GetEnrolments()
        {
            var enrolments = await _context.Enrolments
                .Include(e => e.User)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments);
        }

      
        // Registers a participant into an event and category.
      
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var raceEvent = await _context.Events.FindAsync(dto.EventId);
            if (raceEvent == null)
            {
                return BadRequest(new { message = "Event not found." });
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null || category.EventId != dto.EventId)
            {
                return BadRequest(new { message = "Invalid category for this event." });
            }

            var enrolment = new Enrolment
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                CategoryId = dto.CategoryId,
                EnrolmentDate = DateTime.UtcNow
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment successful!", enrolmentId = enrolment.Id });
        }
    }

    public class CreateEnrolmentDto
    {//start of class

        //properties to store
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }

    }//end of class
}//end of namespace