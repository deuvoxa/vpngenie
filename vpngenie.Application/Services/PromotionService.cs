using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class PromotionService(IPromotionRepository repository)
{
    public async Task<List<Promotion>> GetActivePromotionsAsync()
        => await repository.GetActivePromotionsAsync();
}