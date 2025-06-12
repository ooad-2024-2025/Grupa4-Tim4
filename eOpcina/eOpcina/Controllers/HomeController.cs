using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace eOpcina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
      
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home/Profil
        public async Task<IActionResult> Profil()
        {
            var email = User.Identity.Name;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index"); // ili neka stranica za prijavu

            var korisnik = await _context.Korisnik.FirstOrDefaultAsync(k => k.Email == email);
            if (korisnik == null)
                return NotFound("Korisnik nije pronaðen.");

            return View(korisnik); // prosleðujemo model korisnika View-u
        }


        // GET: Home/HistorijaZahtjeva
        public async Task<IActionResult> PrikaziHistorijuZahtjeva(
     string sortOrder,           // "datum" ili "dokument", default po datumu
     int? tipDokumentaFilter,    // filter po tipu dokumenta (nullable)
     DateTime? fromDate,         // filter od datuma
     DateTime? toDate            // filter do datuma
 )
        {
            var email = User.Identity.Name;

            var korisnik = await _context.Korisnik.FirstOrDefaultAsync(k => k.Email == email);
            if (korisnik == null)
                return NotFound("Korisnik nije pronaðen.");

            DateTime godinaUnazad = DateTime.Now.AddYears(-1);

            // Poèetni query za zahtjeve korisnika unazad godinu dana
            var query = _context.Zahtjev
                .Include(z => z.Dokument)
                    .ThenInclude(d => d.Sablon)  // ukljuèi Sablon jer nam treba TipDokumenta
                .Where(z => z.IdKorisnika == korisnik.Id &&
                            z.DatumSlanja >= godinaUnazad)
                .AsQueryable();

            // Filter po tipu dokumenta ako je zadano
            if (tipDokumentaFilter.HasValue)
            {
                query = query.Where(z => z.Dokument != null
                                         && z.Dokument.Sablon != null
                                         && (int)z.Dokument.Sablon.TipDokumenta == tipDokumentaFilter.Value);
            }

            // Filter po datumu slanja od
            if (fromDate.HasValue)
            {
                query = query.Where(z => z.DatumSlanja >= fromDate.Value);
            }

            // Filter po datumu slanja do
            if (toDate.HasValue)
            {
                query = query.Where(z => z.DatumSlanja <= toDate.Value);
            }

            // Sortiranje
            switch (sortOrder)
            {
                case "dokument":
                    query = query.OrderBy(z => z.Dokument.Sablon.TipDokumenta);
                    break;
                case "datum_desc":
                    query = query.OrderByDescending(z => z.DatumSlanja);
                    break;
                case "datum_asc":
                    query = query.OrderBy(z => z.DatumSlanja);
                    break;
                default:
                    query = query.OrderByDescending(z => z.DatumSlanja);
                    break;
            }

            var zahtjevi = await query.ToListAsync();

            // Predaj ViewData sa filterima i sortiranjem za View
            ViewData["SortOrder"] = sortOrder;
            ViewData["TipDokumentaFilter"] = tipDokumentaFilter;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            return View(zahtjevi);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
