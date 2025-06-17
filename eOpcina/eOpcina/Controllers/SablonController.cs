using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eOpcina.Data;
using eOpcina.Models;

namespace eOpcina.Controllers
{
    public class SablonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SablonController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sablon.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sablon = await _context.Sablon.FirstOrDefaultAsync(m => m.Id == id);
            if (sablon == null) return NotFound();

            return View(sablon);
        }

        public IActionResult Create()
        {
            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipDokumenta tipDokumenta, IFormFile pdfFile)
        {
            if (!Enum.IsDefined(typeof(TipDokumenta), tipDokumenta))
                ModelState.AddModelError("TipDokumenta", "Nevažeći tip dokumenta.");

            if (pdfFile == null || pdfFile.Length == 0)
                ModelState.AddModelError("PDFSablona", "Morate odabrati PDF fajl.");

            if (ModelState.IsValid)
            {
                using var ms = new MemoryStream();
                await pdfFile.CopyToAsync(ms);
                var sablon = new Sablon
                {
                    TipDokumenta = tipDokumenta,
                    PDFSablona = ms.ToArray()
                };
                _context.Add(sablon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sablon = await _context.Sablon.FindAsync(id);
            if (sablon == null) return NotFound();

            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });
            return View(sablon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipDokumenta")] Sablon sablon, IFormFile? pdfFile)
        {
            if (id != sablon.Id) return NotFound();

            if (!Enum.IsDefined(typeof(TipDokumenta), sablon.TipDokumenta))
                ModelState.AddModelError("TipDokumenta", "Nevažeći tip dokumenta.");

            if (ModelState.IsValid)
            {
                try
                {
                    var sablonToUpdate = await _context.Sablon.FindAsync(id);
                    if (sablonToUpdate == null) return NotFound();

                    sablonToUpdate.TipDokumenta = sablon.TipDokumenta;

                    if (pdfFile != null && pdfFile.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await pdfFile.CopyToAsync(ms);
                        sablonToUpdate.PDFSablona = ms.ToArray();
                    }

                    _context.Update(sablonToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SablonExists(sablon.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });

            return View(sablon);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sablon = await _context.Sablon.FirstOrDefaultAsync(m => m.Id == id);
            if (sablon == null) return NotFound();

            return View(sablon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sablon = await _context.Sablon.FindAsync(id);
            if (sablon != null)
            {
                _context.Sablon.Remove(sablon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SablonExists(int id)
        {
            return _context.Sablon.Any(e => e.Id == id);
        }

        //  Helper metoda
        private async Task DodajSablonIzFajlaAsync(TipDokumenta tip, string lokalnaPutanja)
        {
            byte[] pdfBytes = await System.IO.File.ReadAllBytesAsync(lokalnaPutanja);

            var sablon = new Sablon
            {
                TipDokumenta = tip,
                PDFSablona = pdfBytes
            };

            _context.Sablon.Add(sablon);
            await _context.SaveChangesAsync();
        }

        // Inicijalna ruta za unos
        [HttpGet]
        public async Task<IActionResult> InitSabloni()
        {
            string root = _env.ContentRootPath;

            await DodajSablonIzFajlaAsync(TipDokumenta.RodniList, Path.Combine(root, "wwwroot", "templates", "RodniList.pdf"));
            await DodajSablonIzFajlaAsync(TipDokumenta.SmrtniList, Path.Combine(root, "wwwroot", "templates", "SmrtnList.pdf"));
            await DodajSablonIzFajlaAsync(TipDokumenta.UpisNovorodjenceta, Path.Combine(root, "wwwroot", "templates", "UpisNovorodjenceta.pdf"));
            await DodajSablonIzFajlaAsync(TipDokumenta.Drzavljanstvo, Path.Combine(root, "wwwroot", "templates", "Drzavljanstvo.pdf"));
            await DodajSablonIzFajlaAsync(TipDokumenta.Cips, Path.Combine(root, "wwwroot", "templates", "Cips.pdf"));

            return Content("Šabloni su uspješno ubačeni.");
        }

        //  Preuzimanje PDF-a iz baze
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var sablon = await _context.Sablon.FindAsync(id);
            if (sablon == null || sablon.PDFSablona == null)
                return NotFound();

            return File(sablon.PDFSablona, "application/pdf", $"{sablon.TipDokumenta}.pdf");
        }
    }
}
