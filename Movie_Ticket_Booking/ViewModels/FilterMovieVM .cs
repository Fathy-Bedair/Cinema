namespace Movie_Ticket_Booking.ViewModels
{
    public record FilterMovieVM(
        string? search,
        int? categoryId,
        int? cinemaId,
        string? sortOrder,
        bool? inCinema
    );

}
