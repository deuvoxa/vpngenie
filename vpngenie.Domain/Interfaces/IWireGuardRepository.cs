using vpngenie.Domain.Entities;

namespace vpngenie.Domain.Interfaces;

public interface IWireGuardRepository
{
    Task<string> CreateWireGuardConfig(Server server, string username);
    Task DeleteClient(Server server, string username);
    Task<string> ChangeServer(Server oldServer, Server newServer, string username);
}