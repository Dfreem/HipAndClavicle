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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var query = _context.UserMessages.OrderBy(x => x.DateSent).AsQueryable();

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

            List<UserMessage>? mainInquries = new List<UserMessage>();
            //_context.UserMessages.AsNoTracking()
            var allUserMeaages = _context.UserMessages.OrderBy(x => x.DateSent).ToList();
            foreach (var item in filteredMessages)
            {
                if (item.CustomerMessageId == null || item.CustomerMessageId == 0)
                {
                    mainInquries.Add(item);
                }
                else
                {
                    var checkIfInList = mainInquries.IndexOf(item);
                    if (checkIfInList == -1)
                    {
                        var mainMessage = allUserMeaages.FirstOrDefault(x => x.Id == item.CustomerMessageId && x.IsNewQuestion);
                        mainMessage.DateSent = item.DateSent;
                        mainInquries.Add(mainMessage);
                    }
                    else
                    {
                        mainInquries[checkIfInList].DateSent = item.DateSent;
                    }
                }
            }
            return Ok(mainInquries);
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

        [HttpGet]
        public async Task<IActionResult> CustomerSupport()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _context.Users.Include(c => c.Address).FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                var inquriesByCustomer = _context.UserMessages
                    .Where(x => x.CustomerId == currentUser.Id && x.IsNewQuestion).ToList();
                return View(inquriesByCustomer);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ContactSupport()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactSupport(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _context.Users.Include(c => c.Address).FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                UserMessage userMessage = new UserMessage
                {
                    Email = currentUser.Email,
                    Number = currentUser.PhoneNumber,
                    SenderUserName = currentUser.UserName,
                    CustomerId = currentUser.Id,
                    DateSent = DateTime.Now,
                    CustomerName = currentUser.UserName,
                    Title = contactVM.Title,
                    Description = contactVM.Description,
                    IsNewQuestion = true
                };

                var ordersByCustomer = _context.Orders.Where(o => o.PurchaserId == currentUser.Id)
                   .OrderByDescending(o => o.DateOrdered).ToList();
                Order recentOrder = new Order();
                if (ordersByCustomer.Count > 0)
                {
                    recentOrder = ordersByCustomer[0];
                    OrderItem? recentProduct = _context.OrderItems.ToList().LastOrDefault(x => recentOrder.OrderId == recentOrder.OrderId);
                    Product? product = _context.Products.FirstOrDefault(x => x.ProductId == recentProduct.ProductId);
                    userMessage.Product = product?.Name;

                    userMessage.City = currentUser?.Address?.CityTown;

                }
                _context.UserMessages.Add(userMessage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(CustomerSupport));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ResponseToMessage(string? message, int CustomerMessageId)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _context.Users.Include(c => c.Address).FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                UserMessage userMessage = new UserMessage
                {
                    Email = currentUser.Email,
                    Number = currentUser.PhoneNumber,
                    SenderUserName = currentUser.UserName,
                    CustomerId = currentUser.Id,
                    DateSent = DateTime.Now,
                    CustomerMessageId = CustomerMessageId,
                    Description = message
                };
                var messagesForQ = _context.UserMessages
                    .Where(x => x.CustomerMessageId == CustomerMessageId || x.Id == CustomerMessageId)
                    .OrderByDescending(x => x.DateSent)
                    .FirstOrDefault();

                if (messagesForQ != null && userMessage.CustomerId != messagesForQ.CustomerId)
                {
                    messagesForQ.IsRead = true;
                    _context.Update(messagesForQ);
                }
                _context.UserMessages.Add(userMessage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(CustomerSupport));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Messages(ContactVM contactVM)
        {
            var customersWithMessages = _context.UserMessages.Where(x => x.CustomerId != null).ToList();
            ViewBag.products = new SelectList(_context.Products.ToList(), "Name", "Name");
            //ViewBag.cities = new SelectList(_context.Addresses.ToList(), "CityTown", "CityTown");
            ViewBag.cities = new SelectList(_context.Addresses.ToList().DistinctBy(x => x.CityTown), "CityTown", "CityTown");
            return View(customersWithMessages);
        }

        public async Task<IActionResult> CustomerList()
        {
            //var allCustomers =( List<AppUser> )await _userManager.GetUsersInRoleAsync("Customer");
            //var allCustomers = await _userManager.GetUsersInRoleAsync("Customer");

            var users = await _userManager.GetUsersInRoleAsync("Admin");
            var allCustomers = _userManager.Users.Where(user => !users.Contains(user));


            var unseenMessages = _context.UserMessages
                .Where(x => x.IsResolved == false && x.IsRead == false).ToList();

            List<CustomerListVM> customersVM = new List<CustomerListVM>();
            foreach (var item in allCustomers)
            {
                CustomerListVM customerListVM = new CustomerListVM()
                {
                    Id = item.Id,
                    CustomerName = item.UserName,
                    Email = item.Email,
                };
                if (!unseenMessages.IsNullOrEmpty())
                {
                    var count = unseenMessages.Where(x => x.CustomerId == item.Id).Count();
                    if (count > 0)
                    {
                        customerListVM.HasUnreadMessage = true;
                    }
                }
                customersVM.Add(customerListVM);
            }
            var w = _context.UserMessages.Where(x => x.CustomerId == null).ToList();
            if (!w.IsNullOrEmpty())
            {
                w.ForEach(x =>
                {
                    CustomerListVM customerListVM = new CustomerListVM()
                    {
                        Email = x.Email,
                        Content = x.Content,
                    };
                    customersVM.Add(customerListVM);
                });
            }

            return View(customersVM);
        }

        public async Task<IActionResult> MessagesFromCustomer(string id)
        {
            var messagesFromCustomer = _context.UserMessages.Where(x => x.CustomerId == id && x.IsNewQuestion == true).ToList();
            ViewBag.products = new SelectList(_context.Products.ToList(), "Name", "Name");
            ViewBag.cities = new SelectList(_context.Addresses.ToList().DistinctBy(x => x.CityTown), "CityTown", "CityTown");
            return View(messagesFromCustomer);
        }
        public async Task<IActionResult> MessagesForQuestion(int id)
        {
            var messagesForQuestion = _context.UserMessages.Where(x => x.CustomerMessageId == id).ToList();
            return Ok(messagesForQuestion);
        }

        [HttpGet]
        public async Task<IActionResult> IssueResolved(int mainQuestionId)
        {
            var messagesForQuestion = _context.UserMessages
                .Where(x => x.CustomerMessageId == mainQuestionId || x.Id == mainQuestionId)
                .ToList();
            messagesForQuestion.ForEach(x => x.IsResolved = true);

            _context.UpdateRange(messagesForQuestion);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
    public class CustomerListVM
    {
        public string Id { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public DateTime? LastDateSent { get; set; }
        public string? Product { get; set; }
        public string? City { get; set; }
        public bool HasUnreadMessage { get; set; }
        public string Content { get; set; }
    }

    public class CustomerMessage
    {
        public string Message { get; set; }
        public string? SendTo { get; set; }
    }

    public class ContactVM
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string? Description { get; set; }
    }
    public class RespondToMessage
    {
        public int CustomerMessageId { get; set; }
        public string? Response { get; set; }
    }
}
