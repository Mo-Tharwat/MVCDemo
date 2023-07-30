using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.Helpers.InterFaces;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{

    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSettings _emailSettings;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSettings emailSettings)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSettings = emailSettings;
		}


        #region Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
		{
			if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    FName = model.FName,
                    LName = model.LName,
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    IsAgree = model.IsAgree
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Login));

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
		}
		#endregion

		#region Login

		public IActionResult Login()
		{
			return View();
		}

        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email); // this FindByEmailAsync method has been implimented Because of that line .AddEntityFrameworkStores<MVCAppDemoDbcontext>(); in Startup.cs
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index","Home");
                        }
                    }
					ModelState.AddModelError(string.Empty, "Password is Wrong");
				}
                ModelState.AddModelError(string.Empty, "Email Dosn`t Exist");
			}
			return View(model);
		}

        #endregion

         #region SignOut
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
		#endregion

		#region ForgotPassword

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user); // Generats a token to reset password valid for one time  
                    // this Url.Action() generats a link to an action via Url 
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token },Request.Scheme);
                    var email = new Email()
                    {
                        Subject = "Reset Password",
                        To = user.Email,
                        Body = passwordResetLink
                    };

                    // this Function sends the Email using the dotnet built-in library smtp
                    //_emailSettings.SendEmail(email);

                    // this line sends The Email using the MailKil Library
                    _emailSettings.SendEmailUsingMailKit(email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                ModelState.AddModelError(string.Empty, "Email is not Regestered");
            }
            return View(model);
        }

        public IActionResult CheckYourInbox()
        {
            return View();
        }
        #endregion

        #region ResetPassword

        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid) 
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                   var result =  await _userManager.ResetPasswordAsync(user,token,model.NewPassword);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Login));
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

		#endregion
	}
}
