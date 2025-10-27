using Movie_Ticket_Booking.Models;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Repositories.IRepositories
{
    public interface IMovieSubImageRepository : IRepository<MovieSubImage>
    {
        void RemoveRange(IEnumerable<MovieSubImage> movieSubImages);
    }
}
