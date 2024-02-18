using System.ComponentModel.DataAnnotations;

namespace Malefashion.Models.ViewModels
{
	public class ContactViewModel
	{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(255, ErrorMessage = "Email length can't exceed 255")]
        [RegularExpression("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z]{2,}(?:\\.[a-zA-Z]{2,})+$",
        ErrorMessage = "Email is required and must be properly formatted.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}
