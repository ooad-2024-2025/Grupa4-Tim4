using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SablonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sablon
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sablon.ToListAsync());
        }

        // GET: Sablon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sablon = await _context.Sablon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sablon == null)
            {
                return NotFound();
            }

            return View(sablon);
        }

        // GET: Sablon/Create
        public IActionResult Create()
        {
            ViewData["TipDokumenta"] = new SelectList(Enum.GetValues(typeof(TipDokumenta)).Cast<TipDokumenta>());
            return View();
        }

        // POST: Sablon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipDokumenta")] Sablon sablon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sablon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sablon);
        }

        // GET: Sablon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sablon = await _context.Sablon.FindAsync(id);
            if (sablon == null)
            {
                return NotFound();
            }
            return View(sablon);
        }

        // POST: Sablon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipDokumenta")] Sablon sablon)
        {
            if (id != sablon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sablon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SablonExists(sablon.Id))
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
            return View(sablon);
        }

        // GET: Sablon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sablon = await _context.Sablon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sablon == null)
            {
                return NotFound();
            }

            return View(sablon);
        }

        // POST: Sablon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sablon = await _context.Sablon.FindAsync(id);
            if (sablon != null)
            {
                _context.Sablon.Remove(sablon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SablonExists(int id)
        {
            return _context.Sablon.Any(e => e.Id == id);
        }
    }
}
