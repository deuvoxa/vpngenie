using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<User?> GetUserByTelegramIdAsync(long telegramId)
        => await context.Users
            // .AsNoTracking()
            .Include(u => u.Referrals)
            .ThenInclude(r => r.PaymentHistories)
            .Include(u => u.Referrer)
            .Include(u => u.Tickets)
            .Include(u => u.PaymentHistories)
            .Include(u => u.Server)
            .Include(u => u.Metadata)
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

    public async Task<List<User>> GetAllUsersAsync()
        => await context.Users
            .Include(u => u.Server)
            .Include(u => u.Tickets)
            .Include(u => u.Metadata)
            .AsNoTracking()
            .ToListAsync();
    public async Task AddUserAsync(User? user)
    {
        if (user != null) await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User? user)
    {
        if (user != null) context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task AddReferralAsync(User? user, long referrerId)
    {
        var referrer = await GetUserByTelegramIdAsync(referrerId);
        if (referrer == null)
        {
            logger.LogError($"{nameof(AddReferralAsync)}: Пользователь с ID {referrerId} не найден.");
            return;
        }

        user!.Referrer = referrer;
        referrer.Referrals.Add(user);

        context.Users.Update(user);
        context.Users.Update(referrer);
        
        await context.SaveChangesAsync();
    }
}