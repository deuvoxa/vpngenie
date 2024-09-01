namespace vpngenie.Domain.Entities;

public class Server
{
    public Guid Id { get; set; }
    public ICollection<User> Users { get; set; } = [];
    public string Region { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; }
}