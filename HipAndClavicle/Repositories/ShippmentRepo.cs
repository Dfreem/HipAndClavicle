namespace HipAndClavicle.Repositories;

public class ShippmentRepo : IShippmentRepo
{
    ShipmentApi _shipping;
    public ShippmentRepo(IConfigurationProvider config)
    {
        
    }


    public Task CreateShippmentAssync()
    {
        throw new NotImplementedException();
    }

    public Task<Shipment> GetShippmentById(int id)
    {
        throw new NotImplementedException();
    }
}
