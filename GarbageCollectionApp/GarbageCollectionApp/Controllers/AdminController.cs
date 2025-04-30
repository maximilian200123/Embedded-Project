using GarbageCollectionApp.Data;
using GarbageCollectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // check if the user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername")))
            {
                return RedirectToAction("Login", "Authentication");
            }

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

        public async Task<IActionResult> DisplayBins()
        {
            // get all bins
            var allBins = await _context.GarbageBins.ToListAsync();

            // get the assigned bins
            var assignments = await _context.GarbageBinCitizens
                .Include(gbc => gbc.Citizen)
                .ToListAsync();

            ViewBag.AssignedBins = assignments;

            return View(allBins);
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
                var existingBin = _context.GarbageBins
                    .FirstOrDefault(bin => bin.IdGarbageBin == binId);

                if (existingBin != null)
                {
                    ModelState.AddModelError("binId", "A bin with this ID already exists.");
                    return View();
                }

                var newBin = new GarbageBin
                {
                    IdGarbageBin = binId
                };

                _context.GarbageBins.Add(newBin);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }



        [HttpGet]
        public IActionResult AssignBin(int citizenId)
        {
            var citizen = _context.Citizens.Find(citizenId);
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
            var citizen = _context.Citizens.Find(citizenId);
            var bin = _context.GarbageBins.Find(binId);

            if (citizen == null || bin == null)
            {
                return NotFound();
            }

            string normalizedAddress = address.Trim().ToLower();

            // Check if the bin is already assigned to a different citizen
            var existingAssignment = _context.GarbageBinCitizens
                .FirstOrDefault(gbc => gbc.IdGarbageBin == binId);

            if (existingAssignment != null && existingAssignment.IdCitizen != citizenId)
            {
                ModelState.AddModelError("", "This bin is already assigned to another citizen.");
                ViewBag.Bins = _context.GarbageBins.ToList();
                return View(citizen);
            }

            // Check if the bin is already assigned at a different address
            if (existingAssignment != null && existingAssignment.Address.ToLower() != normalizedAddress)
            {
                ModelState.AddModelError("", "This bin is already assigned at a different address.");
                ViewBag.Bins = _context.GarbageBins.ToList();
                return View(citizen);
            }

            var garbageBinCitizen = new GarbageBinCitizen
            {
                IdCitizen = citizenId,
                IdGarbageBin = binId,
                Address = address.Trim()
            };

            _context.GarbageBinCitizens.Add(garbageBinCitizen);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> CitizenCollections(int? citizenId)
        {
            // Retrieving the list of citizens for selection
            var citizens = await _context.Citizens
            .Select(c => new
            {
                c.Id,
                FullName = c.FirstName + " " + c.LastName
            })
            .ToListAsync();

            ViewBag.Citizens = new SelectList(citizens, "Id", "FullName");

            List<GarbageCollectionDTO> collections;

            if (citizenId == null)
            {
                // when no citizen selected, show all collections
                var collectionsRaw = await _context.GarbageCollections
                    .Select(gc => new
                    {
                        gc.IdGarbageBin,
                        gc.CollectionTime,
                        gc.Address,
                        Citizen = _context.GarbageBinCitizens
                            .Where(gbc => gbc.IdGarbageBin == gc.IdGarbageBin)
                            .Select(gbc => new { gbc.Citizen.Id, gbc.Citizen.FirstName, gbc.Citizen.LastName })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                collections = collectionsRaw.Select(c => new GarbageCollectionDTO
                {
                    IdGarbageBin = c.IdGarbageBin,
                    CollectionTime = c.CollectionTime,
                    CitizenId = c.Citizen?.Id ?? 0,
                    CitizenFirstName = c.Citizen?.FirstName ?? "Unknown",
                    CitizenLastName = c.Citizen?.LastName ?? "Unknown",
                    Address = c.Address ?? "Unknown"
                }).ToList();
            }
            else
            {
                // when citizen selected, show only their collections
                var collectionsRaw = await _context.GarbageCollections
                    .Where(gc => _context.GarbageBinCitizens
                        .Any(gbc => gbc.IdGarbageBin == gc.IdGarbageBin && gbc.Citizen.Id == citizenId))
                    .Select(gc => new
                    {
                        gc.IdGarbageBin,
                        gc.CollectionTime,
                        gc.Address,
                        Citizen = _context.GarbageBinCitizens
                            .Where(gbc => gbc.IdGarbageBin == gc.IdGarbageBin && gbc.Citizen.Id == citizenId)
                            .Select(gbc => new { gbc.Citizen.Id, gbc.Citizen.FirstName, gbc.Citizen.LastName })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                collections = collectionsRaw.Select(c => new GarbageCollectionDTO
                {
                    IdGarbageBin = c.IdGarbageBin,
                    CollectionTime = c.CollectionTime,
                    CitizenId = c.Citizen?.Id ?? 0,
                    CitizenFirstName = c.Citizen?.FirstName ?? "Unknown",
                    CitizenLastName = c.Citizen?.LastName ?? "Unknown",
                    Address = c.Address ?? "Unknown"
                }).ToList();
            }

            return View(collections);
        }
       
    }
}
