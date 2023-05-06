using System;
namespace HipAndClavicle.Models;

public class Merchant : AppUser
{
    public List<UserMessage> Messages { get; set; } = new();
    public List<Order> CurrentOrders { get; set; } = new();
    public List<Order> SalesHistory { get; set; } = new();
}

