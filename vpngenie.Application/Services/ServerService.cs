using vpngenie.Domain.Entities;
using vpngenie.Domain.Enums;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class ServerService(IServerRepository serverRepository)
{
    public async Task<List<Server>> GetAllServers()
        => await serverRepository.GetAll();

    public async Task<List<Server>> GetServersByRegion(Region region)
        => await serverRepository.GetByRegion(region);

    public async Task<Guid> AddServerAsync(Server server)
        => await serverRepository.Add(server);

    public async Task<Server> GetServerByIdAsync(Guid id)
        => await serverRepository.GetByIdAsync(id);
}