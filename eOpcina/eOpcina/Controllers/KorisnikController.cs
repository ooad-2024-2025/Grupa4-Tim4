using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var korisnici = await _userManager.Users.ToListAsync();
            return View(korisnici);
        }

        // POST: Korisnik/SnimiSve
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult Dodaj()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Korisnik korisnik, string Password)
        {
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
        public async Task<IActionResult> Uredi(string id)
        {
            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            return View("Uredi", korisnik);
        }

        // POST: Korisnik/Uredi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Korisnik korisnik)
        {
            if (!ModelState.IsValid)
            {
                return View(korisnik);
            }

            var existing = await _userManager.FindByIdAsync(korisnik.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Ime = korisnik.Ime;
            existing.Prezime = korisnik.Prezime;
            existing.Email = korisnik.Email;
            existing.UserName = korisnik.UserName;
            existing.JMBG = korisnik.JMBG;
            existing.BrojLicneKarte = korisnik.BrojLicneKarte;
            existing.AdresaPrebivalista = korisnik.AdresaPrebivalista;
            existing.RokTrajanjaLicneKarte = korisnik.RokTrajanjaLicneKarte;
            existing.ElektronskiPotpis = korisnik.ElektronskiPotpis;
            existing.Spol = korisnik.Spol;
            existing.EmailConfirmed = true;

            // Sinhronizacija zaključavanja korisnika sa Identity lockout-om
            existing.Zakljucan = korisnik.Zakljucan;
            if (korisnik.Zakljucan)
            {
                existing.LockoutEnabled = true;
                existing.LockoutEnd = DateTimeOffset.MaxValue; // zaključaj do daljnjeg
            }
            else
            {
                existing.LockoutEnd = null; // otključaj
            }

            var result = await _userManager.UpdateAsync(existing);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(korisnik);
            }

            TempData["Success"] = $"Uspješno ažuriran korisnik {existing.Ime} {existing.Prezime}.";
            return RedirectToAction("Pretrazi");
        }


        // GET: Korisnik/Pretrazi
        public IActionResult Pretrazi()
        {
            return View();
        }

        // POST: Korisnik/Pretrazi
        [HttpPost]
        public async Task<IActionResult> Pretrazi(string query)
        {
            var users = await _userManager.Users
                .Where(k => (k.Ime + " " + k.Prezime).ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return View("Pretrazi", users);
        }
    }
}
