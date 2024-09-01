using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Enums;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;

namespace vpngenie.Infrastructure.Repositories;

public class ServerRepository(ApplicationDbContext context, IConfiguration configuration) : IServerRepository
{
    private readonly string _key = configuration.GetSection("EncryptionKey").Value!;

    public async Task<List<Server>> GetAll()
        => await context.Servers
            .Include(s => s.Users)
            .ToListAsync();


    public async Task<List<Server>> GetByRegion(Region region)
        => await context.Servers
            .Include(s => s.Users)
            .Where(s => s.Region == region.ToString()).ToListAsync();

    public async Task<Server> GetByIdAsync(Guid id)
    {
        var vps = await context.Servers
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == id);
        // if (vps != null)
        // {
        //     vps.Password = Decrypt(vps.Password);
        // }

        return vps;
    }

    public async Task<Guid> Add(Server server)
    {
        server.Password = Encrypt(server.Password);
        await context.AddAsync(server);
        await context.SaveChangesAsync();
        return server.Id;
    }

    private string Encrypt(string plainText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(_key);
        aesAlg.GenerateIV();
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
        }

        var iv = aesAlg.IV;
        var encryptedContent = msEncrypt.ToArray();
        var result = new byte[iv.Length + encryptedContent.Length];

        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aesAlg = Aes.Create();
        var iv = new byte[aesAlg.BlockSize / 8];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        aesAlg.Key = Encoding.UTF8.GetBytes(_key);
        aesAlg.IV = iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipher);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}