using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using Movie_Ticket_Booking.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }

                return View(registerVM);
            }
            // Send Email Confirmation Link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(registerVM.Email, "Movie ticket booking - Confirm Your Email!"
                , $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER_ROLE);

            return RedirectToAction(nameof(Login));


        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                ModelState.AddModelError(string.Empty, "Invalid User");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                ModelState.AddModelError(string.Empty, "Email Confirmation Failed");
            else
                ModelState.AddModelError(string.Empty, "Email Confirmed Successfully");

            return RedirectToAction(nameof(Login));
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserNameOREmail) ?? await _userManager.FindByEmailAsync(loginVM.UserNameOREmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email OR Password");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Too many attemps, try again after 5 min");
                else if (!user.EmailConfirmed)
                    ModelState.AddModelError(string.Empty, "Please Confirm Your Email First!!");
                else
                    ModelState.AddModelError(string.Empty, "Invalid User Name / Email OR Password");

                return View(loginVM);
            }

            return RedirectToAction("Index", "Movies", new { area = "Customer" });
        }


        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOREmail) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOREmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email");
                return View(resendEmailConfirmationVM);
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Already Confirmed!!");
                return View(resendEmailConfirmationVM);
            }

            // Send Confirmation Mail
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Movie ticket booking - Resend Confirm Your Email!"
                , $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            return RedirectToAction();
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOREmail) ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOREmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email");
                return View(forgetPasswordVM);
            }

            var userOTPs = await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs > 3)
            {
                ModelState.AddModelError(string.Empty, "Too Many Attemps");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString(); // 1000 - 9999

            await _applicationUserOTPRepository.AddAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                IsValid = true,
                OTP = otp,
                ValidTo = DateTime.UtcNow.AddDays(1),
            });
            await _applicationUserOTPRepository.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email!, "Movie ticket Booking - Reset Your Password"
                , $"<h1>Use This OTP: {otp} To Reset Your Account. Don't share it.</h1>");

            TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
            return RedirectToAction("ValidateOTP", new { userId = user.Id });
        }

        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM
            {
                ApplicationUserId = userId
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
                return View(validateOTPVM);

            var result = await _applicationUserOTPRepository.GetOneAsync(e => e.ApplicationUserId == validateOTPVM.ApplicationUserId && e.OTP == validateOTPVM.OTP && e.IsValid);

            if (result is null)
            {
                return RedirectToAction(nameof(ValidateOTP), new { userId = validateOTPVM.ApplicationUserId });
            }

            TempData["Success"] = "OTP verified successfully!";

            return RedirectToAction("NewPassword", new { userId = validateOTPVM.ApplicationUserId });
        }

        public async Task<IActionResult> ResendOTP(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            var userOTPs = await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs > 3)
            {
                ModelState.AddModelError(string.Empty, "Too Many Attemps");
                RedirectToAction("ValidateOTP", new { userId = user.Id });
            }

            var existingOtps = await _applicationUserOTPRepository.GetOneAsync(e => e.ApplicationUserId == userId && e.IsValid);
            
            if (existingOtps is not null)
            {
                existingOtps.IsValid = false;
                _applicationUserOTPRepository.Update(existingOtps);
            }

            

            var newOtp = new Random().Next(1000, 9999).ToString();

            var newOtpEntity = new ApplicationUserOTP
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = userId,
                OTP = newOtp,
                IsValid = true,
                CreateAt = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(1)
            };

            await _applicationUserOTPRepository.AddAsync(newOtpEntity);
            await _applicationUserOTPRepository.CommitAsync();

            // Send OTP to user's email
            await _emailSender.SendEmailAsync(
                user.Email!,
                "Your OTP Code",
                $"<h2>Your new OTP code is: <strong>{newOtp}</strong></h2>"
            );

            TempData["Info"] = "A new OTP has been sent to your email.";
            return RedirectToAction(nameof(ValidateOTP), new { userId });
        }


        public IActionResult NewPassword(string userId)
        {
            return View(new NewPasswordVM
            {
                ApplicationUserId = userId
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email");
                return View(newPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }

                return View(newPasswordVM);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Try signing in with an external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            // If the user cannot log in, try finding them by email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var username = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create a new user if they do not exist
                    Random random = new Random();
                    int r = random.Next(1000, 9999);
                    user = new ApplicationUser
                    {
                        UserName = username.Replace(" ", "") + r.ToString(),
                        Email = email,
                        EmailConfirmed = true
                    };
                    var createUserResult = await _userManager.CreateAsync(user);
                    if (!createUserResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Ensure the external login is linked
                var existingLogins = await _userManager.GetLoginsAsync(user);
                var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == info.LoginProvider);

                if (!hasGoogleLogin)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error linking external login.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? "/");
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
