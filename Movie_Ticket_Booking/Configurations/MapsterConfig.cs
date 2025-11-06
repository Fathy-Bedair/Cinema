using Mapster;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.ViewModels;

namespace Movie_Ticket_Booking.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfig(this IServiceCollection services)
        {
            TypeAdapterConfig<ApplicationUser, ApplicationUserVM>
                    .NewConfig()
                    .Map(d => d.FullName, s => $"{s.FirstName} {s.LastName}")
                    .TwoWays();
        }
    }
}
