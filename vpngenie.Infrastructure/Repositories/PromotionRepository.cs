using Microsoft.EntityFrameworkCore;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class PromotionRepository(ApplicationDbContext context) : IPromotionRepository
{
    public async Task<List<Promotion>> GetActivePromotionsAsync()
    {
        var promotions = await context.Promotions.ToListAsync();
        return promotions.Where(p => p.IsActive).ToList();
    }

}