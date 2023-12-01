using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST10091422_PROG6212_POE.Models;

public partial class User
{
    // model of user
    // this class was adapted from YouTuble
    //https://youtu.be/afAET7CEHYk?si=CUd8NijCA_L1mfsK
    //Ervis Trupja
    //https://www.youtube.com/@DotNetHow
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    [Required(ErrorMessage = "Password field is required.")]
    public string? tempPass { get; set; }

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();
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