using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.models; 

namespace RaceDay.API.Controllers
{//start of namespace
    
    // Manages race events, allowing organisers to create events and participants to browse them.
    
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {//start of class

        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

       
        // Retrieves all scheduled race events including their organiser and categories.
 
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {//start

            var events = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .ToListAsync();

            return Ok(events);

        }//end

  
        // Retrieves a single race event by its unique ID.
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {//start
            var raceEvent = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (raceEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(raceEvent);

        }//end

       
        // Allows an organiser to create a new race event.
 
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            // Verify that the organiser exists and is authorized
            var organiser = await _context.Users.FindAsync(dto.OrganiserId);
            if (organiser == null || organiser.Role != "Organiser")
            {//start of if 

                return BadRequest(new { message = "Invalid organiser ID or user is not an Organiser." });

            }//close if if

            var raceEvent = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType,
                OrganiserId = dto.OrganiserId
            };

            _context.Events.Add(raceEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = raceEvent.Id }, new { message = "Event created successfully!", eventId = raceEvent.Id });
        }
    }

  
    // DTO payload for creating a new race event.

    public class CreateEventDto
    {//start of class

        //properties to storing 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty; // "Run", "Walk", or "Cycle"
        public int OrganiserId { get; set; }

    }//end of class
}//end of namespace