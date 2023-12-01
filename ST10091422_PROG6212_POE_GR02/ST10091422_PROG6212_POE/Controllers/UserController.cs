using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAndCalculations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10091422_PROG6212_POE.Models;

namespace ST10091422_PROG6212_POE.Controllers
{
    public class UserController : Controller
    {
        // This class was adapted from tutorialspoint
        // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

        // Database context for user operations
        private readonly St10091422Prog6212Part2Context _context;

        // Helper class for password-related calculations
        private readonly Calculations _calc;

        // Class managing authorized user state
        private readonly AuthorizedUser _authorizedUser;

        // Constructor to inject dependencies
        public UserController(St10091422Prog6212Part2Context context, Calculations calc, AuthorizedUser authorizedUser)
        {
            _context = context;
            _calc = calc;   
            _authorizedUser = authorizedUser;
        }


        // GET: Users/Create
        public IActionResult Register()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Redirect if the user is already authenticated
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            else
            {
                return View();
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Check if the user already exists
            var Exists = await UserExistsAsync(user.Username);

            if (Exists != null)
            {
                ViewData["message"] = "User Exists";
                return RedirectToAction(nameof(Register));
            }

            // Generate salt and hash the password
            var salt = _calc.GenerateSalt();// salt is generated
            var hashedPass = _calc.HashPassword(user.tempPass, salt);// hashed password is generated
            
            user.Password = hashedPass;
            user.Salt = salt;
            user.tempPass = string.Empty;

            // Add user to the database
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            // Retrieve the newly created user
            var currentUser = await UserExistsAsync(user.Username);

            // Set the user in the authorized user service and create a token
            _authorizedUser.setUser(currentUser);
            await setTokenAsync(currentUser);

            return Redirect("/");
            
        }

        // GET: Users/Edit/5
        public IActionResult Login()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // Redirect if the user is already authenticated
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            else
            {
                return View();
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            // This method was adapted from 
            var currentUser = await UserExistsAsync(user.Username);

            if (currentUser == null)
            {
                ViewData["message"] = "User not found";
                Console.WriteLine("User not found");

                return RedirectToAction(nameof(Login));
            }

            // Verify the entered password with the stored password
            if (!_calc.VerifyPassword(user.tempPass, currentUser.Salt, currentUser.Password))
            {

                ViewData["message"] = "Password does not match username";
                Console.WriteLine("Password does not match username");
                return RedirectToAction(nameof(Login));
            }

            // Set the authorized user and generate authentication token
            _authorizedUser.setUser(currentUser);
            await setTokenAsync(currentUser);
            
            return Redirect("/");
        }

        // Handle user logout
        public async Task<IActionResult> Logout()
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return Redirect("/");
        }

        // Generate and set the authentication token for the user
        private async Task setTokenAsync(User user)
        {
            // This method was adapted from .c-sharpcorner
            // https://www.c-sharpcorner.com/article/cookie-authentication-in-asp-net-core/
            // ANOOP KUMAR SHARMA
            // https://www.c-sharpcorner.com/members/anoop-kumar-sharma

            // Create a list of claims for the user
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()), // User ID as a claim
                new Claim(ClaimTypes.Actor, user.Username),  // Username as a claim
            };

            // Create a new ClaimsIdentity using the claims and Cookie Authentication scheme
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Set authentication properties for the token
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true, // Allow the token to be refreshed
                IsPersistent = true, // Make the token persistent (survives browser sessions)
            };

            // Sign in the user with the generated token using Cookie Authentication
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);
        }

        // Check if a user with the given username already exists in the database
        private async Task<User> UserExistsAsync(string username)
        {
            // This method was adapted from tutorialspoint
            // https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_controllers.htm

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            return user;
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
