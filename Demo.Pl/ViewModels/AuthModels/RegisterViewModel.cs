using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class RegisterViewModel
	{
		public string FName { get; set; }

        public string LName { get; set; }

        [Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

        public bool IsAgree { get; set; }
    }
}
