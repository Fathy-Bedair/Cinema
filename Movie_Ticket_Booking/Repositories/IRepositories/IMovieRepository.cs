using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default);
    }
}