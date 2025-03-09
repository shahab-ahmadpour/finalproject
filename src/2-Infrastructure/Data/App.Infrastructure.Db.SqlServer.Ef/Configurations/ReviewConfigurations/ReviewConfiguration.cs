using App.Domain.Core.Services.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.ReviewConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);
            builder.ToTable("Reviews");
            builder.Property(r => r.Rating).IsRequired().HasDefaultValue(1);
            builder.Property(r => r.Comment).HasMaxLength(500).IsRequired();
            builder.HasOne(r => r.Customer).WithMany(c => c.Reviews).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(r => r.Expert).WithMany(e => e.Reviews).HasForeignKey(r => r.ExpertId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(r => r.Order).WithOne().HasForeignKey<Review>(r => r.OrderId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
