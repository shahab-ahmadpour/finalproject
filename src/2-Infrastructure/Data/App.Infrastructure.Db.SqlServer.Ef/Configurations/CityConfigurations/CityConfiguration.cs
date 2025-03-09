using App.Domain.Core.Locations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.CityConfigurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.HasOne(c => c.Province)
                   .WithMany(p => p.Cities)
                   .HasForeignKey(c => c.ProvinceId);

            builder.HasData(
                new City { Id = 1, Name = "تبریز", ProvinceId = 1 },
                new City { Id = 2, Name = "ارومیه", ProvinceId = 2 },
                new City { Id = 3, Name = "اردبیل", ProvinceId = 3 },
                new City { Id = 4, Name = "اصفهان", ProvinceId = 4 },
                new City { Id = 5, Name = "کرج", ProvinceId = 5 },
                new City { Id = 6, Name = "ایلام", ProvinceId = 6 },
                new City { Id = 7, Name = "بوشهر", ProvinceId = 7 },
                new City { Id = 8, Name = "تهران", ProvinceId = 8 },
                new City { Id = 9, Name = "شهرکرد", ProvinceId = 9 },
                new City { Id = 10, Name = "بیرجند", ProvinceId = 10 },
                new City { Id = 11, Name = "مشهد", ProvinceId = 11 },
                new City { Id = 12, Name = "بجنورد", ProvinceId = 12 },
                new City { Id = 13, Name = "اهواز", ProvinceId = 13 },
                new City { Id = 14, Name = "زنجان", ProvinceId = 14 },
                new City { Id = 15, Name = "سمنان", ProvinceId = 15 },
                new City { Id = 16, Name = "زاهدان", ProvinceId = 16 },
                new City { Id = 17, Name = "شیراز", ProvinceId = 17 },
                new City { Id = 18, Name = "قزوین", ProvinceId = 18 },
                new City { Id = 19, Name = "قم", ProvinceId = 19 },
                new City { Id = 20, Name = "سنندج", ProvinceId = 20 },
                new City { Id = 21, Name = "کرمان", ProvinceId = 21 },
                new City { Id = 22, Name = "کرمانشاه", ProvinceId = 22 },
                new City { Id = 23, Name = "یاسوج", ProvinceId = 23 },
                new City { Id = 24, Name = "گرگان", ProvinceId = 24 },
                new City { Id = 25, Name = "رشت", ProvinceId = 25 },
                new City { Id = 26, Name = "خرم‌آباد", ProvinceId = 26 },
                new City { Id = 27, Name = "ساری", ProvinceId = 27 },
                new City { Id = 28, Name = "اراک", ProvinceId = 28 },
                new City { Id = 29, Name = "بندرعباس", ProvinceId = 29 },
                new City { Id = 30, Name = "همدان", ProvinceId = 30 },
                new City { Id = 31, Name = "یزد", ProvinceId = 31 }
            );
        }
    }
}
