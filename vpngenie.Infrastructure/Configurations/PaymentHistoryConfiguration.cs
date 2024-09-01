using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vpngenie.Domain.Entities;

namespace vpngenie.Infrastructure.Configurations;

public class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
{
    public void Configure(EntityTypeBuilder<PaymentHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.HasOne(h => h.User)
            .WithMany(u => u.PaymentHistories)
            .HasForeignKey(h => h.UserId);
    }
}