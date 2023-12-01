using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAndCalculations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10091422_PROG6212_POE.Models;

namespace ST10091422_PROG6212_POE.Controllers
{
    [Authorize]// Requires authorization for accessing any action in this controller
    public class ModuleController : Controller
    {
        // This class was adapted from tutorialspoint
        // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm
        
        private readonly St10091422Prog6212Part2Context _context; // Database context for module operations
        private readonly Calculations _calc; // Helper class for calculations

        // Constructor to initialize dependencies
        public ModuleController(St10091422Prog6212Part2Context context, Calculations calc)
        {
            _context = context;
            _calc = calc;
        }

        // GET: Module
        public async Task<IActionResult> Index()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Retrieve user ID from claims
            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

            Console.WriteLine(userDataClaim.Value);

            // Retrieve the last (latest) semester for the authenticated user
            var semester = await _context.Semesters.// Filtering semesters based on the user's UserId.
                OrderBy(s => s.SemesterId).Include(m => m.Modules).//Including related 'Modules' for the selected semester
                LastOrDefaultAsync(u => u.UserId == Convert.ToInt32(userDataClaim.Value));// Asynchronously retrieving the last (latest) semester, or null if none is found.

            // If no semester is found, create a new Semester instance
            if (semester == null)
            {
                semester = new Semester();
            }

            // Set ViewData for the view
            ViewData["NumberOfWeeks"] = semester.NumberOfWeeks;
            ViewData["StartDate"] = Convert.ToString(string.Format("{0:dd/MM/yyyy}", semester.StartDate));
            var modules = semester.Modules;
            return View(modules.ToList());
        }

        // GET: Module/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Check for null parameters
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            // Retrieve the module details
            var @module = await _context.Modules
                .Include(m => m.Semester)
                .FirstOrDefaultAsync(m => m.ModuleId == id);

            // If no module is found, return NotFound
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Module/Create
        public IActionResult Create()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Check if a semester exists before allowing module creation
            if (!SemesterExists())
            {
                ViewData["message"] = "Create a Semester before adding module";
                return Redirect("/Semester/Create");
            }
            return View();
        }

        // POST: Module/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Module module)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Retrieve semester data using a separate method
            var semester = await Task.Run(() => semesterData());


            // Calculate self-study hours per week based on module details and semester data
            int selfStudyHours = _calc.SelfStudyHoursPerWeek((int)module.NumberOfCredits, (int)module.ClassHoursPerWeek, (int)semester.NumberOfWeeks);

            // Ensure self-study hours per week is at least 1
            if (selfStudyHours < 1)
            {
                selfStudyHours = 1;
            }

            // Set calculated values in the module
            module.NumberOfSelfStudyHoursPerWeek = selfStudyHours;
            module.NumberOfHours = 0;
            module.RemainingHours = selfStudyHours;
            module.CurrentDate = semester.CurrentDate;
            module.SemesterId = semester.SemesterId;

            // Add the new module to the database
            await Task.Run(async () => await _context.Modules.AddAsync(module));

            await _context.SaveChangesAsync();

            // Redirect to the Semester controller after successful creation
            return Redirect("/Semester");
        }

        // GET: Module/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Check for null parameters
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            // Retrieve the module for editing
            var @module = await _context.Modules.FindAsync(id);

            // If no module is found, return NotFound
            if (@module == null)
            {
                return NotFound();
            }

            // Populate ViewData for the view
            ViewData["module"] = module.Name;
            return View();
        }

        // POST: Module/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Module @module)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm


            // Retrieve the existing module from the database
            var foundModule = await _context.Modules.
                FirstOrDefaultAsync(m => m.ModuleId == id);

            // Check if the existing module is found
            if (foundModule != null)
            {
                // Set the current date for the module
                foundModule.CurrentDate = module.CurrentDate;// the current dat for module is set

                // This if statement was adapted from YouTube
                //https://youtu.be/GG5FnN2nqNM?si=RLpp46qw0y8ePiuR
                //Max O'Didily
                //https://www.youtube.com/@maxodidily

                // Retrieve user-specific semester data
                var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

                var sem = await _context.Semesters.
                    OrderByDescending(s => s.SemesterId).Include(m => m.Modules).
                    FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(userDataClaim.Value));

                // Check if the current date exceeds the current week's end date
                if (DateOnly.FromDateTime((DateTime)foundModule.CurrentDate).DayNumber - DateOnly.FromDateTime((DateTime)sem.WeekStart).DayNumber > 6)
                {// the difference between the start date and current dat is 7 the following values are reset
                    DateTime D = (DateTime)sem.WeekStart;
                    sem.WeekStart = D.AddDays(7);
                    Console.WriteLine($"Count: {sem.Modules.Count}");
                    foreach (var m in sem.Modules)
                    {// FIX THIS
                        m.NumberOfHours = 0;
                        m.RemainingHours = (int)m.NumberOfSelfStudyHoursPerWeek;
                    }
                }

                // Update module hours and remaining hours
                foundModule.NumberOfHours += module.NumberOfHours;//  the modules NumberOfHours is appeneded
                int calcRemainingHours = (int)(foundModule.NumberOfSelfStudyHoursPerWeek - foundModule.NumberOfHours);

                // Ensure remaining hours is at least 1
                if (calcRemainingHours < 1)
                {
                    calcRemainingHours = 1;
                }
                foundModule.RemainingHours = calcRemainingHours;

                // the remaining hours is set as the difference between the module's NumberOfSelfStudyHoursPerWeek and NumberOfHours
                await _context.SaveChangesAsync();
                Console.WriteLine("saved");

                // Redirect to the Module controller after successful update
                return Redirect("/Module");
            }
            // Redirect to the Create action if the module is not found
            return Redirect("/Module/Create");
        }

        public async Task<Semester?> semesterData()
        {
            //This method was adapted from YouTube
            //https://youtu.be/zhYB_P3yjuc?si=bUZw-1eT03R1lwhy
            //C# WPF UI Academy
            //https://www.youtube.com/@WpfUI
            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

            var sem = await _context.Semesters.
                OrderByDescending(s => s.SemesterId).Include(m => m.Modules).
                FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(userDataClaim.Value));
            return sem;
        }
        private bool SemesterExists()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            var userDataClaim = ((ClaimsIdentity)User.Identity).FindFirst("id");

            var exists = (_context.Semesters?.Any(e => e.UserId == Convert.ToInt32(userDataClaim.Value))).GetValueOrDefault(); ;
            return exists;
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