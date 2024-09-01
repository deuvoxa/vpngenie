using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vpngenie.Domain.Entities;

namespace vpngenie.Infrastructure.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasMany(s => s.Users)
            .WithOne(u => u.Server);
    }
}