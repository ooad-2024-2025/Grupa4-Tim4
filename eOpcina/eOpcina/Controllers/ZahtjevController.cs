using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eOpcina.Data;
using eOpcina.Models;
using System.Net.Mail;
using System.Net;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using System.IO;
using eOpcina.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace eOpcina.Controllers
{
    [Authorize]
    public class ZahtjevController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<Korisnik> _userManager;

        public ZahtjevController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Zahtjev
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Index(string stanjeFilter)
        {
            var zahtjeviQuery = _context.Zahtjev
                .Include(z => z.Dokument)
                    .ThenInclude(d => d.Sablon)
                .Include(z => z.Korisnik)
                .AsQueryable();

            if (!string.IsNullOrEmpty(stanjeFilter))
            {
                if (Enum.TryParse<StanjeZahtjeva>(stanjeFilter, out var parsedStanje))
                {
                    zahtjeviQuery = zahtjeviQuery.Where(z => z.StanjeZahtjeva == parsedStanje);
                }
            }

            ViewData["StanjeFilter"] = stanjeFilter;

            var zahtjevi = await zahtjeviQuery.ToListAsync();
            return View(zahtjevi);
        }


        // GET: Zahtjev/Details/5
        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev
                .Include(z => z.Dokument)
                .Include(z => z.Korisnik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zahtjev == null)
            {
                return NotFound();
            }

            return View(zahtjev);
        }

        // GET: Zahtjev/Create
        [Authorize(Roles = "Korisnik,Zaposlenik")]
        public IActionResult Create()
        {
            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });

            ViewBag.RazloziZahtjeva = Enum.GetValues(typeof(Razlog))
                .Cast<Razlog>()
                .Select(r => new SelectListItem
                {
                    Value = ((int)r).ToString(),
                    Text = r.ToString()
                });

            return View(new ZahtjevCreateViewModel());
        }

        // POST: Zahtjev/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Korisnik,Zaposlenik")]
        public async Task<IActionResult> Create(ZahtjevCreateViewModel viewModel)
        {
            if (!Enum.IsDefined(typeof(TipDokumenta), viewModel.TipDokumenta))
                ModelState.AddModelError("TipDokumenta", "Nevažeći tip dokumenta.");

            if (!Enum.IsDefined(typeof(Razlog), viewModel.RazlogZahtjeva))
                ModelState.AddModelError("RazlogZahtjeva", "Nevažeći razlog zahtjeva.");

            if (viewModel.NacinPreuzimanja == null)
                ModelState.AddModelError("NacinPreuzimanja", "Molimo odaberite način preuzimanja dokumenta.");

            if (string.IsNullOrWhiteSpace(viewModel.ElektronskiPotpis))
                ModelState.AddModelError("ElektronskiPotpis", "Elektronski potpis je obavezan.");
            else
            {
                var userId = _userManager.GetUserId(User);
                var user = await _context.Korisnik.FindAsync(userId);
                if (user == null || user.ElektronskiPotpis != viewModel.ElektronskiPotpis)
                {
                    ModelState.AddModelError("ElektronskiPotpis", "Pogrešan elektronski potpis. Pokušajte ponovo.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                    .Cast<TipDokumenta>()
                    .Select(t => new SelectListItem
                    {
                        Value = ((int)t).ToString(),
                        Text = t.ToString()
                    });

                ViewBag.RazloziZahtjeva = Enum.GetValues(typeof(Razlog))
                    .Cast<Razlog>()
                    .Select(r => new SelectListItem
                    {
                        Value = ((int)r).ToString(),
                        Text = r.ToString()
                    });

                return View(viewModel);
            }

            var idKorisnika = _userManager.GetUserId(User);
            return await ObradiZahtjev(idKorisnika, viewModel.TipDokumenta, viewModel.RazlogZahtjeva, viewModel.NacinPreuzimanja);
        }



        // GET: Zahtjev/Edit/5
        [Authorize(Roles = "Zaposlenik")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var zahtjev = await _context.Zahtjev.FindAsync(id);
            if (zahtjev == null) return NotFound();

            if (zahtjev.NacinPreuzimanja != NacinPreuzimanja.Licno)
            {
                // You could optionally redirect or show error if editing isn't allowed
                TempData["Error"] = "Samo zahtjevi za lično preuzimanje se mogu uređivati.";
                return RedirectToAction(nameof(Index));
            }

            var allStates = Enum.GetValues(typeof(StanjeZahtjeva)).Cast<StanjeZahtjeva>();
            var allowed = allStates.Where(s => s != StanjeZahtjeva.Obradjen);

            ViewData["StanjeZahtjevaItems"] = new SelectList(
                allowed.Select(s => new {
                    Value = (int)s,
                    Text = s.GetType()
                            .GetMember(s.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? s.ToString()
                }),
                "Value", "Text",
                (int)zahtjev.StanjeZahtjeva
            );

            return View(zahtjev);
        }

        // POST: Zahtjev/Edit/5
        [Authorize(Roles = "Zaposlenik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StanjeZahtjeva")] Zahtjev input)
        {
            ModelState.Remove(nameof(input.IdKorisnika));
            ModelState.Remove("Korisnik");
            ModelState.Remove("Dokument");

            if (id != input.Id) return NotFound();

            var existing = await _context.Zahtjev.FindAsync(id);
            if (existing == null) return NotFound();

            var allStates = Enum.GetValues(typeof(StanjeZahtjeva)).Cast<StanjeZahtjeva>();
            var allowed = existing.NacinPreuzimanja == NacinPreuzimanja.Licno
                ? allStates.Where(s => s != StanjeZahtjeva.Obradjen)
                : Enumerable.Empty<StanjeZahtjeva>();

            ViewData["StanjeZahtjevaItems"] = new SelectList(
                allowed.Select(s => new {
                    Value = (int)s,
                    Text = s.GetType()
                            .GetMember(s.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? s.ToString()
                }),
                "Value", "Text",
                (int)input.StanjeZahtjeva
            );

            if (!ModelState.IsValid)
                return View(existing);

            if (!allowed.Contains(input.StanjeZahtjeva))
            {
                ModelState.AddModelError("StanjeZahtjeva", "Odabrano stanje nije dozvoljeno za ovaj zahtjev.");
                return View(existing);
            }

            existing.StanjeZahtjeva = input.StanjeZahtjeva;
            _context.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Zahtjev/Delete/5

        [Authorize(Roles = "Zaposlenik,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev
                .Include(z => z.Dokument)
                .Include(z => z.Korisnik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zahtjev == null)
            {
                return NotFound();
            }

            return View(zahtjev);
        }

        // POST: Zahtjev/Delete/5
        [Authorize(Roles = "Zaposlenik,Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zahtjev = await _context.Zahtjev.FindAsync(id);
            if (zahtjev != null)
            {
                _context.Zahtjev.Remove(zahtjev);
                _context.Dokument.Remove(zahtjev.Dokument); // Također brišemo dokument vezan uz zahtjev
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZahtjevExists(int id)
        {
            return _context.Zahtjev.Any(e => e.Id == id);
        }
        
        
        private async Task<bool> ProvjeriUsloveAsync(string idKorisnika)
        {
            var korisnik = await _context.Korisnik.FindAsync(idKorisnika);
            if (korisnik == null)
                return false;
            if (korisnik.RokTrajanjaLicneKarte < DateTime.Now)
                return false;
            return true;
        }


        private async Task PosaljiPDFEmail(string emailKorisnika, byte[] pdfBytes, string subject, string body)
        {
            var fromAddress = new MailAddress("eopcina@gmail.com", "eOpcina");
            var toAddress = new MailAddress(emailKorisnika);
            const string fromPassword = "whxbfafujcewcjpp";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                // Create attachment from byte array
                message.Attachments.Add(new Attachment(
                    new MemoryStream(pdfBytes),
                    "dokument.pdf",
                    "application/pdf"
                ));

                await smtp.SendMailAsync(message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> 
            ObradiZahtjev(string idKorisnika, TipDokumenta tipDokumenta, Razlog razlog, NacinPreuzimanja? nacinPreuzimanja)
        {
            if (!Enum.IsDefined(typeof(TipDokumenta), tipDokumenta) || !Enum.IsDefined(typeof(Razlog), razlog))
                return BadRequest("Nevažeći tip dokumenta ili razlog zahtjeva.");

            if (nacinPreuzimanja == null)
                // Jedna od dvije opcije mora biti odabrana
                return BadRequest("Molimo odaberite način preuzimanja dokumenta.");

            if (idKorisnika == null)
                return BadRequest("Korisnik nije ulogovan.");

            var korisnik = await _context.Korisnik.FindAsync(idKorisnika);
            if (korisnik == null)
                return NotFound("Korisnik nije pronađen.");

            if (!await ProvjeriUsloveAsync(idKorisnika))
                return BadRequest("Korisnik ne ispunjava uslove za obradu zahtjeva.");

            var datumSlanja = DateTime.Now;

            var sablon = await _context.Sablon
                .FirstOrDefaultAsync(s => s.TipDokumenta == tipDokumenta);
            if (sablon == null)
                return NotFound("Šablon za traženi tip dokumenta nije pronađen.");

            byte[] sablonPDF = sablon.PDFSablona;
            byte[] popunjeniPDF = System.IO.File.ReadAllBytes(@"C:\ETF\CetvrtiSemestar\US\UputaZaIzvjestaj.pdf");
            var datumIzdavanja = DateTime.Now;

            /*
            using (var templateStream = new MemoryStream(sablonPDF))
            using (var outputStream = new MemoryStream())
            {
                var pdfReader = new PdfReader(templateStream);
                var pdfWriter = new PdfWriter(outputStream);
                var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                var fields = form.GetFormFields();

                // Fill the fields
                fields["Ime"].SetValue(korisnik.Ime);
                fields["Prezime"].SetValue(korisnik.Prezime);
                fields["Datum"].SetValue(datumIzdavanja.ToShortDateString());

                form.FlattenFields(); // Optional: makes the fields uneditable

                pdfDoc.Close();
                popunjeniPDF = outputStream.ToArray();
            }
            */

            var dokument = new Dokument
            {
                DatumIzdavanja = DateTime.Now,
                RokTrajanja = int.MaxValue,
                IdSablona = sablon.Id,                      
                PDFDokumenta = popunjeniPDF
            };
            _context.Dokument.Add(dokument);
            await _context.SaveChangesAsync();

            var zahtjev = new Zahtjev
            {
                IdKorisnika = idKorisnika.ToString(),
                IdDokumenta = dokument.Id,
                RazlogZahtjeva = razlog,
                DatumSlanja = datumSlanja,
                NacinPreuzimanja = nacinPreuzimanja ?? NacinPreuzimanja.PrekoMaila,
                StanjeZahtjeva = StanjeZahtjeva.Poslan
            };
            _context.Zahtjev.Add(zahtjev);
            await _context.SaveChangesAsync();

            var tipDokumentaNormalized = tipDokumenta.GetType()
                        .GetMember(tipDokumenta.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()?.Name ?? tipDokumenta.ToString();

            
            if (nacinPreuzimanja == NacinPreuzimanja.PrekoMaila)
            {
                // Korisnik će primiti dokument putem emaila
                await PosaljiPDFEmail(
                    emailKorisnika: korisnik.Email,
                    pdfBytes: popunjeniPDF,
                    subject: "Dokument je spreman",
                    body: $"Poštovani,\nVaš zahtjev je obrađen. U prilogu se nalazi dokument: {tipDokumentaNormalized} za zahtjev poslan " +
                          $"{datumSlanja.ToString("dd.MM.yyyy. HH:mm")}.\nS poštovanjem,\neOpcina Team"
                );
                // Promijeni stanje zahtjeva na "Obrađen"
                zahtjev.StanjeZahtjeva = StanjeZahtjeva.Obradjen;
                _context.Zahtjev.Update(zahtjev);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Korisnik će preuzeti dokument u općini
                TempData["Success"] = "Vaš zahtjev je spremljen i dokument će biti dostupan za preuzimanje u općini.";
            }

            return RedirectToAction("PrikaziHistorijuZahtjeva", "Home");
        }
    }
}
