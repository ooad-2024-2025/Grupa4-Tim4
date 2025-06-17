#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using eOpcina.Models;
using Microsoft.EntityFrameworkCore;

namespace eOpcina.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<Korisnik> _userManager;

        public LoginModel(SignInManager<Korisnik> signInManager,
                          ILogger<LoginModel> logger,
                          UserManager<Korisnik> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Unesite svoj JMBG.")]
            [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora sadržavati tačno 13 cifara.")]
            [Display(Name = "JMBG")]
            public string JMBG { get; set; }

            [Required(ErrorMessage = "Unesite lozinku.")]
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; }

            [Display(Name = "Zapamti me")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Pokušaj prijave za JMBG: {JMBG}", Input.JMBG);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.JMBG == Input.JMBG);

            if (user == null)
            {
                _logger.LogWarning("Korisnik sa JMBG {JMBG} nije pronađen.", Input.JMBG);
                ModelState.AddModelError(string.Empty, "Neispravan JMBG ili lozinka.");
                return Page();
            }

            if (user.Zakljucan)
            {
                if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
                    await _userManager.UpdateAsync(user);
                }
            }

            // Možeš i dalje imati ovaj check ako želiš dodatnu poruku ili logiku
            if (user.Zakljucan)
            {
                _logger.LogWarning("Korisnički nalog za {UserName} je zaključan.", user.UserName);
                ModelState.AddModelError(string.Empty, "Vaš korisnički nalog je zaključan. Molimo kontaktirajte administratora.");
                return Page();
            }
            // OVDJE se NE koristi user.UserName — koristi se overload koji prima Korisnik objekat
            var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (user.Zakljucan)
                {
                    user.Zakljucan = false;
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                }
                await _signInManager.SignInAsync(user, isPersistent: Input.RememberMe);
                _logger.LogInformation("Korisnik {UserName} se uspješno prijavio.", user.UserName);
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                if (!user.Zakljucan)
                {
                    user.Zakljucan = true;
                    await _userManager.UpdateAsync(user);
                }
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Neispravan JMBG ili lozinka.");
            return Page();
        }
    }
}
