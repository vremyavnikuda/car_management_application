using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using document_management.Models;
using document_management.Models.ViewModels;
using document_management.Services;
using System.Security.Claims;

namespace document_management.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILoggingService _loggingService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggingService loggingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _loggingService = loggingService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _loggingService.LogUserAction("anonymous", "LoginPageAccess", "User accessed login page");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                _loggingService.LogUserAction("anonymous", "LoginAttempt", $"Login attempt for email: {model.Email}");

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email ?? throw new ArgumentNullException(nameof(model.Email)),
                    model.Password ?? throw new ArgumentNullException(nameof(model.Password)),
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    _loggingService.LogSecurityEvent(user.Id, "LoginSuccess", $"User successfully logged in: {model.Email}");
                    return RedirectToLocal(returnUrl);
                }

                _loggingService.LogSecurityEvent("anonymous", "LoginFailed", $"Failed login attempt for email: {model.Email}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _loggingService.LogUserAction("anonymous", "RegisterPageAccess", "User accessed registration page");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                _loggingService.LogUserAction("anonymous", "RegisterAttempt", $"Registration attempt for email: {model.Email}");

                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(
                    user,
                    model.Password ?? throw new ArgumentNullException(nameof(model.Password)));

                if (result.Succeeded)
                {
                    _loggingService.LogSecurityEvent(user.Id, "RegisterSuccess", 
                        $"New user registered: {model.Email}");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _loggingService.LogSecurityEvent(user.Id, "AutoLogin", 
                        $"User automatically logged in after registration: {model.Email}");

                    return RedirectToLocal(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _loggingService.LogSecurityEvent("anonymous", "RegisterError", 
                        $"Registration error for {model.Email}: {error.Description}");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            await _signInManager.SignOutAsync();
            _loggingService.LogSecurityEvent(userId, "Logout", $"User logged out: {userEmail}");
            
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
} 