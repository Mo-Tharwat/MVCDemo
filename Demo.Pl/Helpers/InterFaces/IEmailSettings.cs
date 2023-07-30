using Demo.DAL.Models;

namespace Demo.PL.Helpers.InterFaces
{
	public interface IEmailSettings
	{
		public void SendEmail(Email email);
		public void SendEmailUsingMailKit(Email email);
	}
}
