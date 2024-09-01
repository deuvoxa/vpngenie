using vpngenie.Domain.Entities;

namespace vpngenie.Domain.Interfaces;

public interface IPromotionRepository
{
    Task<List<Promotion>> GetActivePromotionsAsync();
}