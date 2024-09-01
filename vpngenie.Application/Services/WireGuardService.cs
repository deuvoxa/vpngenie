using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class WireGuardService(IWireGuardRepository wireGuardRepository)
{
    public async Task<string> CreateWireGuardConfig(Server server, string username)
        => await wireGuardRepository.CreateWireGuardConfig(server, username);

    public async Task DeleteClient(Server server, string username)
        => await wireGuardRepository.DeleteClient(server, username);
    
    public async Task<string> ChangeServer(Server oldServer, Server newServer, string username)
        => await wireGuardRepository.ChangeServer(oldServer, newServer, username);
}