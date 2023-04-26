﻿namespace HipAndClavicle.Models;

public class Shipment
{
    public int ShipmentId { get; set; }
    public string APIShippingId { get; set; } = default!;
    public string? TrackingNumber { get; set; } = default!;
    public string Carrier { get; set; } = default!;
    public OrderStatus Status { get; set; } = default!;
    public string? Notes { get; set; } = default!;
    public DateTime DateShipped { get; set; }
    public DateTime DateDelivered { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;
}

