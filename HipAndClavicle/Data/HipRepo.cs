﻿using SQLitePCL;
using System;
namespace HipAndClavicle.Data
{
    public class HipRepo : IHipRepo
    {
        private readonly IServiceProvider _services;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HipRepo(IServiceProvider services, ApplicationDbContext context)
        {
            _services = services;
            _context = context;
            _userManager = services.GetRequiredService<UserManager<AppUser>>();
        }

        #region Orders
        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int id) =>
            await _context.Orders.FindAsync(id);

        public async Task<List<Order>> GetAdminCurrentOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Item)
                .Where(o => o.IsShipped.Equals(false)).ToListAsync();
            return orders;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Products

        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> GetProductById(int id) =>
            await _context.Products
                .Include(p => p.ColorOptions)
                .Include(p => p.Reviews)
                .FirstAsync(p => p.ProductId.Equals(id));


        public async Task<List<Product>> GetAvailableProductsAsync() =>
            await _context.Products
            .Include(p => p.ColorOptions)
            .Include(p => p.Reviews)
            .ToListAsync();

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region OrderItems

        public async Task CreateOrderItemAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderItem> GetOrderItemById(int id) =>
            await _context.OrderItems
                .Include(oi => oi.Item)
                .Include(oi => oi.ItemColor)
                .FirstAsync(p => p.OrderItemId.Equals(id));

        /// <summary>
        /// gets all the <see cref="OrderItem"/>'s in all of the orders stored in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderItem>> GetOrderItemsAsync() =>
            await _context.OrderItems
                .Include(oi => oi.Item)
                .Include(oi => oi.ItemColor)
            .ToListAsync();

        public async Task UpdateOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }


        #endregion
    }
}

