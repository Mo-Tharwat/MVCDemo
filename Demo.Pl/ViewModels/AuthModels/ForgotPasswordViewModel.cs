using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
