using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string UserNameOREmail { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
