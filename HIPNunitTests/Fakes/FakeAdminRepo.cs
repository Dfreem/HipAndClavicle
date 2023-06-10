using HipAndClavicle.Models.Enums;
using HipAndClavicle.Models;
using HipAndClavicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIPNunitTests.Fakes
{
    public class FakeAdminRepo : IAdminRepo
    {
        public async Task<List<Order>> GetAdminOrdersAsync()
        {
            // I'm creating fake users here to match your seed data, but you might want to
            // move this to a separate method if you need the same users in multiple places.
            var devin = new AppUser { UserName = "dfreem987" };
            var michael = new AppUser { UserName = "michael123" };

            // Use the products and other objects from your seed data to construct the orders.
            var butterfly = new Product { Name = "Butterfly Test" };

            var order1 = new Order
            {
                DateOrdered = DateTime.Now,
                Purchaser = devin,
                Address = devin.Address,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Item = butterfly,
                        ItemType = ProductCategory.ButterFlys,
                        AmountOrdered = 3,
                        PricePerUnit = 23.00d
                    }
                }
            };

            var order2 = new Order
            {
                DateOrdered = DateTime.Now,
                Purchaser = michael,
                Address = michael.Address,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Item = butterfly,
                        ItemType = ProductCategory.ButterFlys,
                        AmountOrdered = 2,
                        PricePerUnit = 22.00d
                    }
                }
            };

            return new List<Order> { order1, order2 };
        }

        public Task<List<Order>> GetAdminOrdersAsync(OrderStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<UserSettings> GetSettingsForUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserSettingsAsync(UserSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
