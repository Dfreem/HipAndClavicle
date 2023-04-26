namespace HipAndClavicle.Repositories;

public interface IShippmentRepo
{
    public Task CreateShippmentAssync();
    public Task<Shipment> GetShippmentById(int id);

}
