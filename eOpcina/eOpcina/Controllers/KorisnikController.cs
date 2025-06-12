using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                    // Možeš dodatno provjeriti postoji li korisnik u bazi prije update-a
                    var korisnikUBazi = await _context.Korisnik.FindAsync(k.Id);
                    if (korisnikUBazi != null)
                    {
                        // Update relevantnih polja, nemoj direktno update-ati cijeli objekt zbog tracking-a
                        korisnikUBazi.Ime = k.Ime;
                        korisnikUBazi.Prezime = k.Prezime;
                        korisnikUBazi.Email = k.Email;
                        korisnikUBazi.JMBG = k.JMBG;
                        korisnikUBazi.UserName = k.UserName;
                        // Dodaj ostala polja ako treba
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Ako model nije validan, vrati listu korisnika da se može ponovo urediti
            return View("Index", korisnici);
        }


        // GET: Korisnik/Dodaj
        public IActionResult Dodaj()
        {
            return View();
        }

        // POST: Korisnik/Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(korisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(korisnik);
        }

        // GET: Korisnik/Uredi
        public async Task<IActionResult> Uredi()
        {
            var korisnici = await _userManager.Users.ToListAsync();
            return View(korisnici);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(List<Korisnik> korisnici)
        {
            if (!ModelState.IsValid)
            {
                return View(korisnici);
            }

            foreach (var korisnik in korisnici)
            {
                var existingUser = await _userManager.FindByIdAsync(korisnik.Id);
                if (existingUser != null)
                {
                    existingUser.Ime = korisnik.Ime;
                    existingUser.Prezime = korisnik.Prezime;
                    existingUser.Email = korisnik.Email;
                    existingUser.JMBG = korisnik.JMBG;
                    // Ostala polja ako treba

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(korisnici);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Korisnik nije pronađen.");
                    return View(korisnici);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
