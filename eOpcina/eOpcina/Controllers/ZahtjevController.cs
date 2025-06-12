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

namespace eOpcina.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZahtjevController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Zahtjev
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Zahtjev.Include(z => z.Dokument).Include(z => z.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Zahtjev/Details/5
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
        public IActionResult Create()
        {
            ViewBag.TipoviDokumenata = Enum.GetValues(typeof(TipDokumenta))
                .Cast<TipDokumenta>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });

            ViewBag.Razlozi = Enum.GetValues(typeof(Razlog))
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
        public async Task<IActionResult> Create(ZahtjevCreateViewModel viewModel)
        {
            if (!Enum.IsDefined(typeof(TipDokumenta), viewModel.TipDokumenta))
            {
                ModelState.AddModelError("TipDokumenta", "Nevažeći tip dokumenta.");
            }

            if (!Enum.IsDefined(typeof(Razlog), viewModel.RazlogZahtjeva))
            {
                ModelState.AddModelError("RazlogZahtjeva", "Nevažeći razlog zahtjeva");
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
                ViewBag.Razlozi = Enum.GetValues(typeof(Razlog))
                    .Cast<Razlog>()
                    .Select(r => new SelectListItem
                    {
                        Value = ((int)r).ToString(),
                        Text = r.ToString()
                    });

                return View(viewModel);
            }
            await ObradiZahtjev("1", viewModel.TipDokumenta, viewModel.RazlogZahtjeva);

            return RedirectToAction(nameof(Index));
        }

        // GET: Zahtjev/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev.FindAsync(id);
            if (zahtjev == null)
            {
                return NotFound();
            }
            ViewData["IdDokumenta"] = new SelectList(_context.Dokument, "Id", "Id", zahtjev.IdDokumenta);
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "Id", "Id", zahtjev.IdKorisnika);
            return View(zahtjev);
        }

        // POST: Zahtjev/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumSlanja,IdKorisnika,IdDokumenta,RazlogZahtjeva")] Zahtjev zahtjev)
        {
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zahtjev);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZahtjevExists(zahtjev.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDokumenta"] = new SelectList(_context.Dokument, "Id", "Id", zahtjev.IdDokumenta);
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "Id", "Id", zahtjev.IdKorisnika);
            return View(zahtjev);
        }

        // GET: Zahtjev/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zahtjev = await _context.Zahtjev.FindAsync(id);
            if (zahtjev != null)
            {
                _context.Zahtjev.Remove(zahtjev);
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
        public async Task<IActionResult> ObradiZahtjev(string idKorisnika, TipDokumenta tipDokumenta, Razlog razlog)
        {
            if (!Enum.IsDefined(typeof(TipDokumenta), tipDokumenta) || !Enum.IsDefined(typeof(Razlog), razlog))
            {
                return BadRequest("Nevažeći tip dokumenta ili razlog zahtjeva.");
            }

            var korisnik = await _context.Korisnik.FindAsync(idKorisnika);
            if (korisnik == null)
                return NotFound("Korisnik nije pronađen.");

            if (!await ProvjeriUsloveAsync(idKorisnika))
            {
                return BadRequest("Korisnik ne ispunjava uslove za obradu zahtjeva.");
            }

            var datumSlanja = DateTime.Now;

            /*var sablon = await _context.Sablon
                .FirstOrDefaultAsync(s => s.TipDokumenta == tipDokumenta);
            if (sablon == null)
                return NotFound("Šablon za traženi tip dokumenta nije pronađen.");

            byte[] sablonPDF = sablon.PDFSablona;*/
            byte[] popunjeniPDF = System.IO.File.ReadAllBytes(@"C:\ETF\TreciSemestar\ASP\Zadaće\ASPZadaca1.pdf");
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

            /*var dokument = new Dokument
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
                DatumSlanja = datumSlanja
            };
            _context.Zahtjev.Add(zahtjev);
            await _context.SaveChangesAsync();*/

            await PosaljiPDFEmail(
                emailKorisnika: korisnik.Email,
                pdfBytes: popunjeniPDF,
                subject: "Dokument je spreman",
                body: $"Poštovani,\nVaš zahtjev je obrađen. U prilogu se nalazi dokument: {tipDokumenta} za zahtjev poslan " +
                      $"{datumSlanja}.\nS poštovanjem,\neOpcina Team"
                );

            return View();
        }
    }
}
