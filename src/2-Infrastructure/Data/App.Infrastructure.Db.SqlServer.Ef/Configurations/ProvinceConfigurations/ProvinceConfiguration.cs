using App.Domain.Core.Locations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.ProvinceConfigurations
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(
                new Province { Id = 1, Name = "آذربایجان شرقی" },
                new Province { Id = 2, Name = "آذربایجان غربی" },
                new Province { Id = 3, Name = "اردبیل" },
                new Province { Id = 4, Name = "اصفهان" },
                new Province { Id = 5, Name = "البرز" },
                new Province { Id = 6, Name = "ایلام" },
                new Province { Id = 7, Name = "بوشهر" },
                new Province { Id = 8, Name = "تهران" },
                new Province { Id = 9, Name = "چهارمحال و بختیاری" },
                new Province { Id = 10, Name = "خراسان جنوبی" },
                new Province { Id = 11, Name = "خراسان رضوی" },
                new Province { Id = 12, Name = "خراسان شمالی" },
                new Province { Id = 13, Name = "خوزستان" },
                new Province { Id = 14, Name = "زنجان" },
                new Province { Id = 15, Name = "سمنان" },
                new Province { Id = 16, Name = "سیستان و بلوچستان" },
                new Province { Id = 17, Name = "فارس" },
                new Province { Id = 18, Name = "قزوین" },
                new Province { Id = 19, Name = "قم" },
                new Province { Id = 20, Name = "کردستان" },
                new Province { Id = 21, Name = "کرمان" },
                new Province { Id = 22, Name = "کرمانشاه" },
                new Province { Id = 23, Name = "کهگیلویه و بویراحمد" },
                new Province { Id = 24, Name = "گلستان" },
                new Province { Id = 25, Name = "گیلان" },
                new Province { Id = 26, Name = "لرستان" },
                new Province { Id = 27, Name = "مازندران" },
                new Province { Id = 28, Name = "مرکزی" },
                new Province { Id = 29, Name = "هرمزگان" },
                new Province { Id = 30, Name = "همدان" },
                new Province { Id = 31, Name = "یزد" }
            );
        }
    }
}
