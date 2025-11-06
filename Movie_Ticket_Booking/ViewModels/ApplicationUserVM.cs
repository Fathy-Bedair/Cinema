using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.ViewModels
{
    public class ApplicationUserVM
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string? ConfirmNewPassword { get; set; }
    }
}
