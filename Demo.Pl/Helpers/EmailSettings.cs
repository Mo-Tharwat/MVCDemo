using Demo.DAL.Models;
using Demo.PL.Helpers.InterFaces;
using Demo.PL.Helpers.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
//using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Demo.PL.Helpers
{
	public  class EmailSettings : IEmailSettings
	{
		private readonly MailSettings _appMailSettings;

		public EmailSettings(IOptions<MailSettings> appMailSettings)
        {
			_appMailSettings = appMailSettings.Value;
		}


        /// <summary>
        /// this is an outdated way to send link throw email but this is how to use it better use (MailKit)
		/// to use this method un comment the implimentation then un comment Using System.Net.Mail
        /// </summary>
        /// <param name="email">the email of the user</param>
        public void SendEmail(Email email)
		{
			//using var client = new SmtpClient("smtp.gmail.com", 587);
			//client.EnableSsl = true; // if the server have SSL this line mean the mail will use it to encrypt it 
			/// un comment the following commands to send an email
			//client.Credentials = new NetworkCredential("your email", "put the password provided by Google App Passwords in TwoFactor Auth settings");
			//client.Send("YourEmail", email.To, email.Subject, email.Body);

		}

		/// <summary>
		/// sends an email useing mailkit library
		/// </summary>
		/// <param name="email">the email of the user</param>
		public void SendEmailUsingMailKit(Email email)
		{
			//
			var mail = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_appMailSettings.Email), // the sender mail in AppSetting Folder
				Subject = email.Subject
			};

			mail.To.Add(MailboxAddress.Parse(email.To));

			var builder = new BodyBuilder();
			builder.TextBody = email.Body;
			mail.Body = builder.ToMessageBody();

			mail.From.Add(new MailboxAddress(_appMailSettings.DisplayName, _appMailSettings.Email));

			using var smtp = new SmtpClient();
			smtp.Connect(_appMailSettings.Host, _appMailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_appMailSettings.Email, _appMailSettings.Password);
			smtp.Send(mail);
			smtp.Disconnect(true);
		}
	}
}
