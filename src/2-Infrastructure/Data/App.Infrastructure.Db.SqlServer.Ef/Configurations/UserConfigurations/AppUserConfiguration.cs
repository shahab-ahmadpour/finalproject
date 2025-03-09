using App.Domain.Core.Enums;
using App.Domain.Core.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.UserConfigurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.ProfilePicture).HasMaxLength(255);
            builder.Property(u => u.AccountBalance).HasPrecision(18, 2);
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
        }
    }

}
