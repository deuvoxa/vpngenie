using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class WireGuardRepository(ApplicationDbContext context, IConfiguration configuration) : IWireGuardRepository
{
    private const string WgConfigPath = "/etc/wireguard/wg0.conf";

    public async Task<string> CreateWireGuardConfig(Server server, string username)
        => await Task.Run(() =>
        {
            var serverRepository = new ServerRepository(context, configuration);
            using SshClient client = new(server.IpAddress, server.Username, serverRepository.Decrypt(server.Password));
            client.Connect();
            var config = CreateOrGetConfig(client, username, server.IpAddress);
            client.Disconnect();
            return config;
        });

    public async Task DeleteClient(Server server, string username)
        => await Task.Run(() =>
        {
            var serverRepository = new ServerRepository(context, configuration);
            using SshClient client = new(server.IpAddress, server.Username, serverRepository.Decrypt(server.Password));
            client.Connect();
            DeleteClient(client, username);
            client.Disconnect();
        });
    public async Task<string> ChangeServer(Server oldServer, Server newServer, string username)
        => await Task.Run(() =>
        {
            var serverRepository = new ServerRepository(context, configuration);
            var oldServerPassword = serverRepository.Decrypt(oldServer.Password);
            var newServerPassword = serverRepository.Decrypt(newServer.Password);

            using SshClient oldSshServer = new(oldServer.IpAddress, oldServer.Username, oldServerPassword);

            using SshClient newSshServer = new(newServer.IpAddress, newServer.Username, newServerPassword);

            oldSshServer.Connect();
            newSshServer.Connect();

            DeleteClient(oldSshServer, username);

            oldSshServer.Disconnect();

            var config = CreateOrGetConfig(newSshServer, username, newServer.IpAddress);

            newSshServer.Disconnect();

            return config;
        });

    private static string CreateOrGetConfig(SshClient client, string username, string ip)
    {
        try
        {
            var checkFile = $"/etc/wireguard/clients/{username}.conf";

            var checkFileExists = RunCommand(client, $"if [ -f {checkFile} ]; then echo 'exists'; fi");
            if (checkFileExists == "exists")
            {
                var configExists = RunCommand(client, $"cat {checkFile}");
                client.Disconnect();
                return configExists;
            }


            var serverPublicKey = RunCommand(client, "cat /etc/wireguard/publickey");
            var privateKey = RunCommand(client, $"wg genkey | tee /etc/wireguard/clients/{username}.key");
            var publicKey = RunCommand(client,
                $"echo {privateKey} | wg pubkey | tee /etc/wireguard/clients/{username}.pub");
            var availableIpAddress = GetAvailableIpAddress(client);

            var configContent = $"""
                                 [Interface]
                                 PrivateKey = {privateKey}
                                 Address = 10.7.0.{availableIpAddress}/24
                                 DNS = 8.8.8.8

                                 [Peer]
                                 PublicKey = {serverPublicKey}
                                 Endpoint = {ip}:51830
                                 AllowedIPs = 0.0.0.0/0
                                 PersistentKeepalive = 20
                                 """;

            var serverConfigUpdate = $"""
                                      [Peer]
                                      PublicKey = {publicKey}
                                      AllowedIPs = 10.7.0.{availableIpAddress}/32

                                      """;

            RunCommand(client, $"echo '{configContent}' | sudo tee /etc/wireguard/clients/{username}.conf");
            RunCommand(client, $"echo '{serverConfigUpdate}' | tee -a {WgConfigPath}");
            RunCommand(client, "systemctl restart wg-quick@wg0");

            client.Disconnect();
            return configContent;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return "";
    }

    private static void DeleteClient(SshClient client, string username)
    {
        var checkFile = $"/etc/wireguard/clients/{username}.pub";
        var checkFileExists = RunCommand(client, $"if [ -f {checkFile} ]; then echo 'exists'; fi");

        if (checkFileExists != "exists") return;
        var userPublicKey = RunCommand(client, $"cat {checkFile}").Replace("/", @"\/");

        RunCommand(client, $"rm /etc/wireguard/clients/{username}.*");
        RunCommand(client, $@"sed -i '/\[Peer\]/N;/PublicKey = {userPublicKey}/,+2d' {WgConfigPath}");
        RunCommand(client, "systemctl restart wg-quick@wg0");
    }

    private static int GetAvailableIpAddress(SshClient client)
    {
        const string pattern = @"AllowedIPs\s*=\s*10\.7\.0\.(\d+)/32";

        var result = RunCommand(client, $"cat {WgConfigPath}");

        var matches = Regex.Matches(result, pattern);

        var ipNumbers = new List<int>();

        foreach (Match match in matches)
        {
            if (int.TryParse(match.Groups[1].Value, out var ipNumber))
                ipNumbers.Add(ipNumber);
        }

        if (ipNumbers.Count == 0)
        {
            return 2;
        }

        ipNumbers.Sort();

        for (var i = 1; i < ipNumbers.Count; i++)
        {
            if (ipNumbers[i] - ipNumbers[i - 1] > 1)
            {
                return ipNumbers[i - 1] + 1;
            }
        }

        return ipNumbers[^1] + 1;
    }

    private static string RunCommand(SshClient client, string commandText)
    {
        var command = client.RunCommand(commandText);
        if (command.ExitStatus == 0) return command.Result.Trim();
        throw new Exception("Command execution failed: " + commandText);
    }
}