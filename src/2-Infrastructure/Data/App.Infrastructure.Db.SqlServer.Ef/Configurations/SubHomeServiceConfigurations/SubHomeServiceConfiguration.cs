using App.Domain.Core.Services.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.SubHomeServiceConfigurations
{
    public class SubHomeServiceConfiguration : IEntityTypeConfiguration<SubHomeService>
    {
        public void Configure(EntityTypeBuilder<SubHomeService> builder)
        {
            builder.HasKey(shs => shs.Id);

            builder.ToTable("SubHomeServices");

            builder.Property(shs => shs.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(shs => shs.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(shs => shs.Views)
                .HasDefaultValue(0);

            builder.Property(shs => shs.BasePrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(shs => shs.ImagePath)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(shs => shs.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(shs => shs.HomeService)
                .WithMany(hs => hs.SubHomeServices)
                .HasForeignKey(shs => shs.HomeServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(shs => shs.Requests)
                .WithOne(r => r.SubHomeService)
                .HasForeignKey(r => r.SubHomeServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(shs => shs.Skills)
                .WithOne(s => s.SubHomeService)
                .HasForeignKey(s => s.SubHomeServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new SubHomeService { Id = 1, Name = "سرویس عادی نظافت", Description = "خدمات نظافت عادی ", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\tamizkari\\nezafat\\service adi.jpg", HomeServiceId = 1 },
                new SubHomeService { Id = 2, Name = "سرویس ویژه نظافت ", Description = "خدمات نظافت ویژه", Views = 0, BasePrice = 500000, ImagePath = "images\\SubHomeServices\\tamizkari\\nezafat\\service vizhe.jpg", HomeServiceId = 1 },
                new SubHomeService { Id = 3, Name = "نظافت راه پله", Description = "نظافت راه پله های ساختمان مسکونی ", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\tamizkari\\nezafat\\rah pele.jpg", HomeServiceId = 1 },

                new SubHomeService { Id = 4, Name = "قالیشویی", Description = "شستشوی فرش", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\tamizkari\\shostosho\\ghalishoie.jpg", HomeServiceId = 2 },
                new SubHomeService { Id = 5, Name = "خشکشویی", Description = "خشکشویی انواع لباس های شما", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\tamizkari\\shostosho\\khoshkshoie.jpg", HomeServiceId = 2 },
                new SubHomeService { Id = 6, Name = "شستشو در محل", Description = "انواع خدمات شستشو مبل موکت تشک", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\tamizkari\\shostosho\\shostesho dar mahal.jpg", HomeServiceId = 2 },

                new SubHomeService { Id = 7, Name = "تعمیر کولر آبی و سرویس کولر آبی", Description = "انواع خدمات و تعمیرات تخصصی کولر آبی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\coolerabi.jpg", HomeServiceId = 3 },
                new SubHomeService { Id = 8, Name = "تعمیر و سرویس پکیج", Description = "تعمیر پکیج", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\pakgegarm.jpg", HomeServiceId = 3 },
                new SubHomeService { Id = 9, Name = "سرویس رادیاتور و تعمیر شوفاژ", Description = "انواع خدمات و تعمیرات تخصصی رادیاتور", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\shofazh.jpg", HomeServiceId = 3 },

                new SubHomeService { Id = 10, Name = "بنایی", Description = "خدمات بنایی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\banaie.jpg", HomeServiceId = 4 },
                new SubHomeService { Id = 11, Name = "کاغذ دیواری", Description = "نصب کاغذ دیواری", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\kaqazdivari.jpg", HomeServiceId = 4 },
                new SubHomeService { Id = 12, Name = "سنگ کاری", Description = "خدمات سنگ کاری", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\sangkari.jpg", HomeServiceId = 4 },

                new SubHomeService { Id = 13, Name = "شیرآلات", Description = "نصب شیرآلات ساختمانی و بهداشتی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\lole keshi\\shiralat.jpg", HomeServiceId = 5 },
                new SubHomeService { Id = 14, Name = "لوله بازکنی", Description = "تخلیه چاه و لوله بازکنی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\takhliye chah.jpg", HomeServiceId = 5 },
                new SubHomeService { Id = 15, Name = "پمپ آب", Description = "تعمیر و سرویس پمپ آب خانگی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\pomp ab.jpg", HomeServiceId = 5 },

                new SubHomeService { Id = 16, Name = "سیم کشی", Description = "سیم کشی ساختمان - برق کشی ساختمان", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\barghkari\\simkeshi.jpg", HomeServiceId = 6 },
                new SubHomeService { Id = 17, Name = "نصب چراغ", Description = "نصب و تعمیر لوستر", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sakhteman\\barghkari\\cheragh.jpg", HomeServiceId = 6 },

                new SubHomeService { Id = 18, Name = "نصب و تعمیر یخچال", Description = "تعمیرات تخصصی یخچال", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\nasb va taamirat\\2.jpg", HomeServiceId = 7 },
                new SubHomeService { Id = 19, Name = "نصب و تعمیر ماشین لباسشویی", Description = "تعمیرات تخصصی ماشین لباسشویی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\nasb va taamirat\\1.jpg", HomeServiceId = 7 },

                new SubHomeService { Id = 20, Name = "تعمیر کامپیوتر و لپتاپ", Description = "تعمیرات تخصصی کامپیوتر و لپتاپ", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\khadamat computer\\1.webp", HomeServiceId = 8 },
                new SubHomeService { Id = 21, Name = "پشتیبانی شبکه و سرور", Description = "خدمات تخصصی شبکه", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\khadamat computer\\2.webp", HomeServiceId = 8 },

                new SubHomeService { Id = 22, Name = "خدمات تاچ و ال سی دی", Description = "تعمیرات تخصصی تاچ و ال سی دی موبایل", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\mobile\\2.jpg", HomeServiceId = 9 },
                new SubHomeService { Id = 23, Name = "خدمات باتری", Description = "خدمات تخصصی و تعویض باتری موبایل", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\TamiratAshia\\mobile\\1.jpg", HomeServiceId = 9 },

                new SubHomeService { Id = 24, Name = "اسباب کشی با خاور و کامیون", Description = "حمل و باربری اشیای سنگین با خاور و کامیون", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Barbari\\Asbab keshi khavar\\1.jpg", HomeServiceId = 10 },
                new SubHomeService { Id = 25, Name = "اسباب کشی با وانت و نیسان", Description = "حمل و باربری اشیا با وانت و نیسان", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Barbari\\Asbab keshi khavar\\2.jpg", HomeServiceId = 10 },

                new SubHomeService { Id = 26, Name = "مکانیکی خودرو", Description = "خدمات تخصصی مکانیک خودرو", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\khodro\\khdamat khodro\\1.jpg", HomeServiceId = 11 },
                new SubHomeService { Id = 27, Name = "تعویض باتری خودرو", Description = "خدمات تخصصی تعویض باتری خودرو", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\khodro\\khdamat khodro\\2.jpg", HomeServiceId = 11 },

                new SubHomeService { Id = 28, Name = "کارواش نانو", Description = "خدمات کارواش نانو وسیله نقلیه", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\khodro\\carwash\\1.jpg", HomeServiceId = 12 },
                new SubHomeService { Id = 29, Name = "واکس و پولیش خودرو", Description = "خدمات واکس و پولیش خودرو", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\khodro\\carwash\\2.jpg", HomeServiceId = 12 },

                new SubHomeService { Id = 30, Name = "معاینه پزشکی", Description = "خدمات معاینه پزشکی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\salamati\\pezeshki\\1.jpg", HomeServiceId = 13 },
                new SubHomeService { Id = 31, Name = "پرستاری و تزریقات", Description = "خدمات پرستاری و تزریقات در محل", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\salamati\\pezeshki\\2.jpg", HomeServiceId = 13 },

                new SubHomeService { Id = 32, Name = "خدمات دامپزشکی در محل", Description = "خدمات دامپزشکی انواع حیوانات خانگی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\salamati\\dampezeshki\\1.jpg", HomeServiceId = 14 },

                new SubHomeService { Id = 33, Name = "تعمیرات لباس", Description = "خدمات تخصصی تعمیر انواع لباس", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\khayati lebas\\1.jpg", HomeServiceId = 15 },
                new SubHomeService { Id = 34, Name = "تعمیر کیف و کفش", Description = "خدمات تخصصی تعمیر انواع کیف و کفش", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\khayati lebas\\2.jpg", HomeServiceId = 15 },

                new SubHomeService { Id = 35, Name = "کیک و شیرینی", Description = "خدمات انواع کیک و شیرینی های مجالس و مراسم", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\Sayer\\event\\1.jpg", HomeServiceId = 16 },
                new SubHomeService { Id = 36, Name = "ارسال هدیه", Description = "خدمات ارسال هدیه", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\Sayer\\event\\2.jpg", HomeServiceId = 16 },

                new SubHomeService { Id = 37, Name = "آمادگی برای کنکور", Description = "انواع خدمات آموزشی  برای آمادگی کنکور", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\amoozesh\\1.jpg", HomeServiceId = 17 },
                new SubHomeService { Id = 38, Name = "آموزش زبان های خارجی", Description = "خدمات آموزشی انواع زبان های خارجی", Views = 0, BasePrice = 300000, ImagePath = "images\\SubHomeServices\\Sayer\\amoozesh\\2.jpg", HomeServiceId = 17 }

            );
        }
    }
}
