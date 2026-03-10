using DahiliaCreations.Data;
using DahiliaCreations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DahiliaCreations.Controllers
{
    //Access Modifier class keyword ControllerName : Inherited Class
    public class HomeController : Controller
    {
        //Encapsulation readonly value in run time classname variable name
        private readonly ILogger<HomeController> _logger;

        //Encapsulation reaonly value on run time class name variable name
        private readonly ApplicationDbContext _context;

        //contructor depency injection 
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // You can later filter premium fish if needed
            var fishes = await _context.Fish
                                       .OrderBy(f => f.Id)
                                       .ToListAsync();

            return View(fishes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
