using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace eOpcina.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public KorisnikController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Korisnik/Index
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Index()
        {
            var korisnici = await _userManager.Users.ToListAsync();
            return View(korisnici);
        }

        // POST: Korisnik/SnimiSve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> SnimiSve(List<Korisnik> korisnici)
        {
            if (ModelState.IsValid)
            {
                foreach (var k in korisnici)
                {
                    var korisnikUBazi = await _userManager.FindByIdAsync(k.Id);
                    if (korisnikUBazi != null)
                    {
                        korisnikUBazi.Ime = k.Ime;
                        korisnikUBazi.Prezime = k.Prezime;
                        korisnikUBazi.Email = k.Email;
                        korisnikUBazi.UserName = k.UserName;
                        korisnikUBazi.JMBG = k.JMBG;

                        // Sinhronizacija zaključavanja korisnika sa Identity lockout-om
                        korisnikUBazi.Zakljucan = k.Zakljucan;
                        if (k.Zakljucan)
                        {
                            korisnikUBazi.LockoutEnabled = true;
                            korisnikUBazi.LockoutEnd = DateTimeOffset.MaxValue; // zaključaj do daljnjeg
                        }
                        else
                        {
                            korisnikUBazi.LockoutEnd = null; // otključaj
                        }

                        var updateResult = await _userManager.UpdateAsync(korisnikUBazi);
                        if (!updateResult.Succeeded)
                        {
                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View("Index", korisnici);
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Index", korisnici);
        }


        // GET: Korisnik/Dodaj
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public IActionResult Dodaj()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Dodaj(Korisnik korisnik, string Password)
        {
            // Ako neki od uslova nije zadovoljen, vrati View
            if (!ProvjeriUslove(korisnik, Password))
            {
                Debug.WriteLine("[DEBUG] Provjera uslova nije prošla. Vraćam korisnika na formu za unos.");
                return View(korisnik);
            }

            if (ModelState.IsValid)
            {
                // Postavi UserName na JMBG prije spremanja korisnika
                korisnik.UserName = korisnik.JMBG;

                var result = await _userManager.CreateAsync(korisnik, Password);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Uspješno je dodan Korisnik {korisnik.Ime} {korisnik.Prezime} sa JMBG {korisnik.JMBG}";
                    return RedirectToAction("Dodaj");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(korisnik);
        }


        // GET: Korisnik/Uredi/5 
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Uredi(string id)
        {
            Debug.WriteLine($"[DEBUG] Pozvan GET Uredi sa id = {id}");

            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null)
            {
                Debug.WriteLine("[DEBUG] Korisnik nije pronađen.");
                return NotFound();
            }

            // Postavi Zakljucan status na osnovu LockoutEnd
            korisnik.Zakljucan = korisnik.LockoutEnd.HasValue && korisnik.LockoutEnd > DateTimeOffset.UtcNow;
            Debug.WriteLine($"[DEBUG] Korisnik Zakljucan status: {korisnik.Zakljucan}");
            Debug.WriteLine($"[DEBUG] LockoutEnabled: {korisnik.LockoutEnabled}, LockoutEnd: {korisnik.LockoutEnd}");

            return View("Uredi", korisnik);
        }

        [HttpPost]
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Uredi(Korisnik korisnik)
        {
            Debug.WriteLine("[DEBUG] Pozvan POST Uredi.");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("[DEBUG] ModelState nije validan. Detalji:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"[DEBUG] Polje '{state.Key}': {error.ErrorMessage}");
                    }
                }
                return View(korisnik);
            }

            var existing = await _userManager.FindByIdAsync(korisnik.Id);
            if (existing == null)
            {
                Debug.WriteLine($"[DEBUG] Ne postoji korisnik s Id: {korisnik.Id}");
                return NotFound();
            }

            Debug.WriteLine($"[DEBUG] Prije izmjene: existing.Zakljucan = {existing.Zakljucan}");
            Debug.WriteLine($"[DEBUG] Iz forme: korisnik.Zakljucan = {korisnik.Zakljucan}");

            // Ažuriraj polja osim Zakljucan (zakljucan će biti posebno)
            existing.Ime = korisnik.Ime;
            existing.Prezime = korisnik.Prezime;
            existing.Email = korisnik.Email;
            existing.JMBG = korisnik.JMBG;
            existing.BrojLicneKarte = korisnik.BrojLicneKarte;
            existing.AdresaPrebivalista = korisnik.AdresaPrebivalista;
            existing.RokTrajanjaLicneKarte = korisnik.RokTrajanjaLicneKarte;
            existing.ElektronskiPotpis = korisnik.ElektronskiPotpis;
            existing.Spol = korisnik.Spol;

            // Update Zakljucan
            existing.Zakljucan = korisnik.Zakljucan;
            Debug.WriteLine($"[DEBUG] Setujem existing.Zakljucan na {existing.Zakljucan}");

            // Postavi lockout polja prema Zakljucan
            if (korisnik.Zakljucan)
            {
                existing.LockoutEnabled = true;
                var lockoutResult = await _userManager.SetLockoutEndDateAsync(existing, DateTimeOffset.MaxValue);
                if (lockoutResult.Succeeded)
                {
                    existing.LockoutEnabled = true;
                    Debug.WriteLine("[DEBUG] Korisnik zaključan - LockoutEnd postavljen na MaxValue i LockoutEnabled = true");
                }
                else
                {
                    Debug.WriteLine("[ERROR] Greške prilikom zaključavanja korisnika:");
                    foreach (var err in lockoutResult.Errors)
                        Debug.WriteLine($"[ERROR] {err.Description}");
                }
            }
            else
            {
                existing.LockoutEnabled = true;
                var lockoutResult = await _userManager.SetLockoutEndDateAsync(existing, null);
                if (lockoutResult.Succeeded)
                {
                    existing.LockoutEnabled = false;
                    Debug.WriteLine("[DEBUG] Korisnik otključan - LockoutEnd postavljen na null i LockoutEnabled = false");
                }
                else
                {
                    Debug.WriteLine("[ERROR] Greške prilikom otključavanja korisnika:");
                    foreach (var err in lockoutResult.Errors)
                        Debug.WriteLine($"[ERROR] {err.Description}");
                }
            }

            var result = await _userManager.UpdateAsync(existing);

            if (result.Succeeded)
            {
                Debug.WriteLine("[DEBUG] Uspješno spremljene promjene korisnika.");
                TempData["Success"] = "Korisnik uspješno uređen!";
                return RedirectToAction("Pretrazi");
            }
            else
            {
                Debug.WriteLine("[ERROR] Greške prilikom spremanja korisnika:");
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine($"[ERROR] {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(korisnik);
        }

        // GET: Korisnik/Pretrazi
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public IActionResult Pretrazi()
        {
            return View();
        }

        // POST: Korisnik/Pretrazi
        [HttpPost]
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Pretrazi(string query)
        {
            var users = await _userManager.Users
                .Where(k => (k.Ime + " " + k.Prezime).ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return View("Pretrazi", users);
        }

        private bool ProvjeriUslove(Korisnik korisnik, string Password = "DefaultLozinka1")
        {
            // ModelState mora biti validan
            if (!ModelState.IsValid)
            {
                return false;
            }

            // Email mora biti u ispravnom formatu
            try
            {
                var addr = new System.Net.Mail.MailAddress(korisnik.Email);
                if(addr.Address != korisnik.Email)
                {
                    ModelState.AddModelError("Email", "Email adresa nije u ispravnom formatu.");
                    return false;
                }
            }
            catch
            {
                return false;
            }

            // Password mora biti između 6 i 30 znakova, mora imati barem jednu cifru, jedno veliko slovo i jedno malo slovo
            if (Password.Length < 6 || Password.Length > 18  || 
                !Password.Any(char.IsDigit) || !Password.Any(char.IsUpper) || !Password.Any(char.IsLower))
            {
                ModelState.AddModelError("Password", "Lozinka mora biti između 6 i 18 znakova, i mora sadržavati barem jednu cifru, jedno veliko slovo i jedno malo slovo.");
                return false;
            }

            // JMBG mora biti 13 cifara
            if (korisnik.JMBG.Length != 13 || !korisnik.JMBG.All(char.IsDigit))
            {
                ModelState.AddModelError("JMBG", "JMBG mora sadržavati tačno 13 cifara.");
                return false;
            }

            // Datum rođenja koji se dobija iz JMBG mora biti validan i korisnik mora biti stariji od 18 godina
            DateTime datumRodjenja;
            try
            {
                int dan = int.Parse(korisnik.JMBG.Substring(0, 2));
                int mjesec = int.Parse(korisnik.JMBG.Substring(2, 2));
                int godina = int.Parse(korisnik.JMBG.Substring(4, 3)) + 1000; // JMBG koristi 3 cifre za godinu

                // Provjeri da li su dan, mjesec i godina unutar validnih granica
                if (godina < 1000 || godina > DateTime.Now.Year || mjesec < 1 || mjesec > 12 ||
                    dan < 1 || dan > DateTime.DaysInMonth(godina, mjesec))
                {
                    ModelState.AddModelError("JMBG", "Neispravan JMBG");
                    return false;
                }

                // Provjeri da li je korisnik stariji od 18 godina
                datumRodjenja = new DateTime(godina, mjesec, dan);
                if (DateTime.Now.Year - datumRodjenja.Year < 18 || 
                   (DateTime.Now.Year - datumRodjenja.Year == 18 && 
                   (DateTime.Now.Month < datumRodjenja.Month || 
                   (DateTime.Now.Month == datumRodjenja.Month && DateTime.Now.Day < datumRodjenja.Day))))
                {
                    ModelState.AddModelError("JMBG", "Korisnik mora biti stariji od 18 godina.");
                    return false;
                }
            }
            catch
            {
                ModelState.AddModelError("JMBG", "Neispravan JMBG.");
                return false;
            }

            // Elektronski potpis mora imati tačno 10 znakova
            if (korisnik.ElektronskiPotpis.Length != 10)
            {
                ModelState.AddModelError("ElektronskiPotpis", "Elektronski potpis mora sadržavati najmanje 8 znakova.");
                return false;
            }
            
            // Broj lične karte mora biti tačno 9 znakova
            if (korisnik.BrojLicneKarte.Length != 9)
            {
                ModelState.AddModelError("BrojLicneKarte", "Broj lične karte mora sadržavati tačno 9 znakova.");
                return false;
            }

            // Rok trajanja lične karte mora biti u budućnosti
            if (korisnik.RokTrajanjaLicneKarte <= DateTime.Now)
            {
                ModelState.AddModelError("RokTrajanjaLicneKarte", "Rok trajanja lične karte mora biti u budućnosti.");
                return false;
            }

            return true;
        }
    }
}
