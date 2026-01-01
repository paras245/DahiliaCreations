using DahiliaCreations.Data;
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
            return View(await _context.Fish.ToListAsync());
        }

        // GET: Fish/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fish/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fish fish)
        {
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

                _context.Add(fish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fish);
        }

        // GET: Fish/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish == null) return NotFound();
            return View(fish);
        }

        // POST: Fish/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> Delete(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish == null) return NotFound();
            return View(fish);
        }

        // POST: Fish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fish = await _context.Fish.FindAsync(id);
            _context.Fish.Remove(fish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
