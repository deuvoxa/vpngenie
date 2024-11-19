using vpngenie.Domain.Entities;
using vpngenie.Domain.Enums;

namespace vpngenie.Domain.Interfaces;

public interface IServerRepository
{
    Task<List<Server>> GetAll();
    Task<List<Server>> GetByRegion(Region region);
    Task<Guid> Add(Server server);
    Task<Server> GetByIdAsync(Guid id);
    string Decrypt(string cipherText);
}