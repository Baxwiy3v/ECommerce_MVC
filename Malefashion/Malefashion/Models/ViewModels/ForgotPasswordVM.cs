using System.ComponentModel.DataAnnotations;

namespace Malefashion.Models.ViewModels
{
	public class ForgotPasswordVM
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		[MaxLength(255, ErrorMessage = "Email length can't exceed 255")]
		[RegularExpression("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z]{2,}(?:\\.[a-zA-Z]{2,})+$",
	    ErrorMessage = "Email is required and must be properly formatted.")]
		public string Email { get; set; }
	}
}
