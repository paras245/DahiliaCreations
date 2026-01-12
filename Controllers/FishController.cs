using DahiliaCreations.Data;
using DahiliaCreations.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahiliaCreations.Controllers
{
    public class FishController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public FishController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Fish
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please login to view fish.";
                return RedirectToAction("Login", "User");
            }

            return View(await _context.Fish.ToListAsync());
        }

        [AuthorizeRole("Admin")]
        // GET: Fish/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fish/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(Fish fish)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fish.ImageFile != null) fish.ImagePath = await UploadFile(fish.ImageFile);
                    if (fish.ImageFile2 != null) fish.ImagePath2 = await UploadFile(fish.ImageFile2);
                    if (fish.ImageFile3 != null) fish.ImagePath3 = await UploadFile(fish.ImageFile3);

                    _context.Add(fish);
                    await _context.SaveChangesAsync();

                    TempData["ToastrType"] = "success";
                    TempData["ToastrMessage"] = "Fish created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ToastrType"] = "error";
                    TempData["ToastrMessage"] = "Error creating fish: " + ex.Message;
                }
            }
            else
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please check the form for errors.";
            }
            return View(fish);
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return "/uploads/" + fileName;
        }

        // GET: Fish/Edit/5
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish == null) return NotFound();
            return View(fish);
        }

        // POST: Fish/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, Fish fish)
        {
            if (id != fish.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (fish.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(fish.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await fish.ImageFile.CopyToAsync(fileStream);

                    fish.ImagePath = "/uploads/" + fileName;
                }

                _context.Update(fish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fish);
        }

        // GET: Fish/Delete/5
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish == null) return NotFound();
            return View(fish);
        }

        // POST: Fish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            _context.Fish.Remove(fish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Fish/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastrType"] = "warning";
                TempData["ToastrMessage"] = "Please login to view fish details.";
                return RedirectToAction("Login", "User");
            }

            if (id == null)
            {
                return NotFound();
            }

            var fish = await _context.Fish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fish == null)
            {
                return NotFound();
            }

            return View(fish);
        }
    }
}
