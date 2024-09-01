using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vpngenie.Domain.Entities;

namespace vpngenie.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasMany(u => u.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        
        builder.HasOne(u => u.Referrer)
            .WithMany(u => u.Referrals)
            .HasForeignKey("ReferrerId");

        builder.HasOne(u => u.Server)
            .WithMany(s => s.Users);
    }
}