using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Movie_Ticket_Booking.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        [ValidateNever]
        public Movie Movie { get; set; }
        public DateTime PublishAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; }
        public bool IsValid { get; set; } = true;

        public string Code { get; set; }
        public decimal Discount { get; set; }
    }
}
