using Microsoft.EntityFrameworkCore;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class TicketRepository(ApplicationDbContext context) : ITicketRepository
{
    public async Task<Ticket?> AddAsync(Ticket ticket)
    {
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket?> GetByIdAsync(int id)
        => await context.Tickets
            .Include(t => t.User)
            .FirstOrDefaultAsync(t=> t.Id == id);

    public async Task<IEnumerable<Ticket?>> GetAllByUserIdAsync(long id)
        => await context.Tickets
            .Include(t => t.User)
            .Where(t => t.User.TelegramId == id)
            .ToListAsync();

    public async Task<IEnumerable<Ticket?>> GetAllOpenAsync()
        => await context.Tickets
            .Include(t => t.User)
            .OrderBy(t => t.Id)
            .Where(t => t != null && t.IsOpen).ToListAsync();

    public async Task UpdateAsync(Ticket? ticket)
    {
        context.Tickets.Update(ticket);
        await context.SaveChangesAsync();
    }
}