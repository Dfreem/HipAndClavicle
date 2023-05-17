using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HipAndClavicle.Controllers
{
    public class ContactController : Controller
    {


        private ApplicationDbContext _context;

        public UserManager<AppUser> _userManager { get; }

        public ContactController(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(CustomerChat));
            }
            return View();
        }


        [HttpPost("Create")]
        // Create a new UserMessage object
        public async Task<IActionResult> Create(UserMessageVM userMessageVM)
        {
            UserMessage userMessage = new UserMessage
            {
                Email = userMessageVM.Email,
                Number = userMessageVM.Number,
                Content = userMessageVM.Response,
                DateSent = DateTime.Now


            };

            userMessage.ReceiverUserName = "michael123";

            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("GetUserMessage")]
        // Retrieve a UserMessage object by its ID
        public async Task<IActionResult> GetUserMessage(int userMessageId)
        {
            await _context.UserMessages.FindAsync(userMessageId);

            return RedirectToAction("Index");
        }

        // Update an existing UserMessage object
        public async Task<IActionResult> UpdateUserMessage(UserMessage userMessage)
        {
            _context.Entry(userMessage).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        // Delete an existing UserMessage object
        public async Task<IActionResult> DeleteUserMessage(UserMessage userMessage)
        {
            _context.UserMessages.Remove(userMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> CustomerChat()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            //var allUsers =
            //var messages = _context.UserMessages
            //    .Where(x => x.SenderUserName == User.Identity.Name || x.ReceiverUserName == User.Identity.Name)

            //    .Select(m => new MessageViewModel
            //{
            //    Id = m.Id,
            //    Sender = m.SenderUserName,
            //    Receiver = m.ReceiverUserName,
            //    Content = m.Content,
            //    DateSent = m.DateSent
            //}).ToList();

            //ViewBag.customers = customers.Where(c => c.UserName != User.Identity.Name).ToList();
            ViewBag.customers = _context.Users.ToList();
            var messages = new List<MessageViewModel>();
            if (!User.IsInRole("Admin"))
            {
                messages = _context.UserMessages
                   .Where(x => x.SenderUserName == User.Identity.Name || x.ReceiverUserName == User.Identity.Name)

                   .Select(m => new MessageViewModel
                   {
                       Id = m.Id,
                       Sender = m.SenderUserName,
                       Receiver = m.ReceiverUserName,
                       Content = m.Content,
                       DateSent = m.DateSent,
                       Email = m.Email
                   }).ToList();
            }
            return View(messages);
        }

        public async Task<IActionResult> AllCustomerMesseges()
        {
            IList<AppUser> allAdmins = await _userManager.GetUsersInRoleAsync("Admin");

            var messages = _context.UserMessages
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Sender = m.SenderUserName,
                    Receiver = m.ReceiverUserName,
                    Content = m.Content,
                    DateSent = m.DateSent,
                    Email = m.Email
                }).ToList();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] CustomerMessage customerMessage)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            // Get the other user
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");


            UserMessage userMessage = new UserMessage
            {
                Email = currentUser.Email,
                Number = currentUser.PhoneNumber,
                Content = customerMessage.Message,
                SenderUserName = currentUser.UserName,
                DateSent = DateTime.Now
            };
            if (customerMessage.SendTo.IsNullOrEmpty())
            {
                var admin = adminUsers.Count > 0 ? adminUsers[0] : null;
                //userMessage.ReceiverUserName = admin?.UserName;
                userMessage.ReceiverUserName = "michael123";
            }
            else
            {
                userMessage.ReceiverUserName = customerMessage.SendTo;
            }
            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();


            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> FilterMesseges(string customerName, string dateSent)
        {
            if (customerName == null || dateSent == null)
            {
                return BadRequest("customer name and date sent can not be empty");
            }
            var messageDate = (DateTime)DateTime.Parse(dateSent);
            //
            var messegesUser = _context.UserMessages
                .Where(m => m.SenderUserName == customerName || m.ReceiverUserName == customerName
               )
                .AsEnumerable();
            //.ToList();
            var filtredByDate = messegesUser.Where(m => m.DateSent.Date == messageDate.Date).ToList();

            return Ok(filtredByDate);
        }

        [HttpGet]
        public async Task<IActionResult> MesseagesWithCustomer(string username)
        {
            var currentUser = User.Identity.Name;


            var messegesUser = await _context.UserMessages
    .Where(m => (m.SenderUserName == currentUser && m.ReceiverUserName == username) ||
                (m.SenderUserName == username && m.ReceiverUserName == currentUser))
    .ToListAsync();

            return Ok(messegesUser);
        }
    }

    public class CustomerMessage
    {
        public string Message { get; set; }
        public string? SendTo { get; set; }
    }
}