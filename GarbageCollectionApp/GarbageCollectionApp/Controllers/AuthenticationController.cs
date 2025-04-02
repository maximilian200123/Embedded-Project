using GarbageCollectionApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GarbageCollectionApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AppDbContext _context;

        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // this check is needed for the switch of the navigation bar from login to logout
            var adminUsername = HttpContext.Session.GetString("AdminUsername");

            if (string.IsNullOrEmpty(adminUsername))
            {
                return View();
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // find the admin by username
            var admin = await _context.Admins
                .Where(a => a.Username == username)
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("~/Views/Authentication/Login.cshtml");
            }

            if (VerifyPassword(password, admin.Password))
            {
                //store session data
                HttpContext.Session.SetString("AdminUsername", admin.Username);
                
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("~/Views/Authentication/Login.cshtml");
        }


        // simple password verification using sha256
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                var hashString = string.Concat(hashedPassword.Select(b => b.ToString("x2")));

                return storedHash == hashString;
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Authentication");
        }
    }
}
