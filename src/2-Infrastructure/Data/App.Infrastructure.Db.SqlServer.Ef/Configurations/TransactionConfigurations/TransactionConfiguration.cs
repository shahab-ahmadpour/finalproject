using App.Domain.Core.Transactions.Entities;
using App.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.TransactionConfigurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.ToTable("Transactions");

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.TransactionDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Expert)
                .WithMany()
                .HasForeignKey(t => t.ExpertId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
