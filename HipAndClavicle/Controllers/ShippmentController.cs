using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HipAndClavicle.Data;
using HipAndClavicle.Models;

namespace HipAndClavicle.Controllers
{
    public class ShippmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ShippmentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: Shippment
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Shippment.Include(s => s.Order);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Shippment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shippment == null)
            {
                return NotFound();
            }

            var shippment = await _context.Shippment
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.ShipmentId == id);
            if (shippment == null)
            {
                return NotFound();
            }

            return View(shippment);
        }

        // GET: Shippment/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "PurchaserId");
            return View();
        }

        // POST: Shippment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShippmentId,APIShippingId,TrackingNumber,Carrier,Status,Notes,DateShipped,DateDelivered,OrderId")] Models.Shipment shippment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shippment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "PurchaserId", shippment.OrderId);
            return View(shippment);
        }

        // GET: Shippment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shippment == null)
            {
                return NotFound();
            }

            var shippment = await _context.Shippment.FindAsync(id);
            if (shippment == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "PurchaserId", shippment.OrderId);
            return View(shippment);
        }

        // POST: Shippment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShippmentId,APIShippingId,TrackingNumber,Carrier,Status,Notes,DateShipped,DateDelivered,OrderId")] Models.Shipment shippment)
        {
            if (id != shippment.ShipmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippmentExists(shippment.ShipmentId))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "PurchaserId", shippment.OrderId);
            return View(shippment);
        }

        // GET: Shippment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shippment == null)
            {
                return NotFound();
            }

            var shippment = await _context.Shippment
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.ShipmentId == id);
            if (shippment == null)
            {
                return NotFound();
            }

            return View(shippment);
        }

        // POST: Shippment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shippment == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Shippment'  is null.");
            }
            var shippment = await _context.Shippment.FindAsync(id);
            if (shippment != null)
            {
                _context.Shippment.Remove(shippment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippmentExists(int id)
        {
          return (_context.Shippment?.Any(e => e.ShipmentId == id)).GetValueOrDefault();
        }
    }
}
