namespace vpngenie.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    public bool IsOpen { get; set; } = true;
    public string Response { get; set; } = string.Empty;
    public DateTime ClosedAt { get; set; } = DateTime.MinValue;

    public Guid UserId { get; set; }
    public User? User { get; set; }
}