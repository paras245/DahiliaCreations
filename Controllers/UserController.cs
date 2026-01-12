using DahiliaCreations.Data;
using DahiliaCreations.Filters;
using DahiliaCreations.Helper;
using DahiliaCreations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahiliaCreations.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- REGISTER ----------------
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string confirmPassword)
        {
            ModelState.Remove("Role"); // Set server-side
            ModelState.Remove("IsActive"); // Has default value

            if (!ModelState.IsValid)
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please check the form.";
                return View(user);
            }

            if (user.PasswordHash != confirmPassword)
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Passwords do not match.";
                user.PasswordHash = ""; 
                return View(user);
            }

            bool usernameExists = await _context.Users.AnyAsync(x => x.Username == user.Username);
            if (usernameExists)
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Username already exists.";
                user.PasswordHash = ""; // Clear password
                return View(user);
            }

            bool emailExists = await _context.Users.AnyAsync(x => x.Email == user.Email);
            if (emailExists)
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Email already registered.";
                user.PasswordHash = ""; // Clear password
                return View(user);
            }

            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
            user.Role = "Customer";
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = "Registration successful. Please login.";

            return RedirectToAction(nameof(Login));
        }

        // ---------------- LOGIN ----------------
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please enter both username and password.";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username && x.IsActive);

            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Invalid username or password.";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("Username", user.Username);

            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = $"Welcome back, {user.Username}!";

            return RedirectToAction("Index", "Home");
        }

        // ---------------- LOGOUT ----------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = "Logged out successfully.";

            return RedirectToAction(nameof(Login));
        }

        // ---------------- FORGOT PASSWORD ----------------
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please enter your email address.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user != null)
            {
                user.ResetToken = Guid.NewGuid().ToString();
                user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                // In a real application, you would send an email here
                // For testing purposes, we'll show the reset link in TempData
                var resetLink = Url.Action("ResetPassword", "User", new { token = user.ResetToken }, Request.Scheme);
                TempData["ToastrType"] = "success";
                TempData["ToastrMessage"] = $"Reset link generated! (In production, this would be sent via email)";
                
                // Store the reset link for display (remove this in production)
                TempData["ResetLink"] = resetLink;
            }
            else
            {
                // Don't reveal if email exists or not for security
                TempData["ToastrType"] = "info";
                TempData["ToastrMessage"] = "If the email exists, a reset link has been generated.";
            }

            return View();
        }

        // ---------------- RESET PASSWORD ----------------
        public async Task<IActionResult> ResetPassword(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Invalid or expired reset link.";
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please enter a new password.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Passwords do not match.";
                return View();
            }

            if (newPassword.Length < 6)
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Password must be at least 6 characters long.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "Invalid or expired reset link.";
                return RedirectToAction(nameof(Login));
            }

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = "Password reset successfully. Please login with your new password.";

            return RedirectToAction(nameof(Login));
        }

        // ---------------- ADMIN: USER LIST ----------------
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
    }
}
