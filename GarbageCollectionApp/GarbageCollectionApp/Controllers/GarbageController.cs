using GarbageCollectionApp.Data;
using GarbageCollectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarbageCollectionApp.Controllers
{
    [Route("api/data")]
    [ApiController]
    public class GarbageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GarbageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GarbageCollection data)
        {
            Console.WriteLine($"Received: {data?.IdGarbageBin}, {data?.CollectionTime}");

            if (data == null || string.IsNullOrWhiteSpace(data.IdGarbageBin))
                return BadRequest("Invalid data.");

            _context.GarbageCollections.Add(data);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Data saved successfully." });
        }


        [HttpGet]
        public async Task<IActionResult> GetGarbageCollections()
        {
            var data = await _context.GarbageCollections.ToListAsync();
            return Ok(data);
        }
    }
}
