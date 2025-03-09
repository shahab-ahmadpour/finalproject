using App.Domain.Core.Services.Entities;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Transactions.Entities;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.CategoryConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.ExpertSkillConfigurations;
using Microsoft.EntityFrameworkCore;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.SkillConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.UserConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.OrderConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.RequestConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.TransactionConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.ProposalConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.ReviewConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.SubHomeServiceConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.HomeServiceConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using App.Domain.Core.Enums;
using App.Domain.Core.Locations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.CityConfigurations;
using App.Infrastructure.Db.SqlServer.Ef.Configurations.ProvinceConfigurations;

namespace App.Infrastructure.Db.SqlServer.Ef
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ExpertSkill> ExpertSkills { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HomeService> HomeServices { get; set; }
        public DbSet<SubHomeService> SubHomeServices { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }


        public DbSet<IdentityRole<int>> Roles { get; set; }
        public DbSet<IdentityUserRole<int>> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new ExpertConfiguration());
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new HomeServiceConfiguration());
            modelBuilder.ApplyConfiguration(new SubHomeServiceConfiguration());
            modelBuilder.ApplyConfiguration(new RequestConfiguration());
            modelBuilder.ApplyConfiguration(new ProposalConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new ExpertSkillConfiguration());
            modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());

            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole<int> { Id = 2, Name = "Expert", NormalizedName = "EXPERT" },
                new IdentityRole<int> { Id = 3, Name = "Admin", NormalizedName = "ADMIN" }
            );



            var passwordHasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@gmail.com",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                AccountBalance = 1000,
                ProfilePicture = "images\\User\\Admin\\admin.png",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                IsConfirmed = true,
                Role = UserRole.Admin,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Aa12345")
            };

            var User1 = new AppUser
            {
                Id = 2,
                FirstName = "علی",
                LastName = "عباسی",
                Email = "ali@gmail.com",
                UserName = "ali@gmail.com",
                NormalizedUserName = "ALI@GMAIL.COM",
                NormalizedEmail = "ALI@GMAIL.COM",
                AccountBalance = 2000000,
                ProfilePicture = "images\\User\\Customer\\ali.jpg",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                IsConfirmed = true,
                Role = UserRole.Customer,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Aa12345")
            };

            var User2 = new AppUser
            {
                Id = 3,
                FirstName = "سینا",
                LastName = "مرادی",
                Email = "sina47@gmail.com",
                UserName = "sina47@gmail.com",
                NormalizedUserName = "SINA47@GMAIL.COM",
                NormalizedEmail = "SINA47@GMAIL.COM",
                AccountBalance = 2000000,
                ProfilePicture = "images\\User\\Customer\\sina.png",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                Role = UserRole.Customer,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Ss12345")
            };

            var User3 = new AppUser
            {
                Id = 4,
                FirstName = "شاهین",
                LastName = "حسنی",
                Email = "shahin@gmail.com",
                UserName = "shahin@gmail.com",
                NormalizedUserName = "SHAHIN@GMAIL.COM",
                NormalizedEmail = "SHAHIN@GMAIL.COM",
                AccountBalance = 750,
                ProfilePicture = "images\\User\\Expert\\shahin.png",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                Role = UserRole.Expert,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Ss12345")
            };

            var User4 = new AppUser
            {
                Id = 5,
                FirstName = "فاطمه",
                LastName = "کریمی",
                Email = "karimi@gmail.com",
                UserName = "karimi@gmail.com",
                NormalizedUserName = "KARIMI@GMAIL.COM",
                NormalizedEmail = "KARIMI@GMAIL.COM",
                AccountBalance = 100,
                ProfilePicture = "images\\User\\Expert\\fatemeh.png",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                Role = UserRole.Expert,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Ff12345")
            };

            modelBuilder.Entity<AppUser>().HasData(adminUser, User1, User2, User3, User4);


            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { UserId = 1, RoleId = 3 },
                new IdentityUserRole<int> { UserId = 2, RoleId = 1 },
                new IdentityUserRole<int> { UserId = 3, RoleId = 1 },
                new IdentityUserRole<int> { UserId = 4, RoleId = 2 },
                new IdentityUserRole<int> { UserId = 5, RoleId = 2 }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, AppUserId = 2, PhoneNumber = "09121232165", Address = "شبنم یکم", City = "تهران", State = "تهران" },
                new Customer { Id = 2, AppUserId = 3, PhoneNumber = "09195638521", Address = "پونک", City = "تهران", State = "تهران" }
            );

            modelBuilder.Entity<Expert>().HasData(
                new Expert { Id = 1, AppUserId = 4, PhoneNumber = "09302226858", Address = "کوچه مهناز", City = "تهران", State = "تهران" },
                new Expert { Id = 2, AppUserId = 5, PhoneNumber = "09356985214", Address = "تهران پارس", City = "تهران", State = "تهران" }
            );


            modelBuilder.ApplyConfiguration(new AppUserConfiguration());



        }
    }
}
