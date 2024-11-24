using vpngenie.Domain.Entities;

namespace vpngenie.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByTelegramIdAsync(long telegramId);
    Task<List<User>> GetAllUsersAsync();
    Task AddUserAsync(User? user);
    Task UpdateUserAsync(User? user);
    Task AddReferralAsync(User? user, long referrerId);
}