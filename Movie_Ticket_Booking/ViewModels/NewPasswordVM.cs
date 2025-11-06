using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.ViewModels
{
    public class NewPasswordVM
    {
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
