using vpngenie.Domain.Entities;

namespace vpngenie.Domain.Interfaces;

public interface IPromocodeRepository
{
    Task<List<Promocode>> GetAll();
    Task<Guid> Create(Promocode promocode);
    Task Update(Promocode promocode);
}