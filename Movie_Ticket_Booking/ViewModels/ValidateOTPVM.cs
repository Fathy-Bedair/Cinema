using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.ViewModels
{
    public class ValidateOTPVM
    {
        [Required]

        public string OTP { get; set; } = string.Empty;

        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
