using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eOpcina.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly ApplicationDbContext _context;


        public KorisnikController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Korisnik/Index
        public async Task<IActionResult> Index()
        {
            var korisnici = await _context.Korisnik.ToListAsync();
            return View(korisnici);
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

        // GET: Korisnik/Uredi/{id}
        public async Task<IActionResult> Uredi(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var korisnik = await _context.Korisnik.FindAsync(id);
            if (korisnik == null)
                return NotFound();

            return View(korisnik);
        }

        // POST: Korisnik/Uredi/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(string id, Korisnik korisnik)
        {
            if (id != korisnik.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(korisnik);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorisnikExists(korisnik.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(korisnik);
        }

        private bool KorisnikExists(string id)
        {
            return _context.Korisnik.Any(e => e.Id == id);
        }
    }
}
