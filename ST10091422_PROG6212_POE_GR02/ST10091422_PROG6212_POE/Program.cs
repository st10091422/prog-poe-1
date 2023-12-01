using DataAndCalculations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ST10091422_PROG6212_POE.Models;

namespace ST10091422_PROG6212_POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<St10091422Prog6212Part2Context>(options =>
            {
                IConfiguration configuration = new ConfigurationBuilder().
                SetBasePath(AppDomain.CurrentDomain.BaseDirectory).
                AddJsonFile("appsettings.json").Build();

                options.UseSqlServer(configuration.GetConnectionString("MyConnectionStringDev"));
                //optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyConnectionStringAzure"));
            });

            builder.Services.AddAuthentication(
               CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => {
                    option.LoginPath = "/User/Login";
                    option.ExpireTimeSpan = TimeSpan.FromHours(24);
                });

            builder.Services.AddSingleton<AuthorizedUser>();

            builder.Services.AddSingleton<Calculations>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}