namespace HipAndClavicle.Repositories;

public interface IAdminRepo
{

    public Task<List<Order>> GetAdminOrdersAsync(OrderStatus status);
    public Task<UserSettings> GetSettingsForUserAsync(string id);
    public Task UpdateUserSettingsAsync(UserSettings settings);
}