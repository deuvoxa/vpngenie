namespace vpngenie.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public long TelegramId { get; init; }
    public string? Username { get; init; }
    public bool SubscriptionIsActive => DateTime.UtcNow <= SubscriptionEndDate;
    public DateTime SubscriptionEndDate { get; set; }
    
    public User? Referrer { get; set; }
    public ICollection<User> Referrals { get; init; } = [];
    public ICollection<Ticket> Tickets { get; init; } = [];
    public ICollection<PaymentHistory> PaymentHistories { get; init; } = [];

    public Server? Server { get; set; }
    
    public int MainMessageId { get; set; }
    
    public ICollection<UserMetadata> Metadata { get; set; } = new List<UserMetadata>();
}

public class UserMetadata
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } // Навигационное свойство
    public string Attribute { get; set; }
    public string Value { get; set; }
}