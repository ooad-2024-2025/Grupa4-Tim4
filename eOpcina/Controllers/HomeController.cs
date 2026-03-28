using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Korisnik> _userManager;


        public HomeController(
             ILogger<HomeController> logger,
             ApplicationDbContext context,
             UserManager<Korisnik> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: Home/Profil
        public async Task<IActionResult> Profil()
        {
            // Dohvati trenutno ulogovanog korisnika koristeci UserManager
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
                return RedirectToAction("Index"); // ili stranica za prijavu

            // Opcionalno: ako želiš dodatno podatke iz baze ukljuciti
            var korisnikDetalji = await _context.Korisnik       
                .FirstOrDefaultAsync(k => k.Id == korisnik.Id);

            if (korisnikDetalji == null)
                return NotFound("Korisnik nije pronađen.");

            return View("Profil", korisnikDetalji); // koristi odgovarajuci View
        }

        // GET: Home/HistorijaZahtjeva
        public async Task<IActionResult> PrikaziHistorijuZahtjeva(
            string sortOrder,
            int? tipDokumentaFilter,
            DateTime? fromDate,
            DateTime? toDate)
        {
            // Dobavi ID trenutno ulogovanog korisnika
            var korisnikId = _userManager.GetUserId(User);
            if (korisnikId == null)
                return NotFound("Korisnik nije pronađen.");

            DateTime godinaUnazad = DateTime.Now.AddYears(-1);

            var query = _context.Zahtjev
                .Include(z => z.Dokument)
                    .ThenInclude(d => d.Sablon)
                .Where(z => z.IdKorisnika == korisnikId &&
                            z.DatumSlanja >= godinaUnazad)
                .AsQueryable();

            if (tipDokumentaFilter.HasValue)
            {
                query = query.Where(z =>
                    z.Dokument.Sablon.TipDokumenta == (TipDokumenta)tipDokumentaFilter.Value);
            }

            if (fromDate.HasValue)
                query = query.Where(z => z.DatumSlanja >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(z => z.DatumSlanja <= toDate.Value);

            // Sortiranje
            query = sortOrder switch
            {
                "dokument" => query.OrderBy(z => z.Dokument.Sablon.TipDokumenta),
                "datum_asc" => query.OrderBy(z => z.DatumSlanja),
                _ => query.OrderByDescending(z => z.DatumSlanja),
            };

            var zahtjevi = await query.ToListAsync();

            ViewData["SortOrder"] = sortOrder;
            ViewData["TipDokumentaFilter"] = tipDokumentaFilter;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            return View("PrikaziHistorijuZahtjeva", zahtjevi);
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
