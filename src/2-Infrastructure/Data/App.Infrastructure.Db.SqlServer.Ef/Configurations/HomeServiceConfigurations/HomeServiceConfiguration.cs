using App.Domain.Core.Services.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.HomeServiceConfigurations
{
    public class HomeServiceConfiguration : IEntityTypeConfiguration<HomeService>
    {
        public void Configure(EntityTypeBuilder<HomeService> builder)
        {
            builder.HasKey(hs => hs.Id);

            builder.ToTable("HomeServices");

            builder.Property(hs => hs.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(hs => hs.Description)
                .HasMaxLength(500);

            builder.Property(hs => hs.ImagePath)
                .HasMaxLength(255);


            builder.Property(hs => hs.IsActive)
                .HasDefaultValue(true);

            builder.Property(hs => hs.CategoryId)
                   .IsRequired();

            builder.HasOne(hs => hs.Category)
                   .WithMany(c => c.HomeServices)
                   .HasForeignKey(hs => hs.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(hs => hs.SubHomeServices)
                .WithOne(shs => shs.HomeService)
                .HasForeignKey(shs => shs.HomeServiceId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasData(
                new HomeService { Id = 1, Name = "نظافت و پذیرایی", Description = "انواع خدمات نظافت کار در منزل  و پذیرایی برای مجالس", ImagePath = "images\\HomeServices\\nezafat\\1.webp", CategoryId = 1 },
                new HomeService { Id = 2, Name = "شستشو", Description = "خدمات نظافت تخصصی", ImagePath = "images\\HomeServices\\shoshtosho\\1.webp", CategoryId = 1 },
                new HomeService { Id = 3, Name = "سرمایش و گرمایش", Description = "خدمات تعمیرات تخصصی سیستم های سرمایشی و گرمایشی", ImagePath = "images\\HomeServices\\saramyesh\\1.webp", CategoryId = 2 },
                new HomeService { Id = 4, Name = "تعمیرات ساختمان", Description = "خدمات تعمیرات مربوط به ساختمان و نما", ImagePath = "images\\HomeServices\\taamiratsakhteman\\1.webp", CategoryId = 2 },
                new HomeService { Id = 5, Name = "لوله کشی", Description = "خدمات تخصصی تعمیر لوله ساختمان", ImagePath = "images\\HomeServices\\lolekeshi\\1.webp", CategoryId = 2 },
                new HomeService { Id = 6, Name = "برقکاری", Description = "خدمات تخصصی برق و سیم کشی ساختمان", ImagePath = "images\\HomeServices\\barghkari\\1.webp", CategoryId = 2 },
                new HomeService { Id = 7, Name = "نصب و تعمیرات لوازم خانگی", Description = "تعمیرات تخصصی انواع لوازم های خانگی", ImagePath = "images\\HomeServices\\lavazemkhanegi\\1.webp", CategoryId = 3 },
                new HomeService { Id = 8, Name = "خدمات کامپیتوتری", Description = "تعمیرات تخصصی کامپیوتر های شخصی و لپ تاپ", ImagePath = "images\\HomeServices\\Computer\\1.webp", CategoryId = 3 },
                new HomeService { Id = 9, Name = "تعمیرات موبایل", Description = "تعمیرات تخصصی انواع تلفن همراه", ImagePath = "images\\HomeServices\\Mobile\\1.webp", CategoryId = 3 },
                new HomeService { Id = 10, Name = "باربری و جابجایی", Description = "خدمات تخصصی برای جابجایی و باربری", ImagePath = "images\\HomeServices\\barbari\\1.webp", CategoryId = 4 },
                new HomeService { Id = 11, Name = "خدمات و تعمیرات خودرو", Description = "تعمیرات تخصصی خودرو و وسیله های نقلیه", ImagePath = "images\\HomeServices\\khodro\\1.webp", CategoryId = 5 },
                new HomeService { Id = 12, Name = "کارواش و دیتیلینگ", Description = "خدمات نظافت خودرو", ImagePath = "images\\HomeServices\\carwash\\1.webp", CategoryId = 5 },
                new HomeService { Id = 13, Name = "پزشکی و پرستاری", Description = "خدمات پزشکی و درمانی", ImagePath = "images\\HomeServices\\pezeshki\\1.webp", CategoryId = 6 },
                new HomeService { Id = 14, Name = "حیوانات خانگی", Description = "خدمات دامپزشکی و حیوانات خانگی", ImagePath = "images\\HomeServices\\pet\\1.webp", CategoryId = 6 },
                new HomeService { Id = 15, Name = "خیاطی و تعیمرات لباس", Description = "خدمات و تعمیر انواع لباس های مردانه و زنانه", ImagePath = "images\\HomeServices\\khayati\\1.webp", CategoryId = 7 },
                new HomeService { Id = 16, Name = "مجالس و رویداد ها", Description = "خدمات مجالس و تشریفات", ImagePath = "images\\HomeServices\\event\\1.webp", CategoryId = 7 },
                new HomeService { Id = 17, Name = "آموزش", Description = "خدمات آموزشی", ImagePath = "images\\HomeServices\\amoozesh\\1.webp", CategoryId = 7 }

            );
        }
    }
}
