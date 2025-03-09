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
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AppUserId)
                .IsRequired();

            builder.HasOne(a => a.AppUser)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.AppUserId) 
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
