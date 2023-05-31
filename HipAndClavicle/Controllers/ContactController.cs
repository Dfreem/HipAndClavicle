using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Rendering;


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
            await CreateGuestUserMessage(userMessageVM);

            return RedirectToAction("Index");
        }

        public async Task<UserMessage> CreateGuestUserMessage(UserMessageVM userMessageVM)
        {
            UserMessage userMessage = new UserMessage
            {
                Email = userMessageVM.Email,
                Number = userMessageVM.Number,
                Content = userMessageVM.Response,
                DateSent = DateTime.Now
            };

            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            return userMessage;
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
            var userMessages = _context.UserMessages.Where(x => x.IsArchived).ToList();

            var archivedUsernames = userMessages
                .SelectMany(x => new[] { x.SenderUserName })
                .Where(username => !string.IsNullOrEmpty(username))
                .Distinct()
                .ToArray();

            var usersNotArchived = _context.Users
                                         .Where(x => !archivedUsernames.Contains(x.UserName))
                                         .ToList();

            ViewBag.customers = usersNotArchived;
            var messages = new List<MessageViewModel>();
            if (!User.IsInRole("Admin"))
            {
                messages = _context.UserMessages
                   .Where(x => x.SenderUserName == User.Identity.Name || x.ReceiverUserName == User.Identity.Name
                   && !x.IsArchived)

                   .Select(m => new MessageViewModel
                   {
                       Id = m.Id,
                       Sender = m.SenderUserName,
                       Receiver = m.ReceiverUserName,
                       Content = m.Content,
                       DateSent = m.DateSent,
                       Product = m.Product,
                       City = m.City
                   }).ToList();
            }
            ViewBag.ArchivedChat = false;
            return View(messages);
        }

        public async Task<IActionResult> ArchivedChats()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");

            var userMessages = _context.UserMessages.Where(x => x.IsArchived).ToList();

            var archivedUsernames = userMessages
                .SelectMany(x => new[] { x.SenderUserName })
                .Where(username => !string.IsNullOrEmpty(username))
                .Distinct()
                .ToArray();

            var usersNotArchived = _context.Users
                                         .Where(x => archivedUsernames.Contains(x.UserName))
                                         .ToList();

            ViewBag.customers = usersNotArchived;
            var messages = new List<MessageViewModel>();
            if (!User.IsInRole("Admin"))
            {
                messages = _context.UserMessages
                   .Where(x => x.SenderUserName == User.Identity.Name || x.ReceiverUserName == User.Identity.Name
                   && !x.IsArchived)

                   .Select(m => new MessageViewModel
                   {
                       Id = m.Id,
                       Sender = m.SenderUserName,
                       Receiver = m.ReceiverUserName,
                       Content = m.Content,
                       DateSent = m.DateSent,
                       Product = m.Product,
                       City = m.City,
                   }).ToList();
            }
            ViewBag.ArchivedChat = true;
            return View("CustomerChat", messages);
        }

        public async Task<IActionResult> ArchiveChat(string username)
        {
            var messagesFromCustomer = _context.UserMessages
                .Where(x => x.SenderUserName == username).ToList();
            messagesFromCustomer.ForEach(x => x.IsArchived = true);

            _context.SaveChanges();
            return RedirectToAction(nameof(CustomerChat));
        }

        public async Task<IActionResult> UnArchiveChat(string username)
        {
            var messagesFromCustomer = _context.UserMessages
                .Where(x => x.SenderUserName == username).ToList();
            messagesFromCustomer.ForEach(x => x.IsArchived = false);

            _context.SaveChanges();
            return RedirectToAction(nameof(CustomerChat));
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
                    Email = m.Email,
                    Content = m.Content,
                    DateSent = m.DateSent,
                    Product = m.Product,
                    City = m.City
                }).ToList();
            ViewBag.products = new SelectList(_context.Products.ToList(), "Name", "Name");
            //ViewBag.cities = new SelectList(_context.Addresses.ToList(), "CityTown", "CityTown");
            ViewBag.cities = new SelectList(_context.Addresses.ToList().DistinctBy(x => x.CityTown), "CityTown", "CityTown");
            return View(messages);
        }
        [HttpPost]
        public async Task<UserMessage> SaveMessage([FromBody] CustomerMessage customerMessage)
        {
            var currentUser = await _context.Users.Include(c => c.Address).FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

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
                var hcadmin = await _userManager.FindByNameAsync("hcsadmin");

                userMessage.ReceiverUserName = hcadmin != null ? hcadmin.UserName : admin?.UserName;
                //userMessage.ReceiverId = admin?.Id;

                var ordersByCustomer = _context.Orders.Where(o => o.PurchaserId == currentUser.Id)
                    .OrderByDescending(o => o.DateOrdered).ToList();
                Order recentOrder = new Order();
                if (ordersByCustomer.Count > 0)
                {
                    recentOrder = ordersByCustomer[0];
                    //OrderItem? recentProduct = _context.OrderItems.FirstOrDefault(x => recentOrder.OrderId == recentOrder.OrderId);
                    OrderItem? recentProduct = _context.OrderItems.ToList().LastOrDefault(x => recentOrder.OrderId == recentOrder.OrderId);

                    Product? product = _context.Products.FirstOrDefault(x => x.ProductId == recentProduct.ProductId);
                    userMessage.Product = product?.Name;

                    userMessage.City = currentUser?.Address?.CityTown;

                }
            }
            else
            {
                userMessage.ReceiverUserName = customerMessage.SendTo;
                //userMessage.ReceiverId = customerMessage.SendTo;
            }
            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();
            return userMessage;
            //return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> FilterMesseges(string customerName, string dateSent, string? product, string? city)
        {
            var query = _context.UserMessages.AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(m => m.SenderUserName == customerName || m.ReceiverUserName == customerName);
            }

            if (!string.IsNullOrEmpty(dateSent))
            {
                // Assuming dateSent is a string representation of DateTime
                var sentDate = DateTime.Parse(dateSent);
                query = query.Where(m => m.DateSent.Date == sentDate.Date);
            }

            if (!string.IsNullOrEmpty(product))
            {

                query = query.Where(m => m.Product == product);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(m => m.City == city);
            }

            var filteredMessages = await query.ToListAsync();

            // Process the filtered messages as needed

            return Ok(filteredMessages);
            //var allUserMessages = _context.UserMessages.ToList();
            //var filtredUserMessages = new List<UserMessage>();
            //if (!string.IsNullOrEmpty(customerName))
            //{
            //    filtredUserMessages = allUserMessages.
            //}

            //if (customerName.IsNullOrEmpty() || dateSent.IsNullOrEmpty())
            //{
            //    return BadRequest("customer name and date sent can not be empty");
            //}
            //var messageDate = (DateTime)DateTime.Parse(dateSent);
            ////
            //var messegesUser = _context.UserMessages
            //    .Where(m => m.SenderUserName == customerName || m.ReceiverUserName == customerName
            //   )
            //    .AsEnumerable();
            ////.ToList();
            //var filtredByDate = messegesUser.Where(m => m.DateSent.Date == messageDate.Date).ToList();

            //return Ok();
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
