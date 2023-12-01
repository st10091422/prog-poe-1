using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10091422_PROG6212_POE.Models;

namespace ST10091422_PROG6212_POE.Controllers
{
    [Authorize]
    public class SemesterController : Controller
    {
        // This class was adapted from tutorialspoint
        // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

        private readonly St10091422Prog6212Part2Context _context; // Database context for semester operations
        private readonly AuthorizedUser _authorizedUser; // Helper class for managing authorized user information

        // Constructor to initialize dependencies
        public SemesterController(St10091422Prog6212Part2Context context, AuthorizedUser authorizedUser)
        {
            _context = context;
            _authorizedUser = authorizedUser;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Retrieve the user's ID from the claims
            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

            Console.WriteLine(userDataClaim.Value);

            // Retrieve the latest semester for the user, including related modules
            var semester = await _context.Semesters.// Filtering semesters based on the user's UserId.
                OrderBy(s => s.SemesterId).Include(m => m.Modules).//Including related 'Modules' for the selected semester
                LastOrDefaultAsync(u => u.UserId == Convert.ToInt32(userDataClaim.Value));// Asynchronously retrieving the last (latest) semester, or null if none is found.

            // If no semester is found, create an empty one
            if (semester == null)
            {
                semester = new Semester();
            }

            // Pass data to the view
            ViewData["NumberOfWeeks"] = semester.NumberOfWeeks;
            ViewData["StartDate"] = Convert.ToString(string.Format("{0:dd/MM/yyyy}", semester.StartDate)); 
            var modules = semester.Modules;
            return View(modules.ToList());
        }

        public async Task<IActionResult> Graph()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Retrieve the user's ID from the claims
            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

            Console.WriteLine(userDataClaim.Value);

            // Retrieve the latest semester for the user, including related modules
            var semester = await _context.Semesters.// Filtering semesters based on the user's UserId.
                OrderBy(s => s.SemesterId).Include(m => m.Modules).//Including related 'Modules' for the selected semester
                LastOrDefaultAsync(u => u.UserId == Convert.ToInt32(userDataClaim.Value));// Asynchronously retrieving the last (latest) semester, or null if none is found.

            // If no semester is found, create an empty one
            if (semester == null)
            {
                semester = new Semester();
            }

            var modules = semester.Modules;

            return View(modules.ToList());
        }

        // GET: Semesters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Check if semester ID is not provided or if Semesters collection is null
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            // Retrieve the specified semester including related user information
            var semester = await _context.Semesters
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SemesterId == id);

            // If the semester is not found, return NotFound
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }


        // GET: Semesters/Create
        public IActionResult Create()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            return View();
        }

        // POST: Semesters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Set initial values for WeekStart, CurrentWeek, and CurrentDate
            semester.WeekStart = semester.StartDate;
            semester.CurrentWeek = 1;
            semester.CurrentDate = semester.StartDate;// the semester weekStart is set as semester startDate
                                                      //_semester.setSemester(semester);// the semester is set

            // Retrieve the user's ID from claims and assign it to the semester
            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id").Value;
            semester.UserId = Convert.ToInt32(userDataClaim);

            // Add the new semester to the database and save changes
            await _context.AddAsync(semester);
            await _context.SaveChangesAsync();

            // Redirect to the Index action after successful creation
            return RedirectToAction(nameof(Index));
        }

        // Check if a semester with the given ID exists
        private bool SemesterExists(int id)
        {
          return (_context.Semesters?.Any(e => e.SemesterId == id)).GetValueOrDefault();
        }
    }
}
/*
 REFERENCE LIST

    - Chao, L. 2014. Cloud Database Development and Management, CRC Press.
    - Ervis Trupja (2021) 29. Adding your first service | ASP.NET MVC, 15/08/2021. [online] Available at: https://youtu.be/zvkSIOs6s-M [Accessed 27 December 2023]
    - Payload (2021) WPF C# Professional Modern Flat UI Tutorial, 3 April 2021. [online] Available at: https://youtu.be/PzP8mw7JUzI?si=m_tnXOm6f8uKWyQv [Accessed 27 December 2023]
    - Patrick God (2023) .NET 7 Web API Role-Based Authorization with JSON Web Tokens (JWT) & the dotnet user-jwts CLI. 14 February.Available at: https://youtu.be/6sMPvucWNRE [Accessed 27 December 2023]
    - Microft.learn. 2023. Tutorial: Create a .NET class library using Visual Studio Code, 9 July 2023. [online] Available at: Create a .NET class library using Visual Studio Code - .NET | Microsoft Learn [Accessed 23 August 2023]
    - tutorialspoint.com. n.d. ASP.NET MVC - Controllers. [online] Available at: https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm [Accessed 27 December 2023]
    - c-sharpcorner.com. 2021. Cookie Authentication In ASP.NET Core, 17 May 2021. [online] Available at: https://www.c-sharpcorner.com/article/cookie-authentication-in-asp-net-core/ [Accessed 27 December 2023]
*/