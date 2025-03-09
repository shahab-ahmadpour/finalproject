using App.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Core.Services.Entities;
using System.Reflection.Emit;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.RequestConfigurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Id);
            builder.ToTable("Requests");
            builder.Property(r => r.Description).HasMaxLength(500).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.Deadline).IsRequired();
            builder.Property(r => r.ExecutionDate).IsRequired();
            builder.Property(r => r.EnvironmentImagePath).HasMaxLength(255).IsRequired(false);
            builder.Property(r => r.IsEnabled).HasDefaultValue(true);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.HasOne(r => r.Customer).WithMany(c => c.Requests).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(r => r.SubHomeService).WithMany(s => s.Requests).HasForeignKey(r => r.SubHomeServiceId).OnDelete(DeleteBehavior.NoAction);
            builder.HasData(
                new Request { Id = 1, CustomerId = 1, SubHomeServiceId = 10, Description = "درخواست بنایی ساختمان", Status = RequestStatus.Pending, Deadline = DateTime.UtcNow.AddDays(5), ExecutionDate = DateTime.UtcNow.AddDays(3), CreatedAt = DateTime.UtcNow, IsEnabled = true },
                new Request { Id = 2, CustomerId = 1, SubHomeServiceId = 11, Description = "درخواست کاغذ دیواری", Status = RequestStatus.Pending, Deadline = DateTime.UtcNow.AddDays(7), ExecutionDate = DateTime.UtcNow.AddDays(5), CreatedAt = DateTime.UtcNow, IsEnabled = true },
                new Request { Id = 3, CustomerId = 2, SubHomeServiceId = 12, Description = "درخواست سنگ کاری", Status = RequestStatus.Pending, Deadline = DateTime.UtcNow.AddDays(10), ExecutionDate = DateTime.UtcNow.AddDays(8), CreatedAt = DateTime.UtcNow, IsEnabled = true }
            );
        }
    }
}
