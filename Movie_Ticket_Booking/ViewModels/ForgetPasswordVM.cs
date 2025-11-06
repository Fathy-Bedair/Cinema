using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        public string UserNameOREmail { get; set; } = string.Empty;
    }
}
