using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Infrastructure.Db.SqlServer.Ef.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experts_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeServices_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubHomeServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HomeServiceId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubHomeServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubHomeServices_HomeServices_HomeServiceId",
                        column: x => x.HomeServiceId,
                        principalTable: "HomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SubHomeServiceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EnvironmentImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_SubHomeServices_SubHomeServiceId",
                        column: x => x.SubHomeServiceId,
                        principalTable: "SubHomeServices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubHomeServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_SubHomeServices_SubHomeServiceId",
                        column: x => x.SubHomeServiceId,
                        principalTable: "SubHomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpertSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpertId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertSkills_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpertSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpertId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposals_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposals_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposals_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ExpertId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ProposalId = table.Column<int>(type: "int", nullable: true),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ExpertId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ExpertId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, "Customer", "CUSTOMER" },
                    { 2, null, "Expert", "EXPERT" },
                    { 3, null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccountBalance", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsConfirmed", "IsEnabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, 1000m, "3f01898b-ab43-4532-84a9-2d535f09d8b9", new DateTime(2025, 3, 7, 10, 3, 49, 469, DateTimeKind.Utc).AddTicks(8703), "admin@gmail.com", false, "Admin", true, true, "User", false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEBAbfgAlNw72yyrTPGrqz0VB+dualL4wwYyoua9s1IoKjCubauIRzBCbI8664jaAmA==", null, false, "images\\User\\Admin\\admin.png", 3, "36eb3ad0-b9f6-49fd-9529-250b48ab24a5", false, "admin@gmail.com" },
                    { 2, 0, 2000000m, "af3902cf-2fb3-4443-89a2-7b4d12a029b8", new DateTime(2025, 3, 7, 10, 3, 49, 525, DateTimeKind.Utc).AddTicks(8610), "ali@gmail.com", false, "علی", true, true, "عباسی", false, null, "ALI@GMAIL.COM", "ALI@GMAIL.COM", "AQAAAAIAAYagAAAAEMNqomD1Ue5fN5gn+s99zpzb53rUpgydkqyV3QkJhbAUY9GzZb5b5IbEc+YZ5zmBHg==", null, false, "images\\User\\Customer\\ali.jpg", 1, "26dba629-3910-4982-8fd1-07b8c1989480", false, "ali@gmail.com" },
                    { 3, 0, 2000000m, "daadc522-d356-4c6e-bc4d-b90d14a28567", new DateTime(2025, 3, 7, 10, 3, 49, 583, DateTimeKind.Utc).AddTicks(9999), "sina47@gmail.com", false, "سینا", false, true, "مرادی", false, null, "SINA47@GMAIL.COM", "SINA47@GMAIL.COM", "AQAAAAIAAYagAAAAEHLQyP7hZTgtanH6IE7yGAvPJ6xKZF6cFinzqHo/vRjVdA/gU7aQqnTJBjzWc+5b9w==", null, false, "images\\User\\Customer\\sina.png", 1, "07969a58-7e6f-4191-b684-fe653bb6189c", false, "sina47@gmail.com" },
                    { 4, 0, 750m, "52337636-c848-4217-899a-ab5030381a75", new DateTime(2025, 3, 7, 10, 3, 49, 639, DateTimeKind.Utc).AddTicks(2354), "shahin@gmail.com", false, "شاهین", false, true, "حسنی", false, null, "SHAHIN@GMAIL.COM", "SHAHIN@GMAIL.COM", "AQAAAAIAAYagAAAAEJysqcorUzG0rX80V4METmwWTwZRnJJkYKzKxPv9WRe7WZ3/rYm2kLJKgBzBDTLhBQ==", null, false, "images\\User\\Expert\\shahin.png", 2, "bb127e55-198c-4ba5-babc-fa9579b72348", false, "shahin@gmail.com" },
                    { 5, 0, 100m, "4fdb65b9-a8ea-4913-9a68-278631cd6cd4", new DateTime(2025, 3, 7, 10, 3, 49, 695, DateTimeKind.Utc).AddTicks(5026), "karimi@gmail.com", false, "فاطمه", false, true, "کریمی", false, null, "KARIMI@GMAIL.COM", "KARIMI@GMAIL.COM", "AQAAAAIAAYagAAAAEJauoyjsSP430xF65mgN0pnIuwRMIYRzWhd/2pBOXfxT3L7JYVpiVNuNxn15DXVejg==", null, false, "images\\User\\Expert\\fatemeh.png", 2, "4dec609e-1af3-4952-992e-af91a030c06a", false, "karimi@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "ImagePath", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "خدمات نظافتی وسیله نقلیه و منزل و شرکت‌ها", "images\\Categories\\tamizkari\\1.webp", true, "تمیزکاری" },
                    { 2, "خدمات مربوط به ساخت و ساز", "images\\Categories\\sakhteman\\1.webp", true, "ساختمان" },
                    { 3, "خدمات تعمیر لوازم شخصی و خانگی و تجهیزات الکترونیکی", "images\\Categories\\tamirat\\1.webp", true, "تعمیرات اشیا" },
                    { 4, "خدمات حمل بار و اسباب کشی و جابجایی اشیا", "images\\Categories\\asbabkeshi\\1.webp", true, "اسباب کشی" },
                    { 5, "خدمات تعمیر و سرویس و نظافت وسیله نقلیه", "images\\Categories\\khodro\\1.webp", true, "خودرو" },
                    { 6, "خدمات پزشکی و پرستاری و مشاوره", "images\\Categories\\salamti\\1.webp", true, "سلامت و زیبایی" },
                    { 7, "خدمات دیگر", "images\\Categories\\sayer\\1.webp", true, "سایر" }
                });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "آذربایجان شرقی" },
                    { 2, "آذربایجان غربی" },
                    { 3, "اردبیل" },
                    { 4, "اصفهان" },
                    { 5, "البرز" },
                    { 6, "ایلام" },
                    { 7, "بوشهر" },
                    { 8, "تهران" },
                    { 9, "چهارمحال و بختیاری" },
                    { 10, "خراسان جنوبی" },
                    { 11, "خراسان رضوی" },
                    { 12, "خراسان شمالی" },
                    { 13, "خوزستان" },
                    { 14, "زنجان" },
                    { 15, "سمنان" },
                    { 16, "سیستان و بلوچستان" },
                    { 17, "فارس" },
                    { 18, "قزوین" },
                    { 19, "قم" },
                    { 20, "کردستان" },
                    { 21, "کرمان" },
                    { 22, "کرمانشاه" },
                    { 23, "کهگیلویه و بویراحمد" },
                    { 24, "گلستان" },
                    { 25, "گیلان" },
                    { 26, "لرستان" },
                    { 27, "مازندران" },
                    { 28, "مرکزی" },
                    { 29, "هرمزگان" },
                    { 30, "همدان" },
                    { 31, "یزد" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 4 },
                    { 2, 5 }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name", "ProvinceId" },
                values: new object[,]
                {
                    { 1, "تبریز", 1 },
                    { 2, "ارومیه", 2 },
                    { 3, "اردبیل", 3 },
                    { 4, "اصفهان", 4 },
                    { 5, "کرج", 5 },
                    { 6, "ایلام", 6 },
                    { 7, "بوشهر", 7 },
                    { 8, "تهران", 8 },
                    { 9, "شهرکرد", 9 },
                    { 10, "بیرجند", 10 },
                    { 11, "مشهد", 11 },
                    { 12, "بجنورد", 12 },
                    { 13, "اهواز", 13 },
                    { 14, "زنجان", 14 },
                    { 15, "سمنان", 15 },
                    { 16, "زاهدان", 16 },
                    { 17, "شیراز", 17 },
                    { 18, "قزوین", 18 },
                    { 19, "قم", 19 },
                    { 20, "سنندج", 20 },
                    { 21, "کرمان", 21 },
                    { 22, "کرمانشاه", 22 },
                    { 23, "یاسوج", 23 },
                    { 24, "گرگان", 24 },
                    { 25, "رشت", 25 },
                    { 26, "خرم‌آباد", 26 },
                    { 27, "ساری", 27 },
                    { 28, "اراک", 28 },
                    { 29, "بندرعباس", 29 },
                    { 30, "همدان", 30 },
                    { 31, "یزد", 31 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "AppUserId", "City", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { 1, "شبنم یکم", 2, "تهران", "09121232165", "تهران" },
                    { 2, "پونک", 3, "تهران", "09195638521", "تهران" }
                });

            migrationBuilder.InsertData(
                table: "Experts",
                columns: new[] { "Id", "Address", "AppUserId", "City", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { 1, "کوچه مهناز", 4, "تهران", "09302226858", "تهران" },
                    { 2, "تهران پارس", 5, "تهران", "09356985214", "تهران" }
                });

            migrationBuilder.InsertData(
                table: "HomeServices",
                columns: new[] { "Id", "CategoryId", "Description", "ImagePath", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, "انواع خدمات نظافت کار در منزل  و پذیرایی برای مجالس", "images\\HomeServices\\nezafat\\1.webp", true, "نظافت و پذیرایی" },
                    { 2, 1, "خدمات نظافت تخصصی", "images\\HomeServices\\shoshtosho\\1.webp", true, "شستشو" },
                    { 3, 2, "خدمات تعمیرات تخصصی سیستم های سرمایشی و گرمایشی", "images\\HomeServices\\saramyesh\\1.webp", true, "سرمایش و گرمایش" },
                    { 4, 2, "خدمات تعمیرات مربوط به ساختمان و نما", "images\\HomeServices\\taamiratsakhteman\\1.webp", true, "تعمیرات ساختمان" },
                    { 5, 2, "خدمات تخصصی تعمیر لوله ساختمان", "images\\HomeServices\\lolekeshi\\1.webp", true, "لوله کشی" },
                    { 6, 2, "خدمات تخصصی برق و سیم کشی ساختمان", "images\\HomeServices\\barghkari\\1.webp", true, "برقکاری" },
                    { 7, 3, "تعمیرات تخصصی انواع لوازم های خانگی", "images\\HomeServices\\lavazemkhanegi\\1.webp", true, "نصب و تعمیرات لوازم خانگی" },
                    { 8, 3, "تعمیرات تخصصی کامپیوتر های شخصی و لپ تاپ", "images\\HomeServices\\Computer\\1.webp", true, "خدمات کامپیتوتری" },
                    { 9, 3, "تعمیرات تخصصی انواع تلفن همراه", "images\\HomeServices\\Mobile\\1.webp", true, "تعمیرات موبایل" },
                    { 10, 4, "خدمات تخصصی برای جابجایی و باربری", "images\\HomeServices\\barbari\\1.webp", true, "باربری و جابجایی" },
                    { 11, 5, "تعمیرات تخصصی خودرو و وسیله های نقلیه", "images\\HomeServices\\khodro\\1.webp", true, "خدمات و تعمیرات خودرو" },
                    { 12, 5, "خدمات نظافت خودرو", "images\\HomeServices\\carwash\\1.webp", true, "کارواش و دیتیلینگ" },
                    { 13, 6, "خدمات پزشکی و درمانی", "images\\HomeServices\\pezeshki\\1.webp", true, "پزشکی و پرستاری" },
                    { 14, 6, "خدمات دامپزشکی و حیوانات خانگی", "images\\HomeServices\\pet\\1.webp", true, "حیوانات خانگی" },
                    { 15, 7, "خدمات و تعمیر انواع لباس های مردانه و زنانه", "images\\HomeServices\\khayati\\1.webp", true, "خیاطی و تعیمرات لباس" },
                    { 16, 7, "خدمات مجالس و تشریفات", "images\\HomeServices\\event\\1.webp", true, "مجالس و رویداد ها" },
                    { 17, 7, "خدمات آموزشی", "images\\HomeServices\\amoozesh\\1.webp", true, "آموزش" }
                });

            migrationBuilder.InsertData(
                table: "SubHomeServices",
                columns: new[] { "Id", "BasePrice", "Description", "HomeServiceId", "ImagePath", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 300000m, "خدمات نظافت عادی ", 1, "images\\SubHomeServices\\tamizkari\\nezafat\\service adi.jpg", true, "سرویس عادی نظافت" },
                    { 2, 500000m, "خدمات نظافت ویژه", 1, "images\\SubHomeServices\\tamizkari\\nezafat\\service vizhe.jpg", true, "سرویس ویژه نظافت " },
                    { 3, 300000m, "نظافت راه پله های ساختمان مسکونی ", 1, "images\\SubHomeServices\\tamizkari\\nezafat\\rah pele.jpg", true, "نظافت راه پله" },
                    { 4, 300000m, "شستشوی فرش", 2, "images\\SubHomeServices\\tamizkari\\shostosho\\ghalishoie.jpg", true, "قالیشویی" },
                    { 5, 300000m, "خشکشویی انواع لباس های شما", 2, "images\\SubHomeServices\\tamizkari\\shostosho\\khoshkshoie.jpg", true, "خشکشویی" },
                    { 6, 300000m, "انواع خدمات شستشو مبل موکت تشک", 2, "images\\SubHomeServices\\tamizkari\\shostosho\\shostesho dar mahal.jpg", true, "شستشو در محل" },
                    { 7, 300000m, "انواع خدمات و تعمیرات تخصصی کولر آبی", 3, "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\coolerabi.jpg", true, "تعمیر کولر آبی و سرویس کولر آبی" },
                    { 8, 300000m, "تعمیر پکیج", 3, "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\pakgegarm.jpg", true, "تعمیر و سرویس پکیج" },
                    { 9, 300000m, "انواع خدمات و تعمیرات تخصصی رادیاتور", 3, "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\shofazh.jpg", true, "سرویس رادیاتور و تعمیر شوفاژ" },
                    { 10, 300000m, "خدمات بنایی", 4, "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\banaie.jpg", true, "بنایی" },
                    { 11, 300000m, "نصب کاغذ دیواری", 4, "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\kaqazdivari.jpg", true, "کاغذ دیواری" },
                    { 12, 300000m, "خدمات سنگ کاری", 4, "images\\SubHomeServices\\Sakhteman\\tamiratsakhteman\\sangkari.jpg", true, "سنگ کاری" },
                    { 13, 300000m, "نصب شیرآلات ساختمانی و بهداشتی", 5, "images\\SubHomeServices\\Sakhteman\\lole keshi\\shiralat.jpg", true, "شیرآلات" },
                    { 14, 300000m, "تخلیه چاه و لوله بازکنی", 5, "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\takhliye chah.jpg", true, "لوله بازکنی" },
                    { 15, 300000m, "تعمیر و سرویس پمپ آب خانگی", 5, "images\\SubHomeServices\\Sakhteman\\garmayesh sarmayesh\\pomp ab.jpg", true, "پمپ آب" },
                    { 16, 300000m, "سیم کشی ساختمان - برق کشی ساختمان", 6, "images\\SubHomeServices\\Sakhteman\\barghkari\\simkeshi.jpg", true, "سیم کشی" },
                    { 17, 300000m, "نصب و تعمیر لوستر", 6, "images\\SubHomeServices\\Sakhteman\\barghkari\\cheragh.jpg", true, "نصب چراغ" },
                    { 18, 300000m, "تعمیرات تخصصی یخچال", 7, "images\\SubHomeServices\\TamiratAshia\\nasb va taamirat\\2.jpg", true, "نصب و تعمیر یخچال" },
                    { 19, 300000m, "تعمیرات تخصصی ماشین لباسشویی", 7, "images\\SubHomeServices\\TamiratAshia\\nasb va taamirat\\1.jpg", true, "نصب و تعمیر ماشین لباسشویی" },
                    { 20, 300000m, "تعمیرات تخصصی کامپیوتر و لپتاپ", 8, "images\\SubHomeServices\\TamiratAshia\\khadamat computer\\1.webp", true, "تعمیر کامپیوتر و لپتاپ" },
                    { 21, 300000m, "خدمات تخصصی شبکه", 8, "images\\SubHomeServices\\TamiratAshia\\khadamat computer\\2.webp", true, "پشتیبانی شبکه و سرور" },
                    { 22, 300000m, "تعمیرات تخصصی تاچ و ال سی دی موبایل", 9, "images\\SubHomeServices\\TamiratAshia\\mobile\\2.jpg", true, "خدمات تاچ و ال سی دی" },
                    { 23, 300000m, "خدمات تخصصی و تعویض باتری موبایل", 9, "images\\SubHomeServices\\TamiratAshia\\mobile\\1.jpg", true, "خدمات باتری" },
                    { 24, 300000m, "حمل و باربری اشیای سنگین با خاور و کامیون", 10, "images\\SubHomeServices\\Barbari\\Asbab keshi khavar\\1.jpg", true, "اسباب کشی با خاور و کامیون" },
                    { 25, 300000m, "حمل و باربری اشیا با وانت و نیسان", 10, "images\\SubHomeServices\\Barbari\\Asbab keshi khavar\\2.jpg", true, "اسباب کشی با وانت و نیسان" },
                    { 26, 300000m, "خدمات تخصصی مکانیک خودرو", 11, "images\\SubHomeServices\\khodro\\khdamat khodro\\1.jpg", true, "مکانیکی خودرو" },
                    { 27, 300000m, "خدمات تخصصی تعویض باتری خودرو", 11, "images\\SubHomeServices\\khodro\\khdamat khodro\\2.jpg", true, "تعویض باتری خودرو" },
                    { 28, 300000m, "خدمات کارواش نانو وسیله نقلیه", 12, "images\\SubHomeServices\\khodro\\carwash\\1.jpg", true, "کارواش نانو" },
                    { 29, 300000m, "خدمات واکس و پولیش خودرو", 12, "images\\SubHomeServices\\khodro\\carwash\\2.jpg", true, "واکس و پولیش خودرو" },
                    { 30, 300000m, "خدمات معاینه پزشکی", 13, "images\\SubHomeServices\\salamati\\pezeshki\\1.jpg", true, "معاینه پزشکی" },
                    { 31, 300000m, "خدمات پرستاری و تزریقات در محل", 13, "images\\SubHomeServices\\salamati\\pezeshki\\2.jpg", true, "پرستاری و تزریقات" },
                    { 32, 300000m, "خدمات دامپزشکی انواع حیوانات خانگی", 14, "images\\SubHomeServices\\salamati\\dampezeshki\\1.jpg", true, "خدمات دامپزشکی در محل" },
                    { 33, 300000m, "خدمات تخصصی تعمیر انواع لباس", 15, "images\\SubHomeServices\\Sayer\\khayati lebas\\1.jpg", true, "تعمیرات لباس" },
                    { 34, 300000m, "خدمات تخصصی تعمیر انواع کیف و کفش", 15, "images\\SubHomeServices\\Sayer\\khayati lebas\\2.jpg", true, "تعمیر کیف و کفش" },
                    { 35, 300000m, "خدمات انواع کیک و شیرینی های مجالس و مراسم", 16, "images\\SubHomeServices\\Sayer\\Sayer\\event\\1.jpg", true, "کیک و شیرینی" },
                    { 36, 300000m, "خدمات ارسال هدیه", 16, "images\\SubHomeServices\\Sayer\\Sayer\\event\\2.jpg", true, "ارسال هدیه" },
                    { 37, 300000m, "انواع خدمات آموزشی  برای آمادگی کنکور", 17, "images\\SubHomeServices\\Sayer\\amoozesh\\1.jpg", true, "آمادگی برای کنکور" },
                    { 38, 300000m, "خدمات آموزشی انواع زبان های خارجی", 17, "images\\SubHomeServices\\Sayer\\amoozesh\\2.jpg", true, "آموزش زبان های خارجی" }
                });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "Deadline", "Description", "EnvironmentImagePath", "ExecutionDate", "IsEnabled", "Status", "SubHomeServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(827), 1, new DateTime(2025, 3, 12, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(821), "درخواست بنایی ساختمان", null, new DateTime(2025, 3, 10, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(827), true, 0, 10 },
                    { 2, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(831), 1, new DateTime(2025, 3, 14, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(830), "درخواست کاغذ دیواری", null, new DateTime(2025, 3, 12, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(830), true, 0, 11 },
                    { 3, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(834), 2, new DateTime(2025, 3, 17, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(833), "درخواست سنگ کاری", null, new DateTime(2025, 3, 15, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(833), true, 0, 12 }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Name", "SubHomeServiceId" },
                values: new object[,]
                {
                    { 1, "بنایی", 10 },
                    { 2, "کاغذ دیواری", 11 },
                    { 3, "سنگ کاری", 12 }
                });

            migrationBuilder.InsertData(
                table: "ExpertSkills",
                columns: new[] { "Id", "ExpertId", "SkillId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "Proposals",
                columns: new[] { "Id", "CreatedAt", "Description", "ExecutionDate", "ExpertId", "IsEnabled", "OrderId", "Price", "RequestId", "ResponseTime", "SkillId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6414), "پیشنهاد انجام خدمات برای درخواست بنایی", new DateTime(2025, 3, 12, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6408), 1, true, null, 450000m, 1, new DateTime(2025, 3, 8, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6412), 1, 0 },
                    { 2, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6419), "پیشنهاد انجام خدمات برای درخواست کاغذ دیواری", new DateTime(2025, 3, 10, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6417), 1, true, null, 600000m, 2, new DateTime(2025, 3, 8, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6418), 2, 0 }
                });

            migrationBuilder.InsertData(
                table: "Proposals",
                columns: new[] { "Id", "CreatedAt", "Description", "ExecutionDate", "ExpertId", "OrderId", "Price", "RequestId", "ResponseTime", "SkillId", "Status" },
                values: new object[] { 3, new DateTime(2025, 3, 7, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6422), "پیشنهاد انجام خدمات برای درخواست سنگ کاری", new DateTime(2025, 3, 14, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6421), 2, null, 780000m, 3, new DateTime(2025, 3, 9, 10, 3, 49, 467, DateTimeKind.Utc).AddTicks(6421), 3, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AppUserId",
                table: "Admins",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AppUserId",
                table: "Customers",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experts_AppUserId",
                table: "Experts",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpertSkills_ExpertId",
                table: "ExpertSkills",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertSkills_SkillId",
                table: "ExpertSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeServices_CategoryId",
                table: "HomeServices",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExpertId",
                table: "Orders",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProposalId",
                table: "Orders",
                column: "ProposalId",
                unique: true,
                filter: "[ProposalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RequestId",
                table: "Orders",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ExpertId",
                table: "Proposals",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_RequestId",
                table: "Proposals",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_SkillId",
                table: "Proposals",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CustomerId",
                table: "Requests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SubHomeServiceId",
                table: "Requests",
                column: "SubHomeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ExpertId",
                table: "Reviews",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SubHomeServiceId",
                table: "Skills",
                column: "SubHomeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SubHomeServices_HomeServiceId",
                table: "SubHomeServices",
                column: "HomeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExpertId",
                table: "Transactions",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ExpertSkills");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DropTable(
                name: "Experts");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SubHomeServices");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HomeServices");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
