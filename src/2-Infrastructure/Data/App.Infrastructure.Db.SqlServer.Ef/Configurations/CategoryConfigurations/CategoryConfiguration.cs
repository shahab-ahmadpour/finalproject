using App.Domain.Core.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.CategoryConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.ToTable("Categories");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(255);

            builder.Property(c => c.ImagePath)
                .HasMaxLength(255);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.HasMany(c => c.HomeServices)
                .WithOne(h => h.Category)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Category { Id = 1, Name = "تمیزکاری", Description = "خدمات نظافتی وسیله نقلیه و منزل و شرکت‌ها", ImagePath = "images\\Categories\\tamizkari\\1.webp" },
                new Category { Id = 2, Name = "ساختمان", Description = "خدمات مربوط به ساخت و ساز", ImagePath= "images\\Categories\\sakhteman\\1.webp" },
                new Category { Id = 3, Name = "تعمیرات اشیا", Description ="خدمات تعمیر لوازم شخصی و خانگی و تجهیزات الکترونیکی", ImagePath = "images\\Categories\\tamirat\\1.webp" },
                new Category { Id = 4, Name = "اسباب کشی", Description = "خدمات حمل بار و اسباب کشی و جابجایی اشیا", ImagePath = "images\\Categories\\asbabkeshi\\1.webp" },
                new Category { Id = 5, Name = "خودرو", Description = "خدمات تعمیر و سرویس و نظافت وسیله نقلیه", ImagePath = "images\\Categories\\khodro\\1.webp" },
                new Category { Id = 6, Name = "سلامت و زیبایی", Description = "خدمات پزشکی و پرستاری و مشاوره" , ImagePath = "images\\Categories\\salamti\\1.webp" },
                new Category { Id = 7, Name = "سایر", Description = "خدمات دیگر", ImagePath = "images\\Categories\\sayer\\1.webp" }
            );
        }
    }
}
