namespace HipAndClavicle.Repositories;

public interface IAdminRepo
{

    public Task<List<Order>> GetAdminOrdersAsync(OrderStatus status);


}