using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HipAndClavicle.Data;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Identity;
using HipAndClavicle.ViewModels;

namespace HipAndClavicle.Controllers
{
    [Authorize]
    public class ShippingAddressesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotyfService _notyfService;

        public ShippingAddressesController(ApplicationDbContext context,
            UserManager<AppUser> userManager, INotyfService notyfService)
        {
            _context = context;
            _userManager = userManager;
            _notyfService = notyfService;
        }

        public async Task<IActionResult> Index()
        {

            AppUser currentUser = await _context.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);


            //var userShippingAddresses = await _context.Addresses.Where(x => x.AppUserId == currentUser.Id).ToListAsync();
            //return  View(userShippingAddresses) ;
            return View(currentUser);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Addresses == null)
            {
                return NotFound();
            }

            var shippingAddress = await _context.Addresses
                .FirstOrDefaultAsync(m => m.ShippingAddressId == id);
            if (shippingAddress == null)
            {
                return NotFound();
            }

            return View(shippingAddress);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingAddressVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.GetUserAsync(User);

                ShippingAddress shippingAddress = new ShippingAddress()
                {
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    CityTown = model.CityTown,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber,
                    PostalCode = model.PostalCode,
                    Residential = model.Residential,
                    StateAbr = model.StateAbr
                };

                user.Address = shippingAddress;
                IdentityResult result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Error updating shipping address.");
                }

                return RedirectToAction(nameof(Index), "Home");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit()
        {

            AppUser currentUser = await _context.Users
             .Include(x => x.Address)
             .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var shippingAddress = currentUser.Address;
            if (shippingAddress == null)
            {
                _notyfService.Error("no shipping address for user");
                return View();
            }
            var vm = new ShippingAddressVM
            {
                CityTown = shippingAddress.CityTown,
                ShippingAddressId = shippingAddress.ShippingAddressId,
                AddressLine1 = shippingAddress.AddressLine1,
                AddressLine2 = shippingAddress.AddressLine2,
                AppUserId = currentUser.Id,
                Country = shippingAddress.Country,
                PhoneNumber = shippingAddress.PhoneNumber,
                PostalCode = shippingAddress.PostalCode,
                Residential = shippingAddress.Residential,
                StateAbr = shippingAddress.StateAbr
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ShippingAddressVM model)
        {

            if (id != model.ShippingAddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    ShippingAddress shippingAddress = _context.Addresses.Find(model.ShippingAddressId);
                    if (shippingAddress is null)
                    {
                        _notyfService.Error("Shipping Address not fouund");
                        return View();
                    }
                    shippingAddress.AddressLine1 = model.AddressLine1;
                    shippingAddress.AddressLine2 = model.AddressLine2;
                    shippingAddress.CityTown = model.CityTown;
                    shippingAddress.Country = model.Country;
                    shippingAddress.PhoneNumber = model.PhoneNumber;
                    shippingAddress.PostalCode = model.PostalCode;
                    shippingAddress.Residential = model.Residential;
                    shippingAddress.StateAbr = model.StateAbr;

                    _context.Update(shippingAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingAddressExists(model.ShippingAddressId))
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
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Addresses == null)
            {
                return NotFound();
            }

            var shippingAddress = await _context.Addresses
                .FirstOrDefaultAsync(m => m.ShippingAddressId == id);
            if (shippingAddress == null)
            {
                return NotFound();
            }

            return View(shippingAddress);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Addresses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Addresses'  is null.");
            }
            var shippingAddress = await _context.Addresses.FindAsync(id);
            if (shippingAddress != null)
            {
                _context.Addresses.Remove(shippingAddress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingAddressExists(int id)
        {
            return (_context.Addresses?.Any(e => e.ShippingAddressId == id)).GetValueOrDefault();
        }
    }
}