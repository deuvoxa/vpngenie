using Microsoft.EntityFrameworkCore;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class PromocodeRepository(ApplicationDbContext context) : IPromocodeRepository
{
    public async Task<List<Promocode>> GetAll()
        => await context.Promocodes
            .Include(u => u.PromocodeUsages)
            .ToListAsync();

    public async Task<Guid> Create(Promocode promocode)
    {
        await context.Promocodes.AddAsync(promocode);
        await context.SaveChangesAsync();
        return promocode.Id;
    }

    public async Task Update(Promocode promocode)
    {
        context.Promocodes.Update(promocode);
        await context.SaveChangesAsync();
    }
}