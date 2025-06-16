#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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

            [Display(Name = "Remember me?")]
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

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Pokušaj prijave za JMBG: {JMBG}", Input.JMBG);

                // Pronađi korisnika po JMBG
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.JMBG == Input.JMBG);
                if (user == null)
                {
                    _logger.LogWarning("Korisnik sa JMBG {JMBG} nije pronađen.", Input.JMBG);
                    ModelState.AddModelError(string.Empty, "Neispravan JMBG ili lozinka.");
                    return Page();
                }

                _logger.LogInformation("Korisnik pronađen: {UserName}", user.UserName);

                // Provjera je li korisnik zaključan ručno
                if (user.Zakljucan)
                {
                    _logger.LogWarning("Korisnički nalog za {UserName} je zaključan.", user.UserName);
                    ModelState.AddModelError(string.Empty, "Vaš korisnički nalog je zaključan. Molimo kontaktirajte administratora.");
                    return Page();
                }

<<<<<<< HEAD
                // Login - koristi UserName za PasswordSignInAsync
=======
                /*// Login sa automatskom ASP.NET lockout podrškom
>>>>>>> bb01b9e19fefee7cb7aa6d5751b1a46a7ee48d65
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation("Korisnik {UserName} se uspješno prijavio.", user.UserName);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    if (result.RequiresTwoFactor)
                    {
                        _logger.LogInformation("Korisnik {UserName} zahtijeva 2FA.", user.UserName);
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Korisnički nalog {UserName} je zaključan zbog previše pokušaja.", user.UserName);
                        return RedirectToPage("./Lockout");
                    }

                    _logger.LogWarning("Neuspješan pokušaj prijave za korisnika {UserName}. Lozinka nije ispravna ili drugi problem.", user.UserName);
                    ModelState.AddModelError(string.Empty, "Neispravan JMBG ili lozinka.");
                    return Page();
                }
<<<<<<< HEAD
            }
            else
            {
                // Ispis svih validacijskih grešaka
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string sveGreske = string.Join("; ", errors);
                _logger.LogWarning("Validacija forme nije prošla. Greške: {Errors}", sveGreske);
=======

                ModelState.AddModelError(string.Empty, "Neispravan JMBG ili lozinka.");
                return Page();*/
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
>>>>>>> bb01b9e19fefee7cb7aa6d5751b1a46a7ee48d65
            }

            // Validacija nije prošla
            return Page();
        }

    }
}
