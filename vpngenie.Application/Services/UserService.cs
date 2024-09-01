using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class UserService(IUserRepository userRepository)
{
    public async Task<List<User?>> GetAllUsersAsync()
        => await userRepository.GetAllUsersAsync();

    public async Task<List<User?>> GetLastUsersAsync()
    {
        var users = await userRepository.GetAllUsersAsync();
        return users.Count > 10 ? users[..^10]! : users;
    }

    public async Task<User?> GetUserByTelegramIdAsync(long telegramId)
        => await userRepository.GetUserByTelegramIdAsync(telegramId);

    public async Task AddUserAsync(User user)
    {
        await userRepository.AddUserAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await userRepository.UpdateUserAsync(user);
    }

    public async Task ExtendSubscriptionAsync(User user, int daysToAdd)
    {
        user.SubscriptionEndDate = user.SubscriptionEndDate < DateTime.UtcNow
            ? DateTime.UtcNow.AddDays(daysToAdd)
            : user.SubscriptionEndDate.AddDays(daysToAdd);

        await userRepository.UpdateUserAsync(user);
    }

    public async Task RegisterReferralAsync(User? user, long referrerId)
        => await userRepository.AddReferralAsync(user, referrerId);

    public async Task AddMetadata(long telegramId, string attribute, string value)
    {
        var user = await userRepository.GetUserByTelegramIdAsync(telegramId);
        var metadata = new UserMetadata
        {
            UserId = user.Id,
            Attribute = attribute,
            Value = value
        };

        user.Metadata.Add(metadata);
        await userRepository.UpdateUserAsync(user);
    }

    public async Task RemoveMetadata(long telegramId, string attribute)
    {
        var user = await userRepository.GetUserByTelegramIdAsync(telegramId);
        var metadata = user.Metadata.FirstOrDefault(m => m.Attribute == attribute);
        if (metadata is null)
        {
            Console.WriteLine($"У пользователся {user.Username} нет метаданных с атрибутом: {attribute}");
            return;
        }
        user.Metadata.Remove(metadata);
        await userRepository.UpdateUserAsync(user);
    }
}