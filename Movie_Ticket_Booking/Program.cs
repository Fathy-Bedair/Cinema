using ECommerce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.Configurations;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using Movie_Ticket_Booking.Utitlies.DBInitilizer;
using Stripe;

namespace Movie_Ticket_Booking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

            builder.Services.RegisterConfig(connectionString);
            builder.Services.RegisterMapsterConfig();


            builder.Services.AddAuthentication()
            .AddGoogle("google", opt =>
            {
                var googleAuth = builder.Configuration.GetSection("Authentication:Google");
                opt.ClientId = googleAuth["ClientId"];
                opt.ClientSecret = googleAuth["ClientSecret"];
                opt.SignInScheme = IdentityConstants.ExternalScheme;
            });

            builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
                facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
            });

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service!.Initialize();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Movies}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
