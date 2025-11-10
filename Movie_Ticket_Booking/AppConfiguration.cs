using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using Movie_Ticket_Booking.Utitlies.DBInitilizer;
using NuGet.Protocol.Core.Types;

namespace ECommerce
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connection)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                
                option.UseSqlServer(connection);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequiredLength = 8;
                option.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Default login path
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Default access denied path
            });

            services.AddTransient<IEmailSender, EmailSender>();


            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IMovieSubImageRepository, MovieSubImageRepository>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            services.AddScoped<IRepository<PromotionUsage>,Repository<PromotionUsage>>();


            services.AddScoped<IDBInitializer, DBInitializer>();

        }
    }
}
