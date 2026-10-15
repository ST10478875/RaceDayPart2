using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.models;

namespace RaceDay.API.Controllers
{//start of namespace
 
    // Manages race categories (e.g., 10km, Senior, Under 20) associated with specific events.
    
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }
        
        // Retrieves all categories available across events.
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Event)
                .ToListAsync();

            return Ok(categories);
        }

       
        // Creates a new category for a specific event.
        
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var raceEvent = await _context.Events.FindAsync(dto.EventId);
            if (raceEvent == null)
            {
                return BadRequest(new { message = "Associated event not found." });
            }

            var category = new Category
            {
                Name = dto.Name,
                EventId = dto.EventId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category created successfully!", categoryId = category.Id });
        }
    }

    public class CreateCategoryDto
    {//start of class

        public string Name { get; set; } = string.Empty;
        public int EventId { get; set; }

    }//end of class
}//end of namespace