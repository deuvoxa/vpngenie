using vpngenie.Domain.Entities;

namespace vpngenie.Domain.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> AddAsync(Ticket? ticket);
    Task<Ticket?> GetByIdAsync(int id);
    Task<IEnumerable<Ticket?>> GetAllByUserIdAsync(long id);
    Task<IEnumerable<Ticket?>> GetAllOpenAsync();
    Task UpdateAsync(Ticket? ticket);
}