using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class TicketService(ITicketRepository ticketRepository)
{
    public async Task<Ticket?> CreateTicketAsync(Ticket? ticket) => await ticketRepository.AddAsync(ticket);

    public async Task<Ticket?> GetTicketByIdAsync(int id) => await ticketRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Ticket?>> GetAllOpenTicketsAsync() => await ticketRepository.GetAllOpenAsync();
    public async Task<IEnumerable<Ticket?>> GetAllTicketsByUserIdAsync(long id) => await ticketRepository.GetAllByUserIdAsync(id);

    public async Task UpdateTicketAsync(Ticket? ticket) => await ticketRepository.UpdateAsync(ticket);
    public async Task RespondToTicketAsync(Ticket ticket, string response)
    {
        // var ticket = await GetTicketByIdAsync(ticketId);
        ticket.Response = response;
        await UpdateTicketAsync(ticket);
    }
}