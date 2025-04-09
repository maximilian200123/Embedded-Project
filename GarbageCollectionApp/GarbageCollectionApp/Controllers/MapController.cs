// Controllers/MapController.cs
using GarbageCollectionApp.Data;
using GarbageCollectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarbageCollectionApp.Controllers
{
    public class MapController : Controller
    {
        private readonly AppDbContext _context;

        public MapController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CollectionMap(DateTime? selectedDate)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername")))
            {
                return RedirectToAction("Login", "Authentication");
            }

            // Set default to 15.10.2024
            var queryDate = selectedDate ?? new DateTime(2024, 10, 15);

            var collections = await _context.GarbageCollections
                .Where(gc => gc.CollectionTime.Date == queryDate.Date)
                .OrderBy(gc => gc.CollectionTime)
                .Select(gc => new
                {
                    gc.IdGarbageBin,
                    CollectionTime = gc.CollectionTime.ToString("o"),
                    gc.Address,
                    gc.Latitude,
                    gc.Longitude
                })
                .ToListAsync();

            ViewBag.Stops = new
            {
                StartPoint = new { Name = "SOMA HQ", Address = "Strada Șelimbărului 90, Cisnădie, Romania", Lat = 45.7315361, Lng = 24.1779393 },
                FinishPoint = new { Name = "Groapa de gunoi Cristian", Address = "DN1 FN, Cristian 557085", Lat = 45.7877059, Lng = 24.0247875 }
            };

            ViewBag.SelectedDate = queryDate.ToString("yyyy-MM-dd");
            return View(collections);
        }
    }
}