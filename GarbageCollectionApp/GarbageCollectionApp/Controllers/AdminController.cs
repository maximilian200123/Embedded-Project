using GarbageCollectionApp.Data;
using GarbageCollectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarbageCollectionApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Citizens()
        {
            var citizens = await _context.Citizens.ToListAsync();
            return View(citizens);
        }

        public IActionResult AddCitizen()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCitizen([Bind("FirstName,LastName,Email,Cnp")] Citizen citizen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(citizen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Citizens));
            }
            return View(citizen);
        }


        public async Task<IActionResult> EditCitizen(int id)
        {
            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }
            return View(citizen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCitizen(int id, [Bind("Id,FirstName,LastName,Email,Cnp")] Citizen citizen)
        {
            if (id != citizen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(citizen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenExists(citizen.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Citizens));
            }
            return View(citizen);
        }

        public async Task<IActionResult> DeleteCitizen(int id)
        {
            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }

            return View(citizen);
        }

        [HttpPost, ActionName("DeleteCitizen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }

            _context.Citizens.Remove(citizen);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Citizens));
        }

        private bool CitizenExists(int id)
        {
            return _context.Citizens.Any(e => e.Id == id);
        }

        public IActionResult DisplayBins()
        {
            var bins = _context.GarbageBins.ToList();

            return View(bins);
        }

        [HttpGet]
        public IActionResult AddBin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBin(string binId)
        {
            if (ModelState.IsValid)
            {
                // Check if a bin with the same IdGarbageBin already exists
                var existingBin = _context.GarbageBins
                    .FirstOrDefault(bin => bin.IdGarbageBin == binId);

                if (existingBin != null)
                {
                    // If the bin exists, add a validation error
                    ModelState.AddModelError("binId", "A bin with this ID already exists.");
                    return View();  // Return the view with the error
                }

                // If no duplicate, create and add the new bin
                var newBin = new GarbageBin
                {
                    IdGarbageBin = binId
                };

                _context.GarbageBins.Add(newBin);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();  // If model is not valid, just return the view
        }



        [HttpGet]
        public IActionResult AssignBin(int citizenId)
        {
            var citizen = _context.Citizens.FirstOrDefault(c => c.Id == citizenId);
            if (citizen == null)
            {
                return NotFound();
            }

            var bins = _context.GarbageBins.ToList();
            ViewBag.Bins = bins;

            return View(citizen);
        }

        [HttpPost]
        public IActionResult AssignBin(int citizenId, string binId, string address)
        {
            var citizen = _context.Citizens.FirstOrDefault(c => c.Id == citizenId);
            var bin = _context.GarbageBins.FirstOrDefault(b => b.IdGarbageBin == binId);

            if (citizen == null || bin == null)
            {
                return NotFound();
            }

            var garbageBinCitizen = new GarbageBinCitizen
            {
                IdCitizen = citizenId,
                IdGarbageBin = binId,
                Address = address
            };

            _context.GarbageBinCitizens.Add(garbageBinCitizen);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
