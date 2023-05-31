namespace HipAndClavicle.Models.Enums
{
    public enum OrderStatus
    {
        Received = 1,
        Paid = 2,
        ReadyToShip = 4,
        Shipped = 8,
        Late = 16,
    }
}

