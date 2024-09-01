namespace vpngenie.Domain.Entities;

public class Promocode
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public int Usages { get; set; }
    public DateTime ValidTo { get; init; }
    public int BonusAmount { get; init; }
    public ICollection<User> PromocodeUsages { get; init; } = [];
}