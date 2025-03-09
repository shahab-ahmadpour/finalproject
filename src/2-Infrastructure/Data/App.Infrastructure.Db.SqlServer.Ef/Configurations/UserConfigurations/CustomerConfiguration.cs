using App.Domain.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.UserConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Address).HasMaxLength(255);
            builder.Property(c => c.City).HasMaxLength(100);
            builder.Property(c => c.State).HasMaxLength(100);

            builder.HasOne(c => c.AppUser)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Requests)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId);

        }
    }
}
