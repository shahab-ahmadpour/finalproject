using App.Domain.Core.Users.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.UserConfigurations
{
    public class ExpertConfiguration : IEntityTypeConfiguration<Expert>
    {
        public void Configure(EntityTypeBuilder<Expert> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.AppUserId)
                .IsRequired();

            builder.HasOne(e => e.AppUser)
                .WithOne(u => u.Expert)
                .HasForeignKey<Expert>(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
