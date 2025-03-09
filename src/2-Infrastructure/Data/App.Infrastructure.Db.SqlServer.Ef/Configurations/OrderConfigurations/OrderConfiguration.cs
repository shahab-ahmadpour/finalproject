using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.OrderConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.ToTable("Orders");
            builder.Property(o => o.FinalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(o => o.PaymentStatus).IsRequired();
            builder.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o => o.Expert).WithMany(e => e.Orders).HasForeignKey(o => o.ExpertId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o => o.Request).WithMany(r => r.Orders).HasForeignKey(o => o.RequestId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o => o.Proposal).WithOne(p => p.Order).HasForeignKey<Order>(o => o.ProposalId).IsRequired(false).OnDelete(DeleteBehavior.SetNull); 
            
            
            

        }
    }
}
