
namespace HipAndClavicle.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShippmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        string _apiKey;
        string _apiSecret;
        string _apiUrl =  "https://shipping-api-sandbox.pitneybowes.com/shippingservices";

        public ShippmentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _apiKey = _configuration["PitneyBowes:Key"]!; 
            _apiSecret = _configuration["PitneyBowes:Secret"]!;
            _apiUrl = _configuration["ShippingAPI:ApiUrl"]!;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingVM svm)
        {
            if (ModelState.IsValid)
            {
                _context.Add((Shipment)svm);
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
