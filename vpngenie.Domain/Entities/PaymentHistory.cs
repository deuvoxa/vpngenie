namespace vpngenie.Domain.Entities;

public class PaymentHistory
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.MinValue;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}