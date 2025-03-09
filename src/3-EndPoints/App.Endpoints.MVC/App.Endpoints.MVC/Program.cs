using App.Domain.AppServices.LocationAppServices;
using App.Domain.Core.Locations.Interfaces.IAppService;
using App.Domain.Core.Locations.Interfaces.IRepository;
using App.Domain.Core.Locations.Interfaces.IService;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Skills.Interfaces;
using App.Domain.Core.Skills.Interfaces.IAppServices;
using App.Domain.Core.Skills.Interfaces.IService;
using App.Domain.Core.Transactions.Interfaces.IAppService;
using App.Domain.Core.Transactions.Interfaces.IRepository;
using App.Domain.Core.Transactions.Interfaces.IService;
using App.Domain.Core.Users.AppServices;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Domain.Core.Users.Interfaces.IService;
using App.Domain.Core.Users.Services;
using App.Infrastructure.Db.SqlServer.Ef;
using App.Infrastructure.DbAccess.Repository.Ef.Repositories.Locations;
using App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services;
using App.Infrastructure.DbAccess.Repository.Ef.Repositories.Skills;
using App.Infrastructure.DbAccess.Repository.Ef.Repositories.Transactions;
using App.Infrastructure.DbAccess.Repository.Ef.Repositories.Users;
using HomeService.Domain.AppServices.AdminAppServices;
using HomeService.Domain.AppServices.CategoryAppServices;
using HomeService.Domain.AppServices.CustomerAppServices;
using HomeService.Domain.AppServices.Dashboard;
using HomeService.Domain.AppServices.HomeServiceAppServices;
using HomeService.Domain.AppServices.OrderAppServices;
using HomeService.Domain.AppServices.ProposalAppServices;
using HomeService.Domain.AppServices.RequestAppServices;
using HomeService.Domain.AppServices.ReviewAppServices;
using HomeService.Domain.AppServices.SkillAppServices;
using HomeService.Domain.AppServices.SubHomeSerAppServices;
using HomeService.Domain.AppServices.TransactionAppServices;
using HomeService.Domain.AppServices.UserAppServices;
using HomeService.Domain.Services.AdminServices;
using HomeService.Domain.Services.CategoryServices;
using HomeService.Domain.Services.CustomerServices;
using HomeService.Domain.Services.HomeServiceServices;
using HomeService.Domain.Services.LocationServices;
using HomeService.Domain.Services.OrderServices;
using HomeService.Domain.Services.ProposalServices;
using HomeService.Domain.Services.RequestServices;
using HomeService.Domain.Services.ReviewServices;
using HomeService.Domain.Services.SkillServices;
using HomeService.Domain.Services.SubHomeSerServices;
using HomeService.Domain.Services.TransactionServices;
using HomeService.Domain.Services.UserServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IHomeServiceRepository, HomeServiceRepository>();
builder.Services.AddScoped<ISubHomeServiceRepository, SubHomeServiceRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IExpertRepository, ExpertRepository>();
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IExpertSkillRepository, ExpertSkillRepository>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IAdminUserAppService, AdminUserAppService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<ISubHomeServiceService, SubHomeServiceService>();
builder.Services.AddScoped<ISubHomeServiceAppService, SubHomeServiceAppService>();
builder.Services.AddScoped<IHomeServiceService, HomeServiceService>();
builder.Services.AddScoped<IHomeServiceAppService, HomeServiceAppService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderAppService, OrderAppService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<IProposalAppService, ProposalAppService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewAppService, ReviewAppService>();
builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryAppService, CategoryAppService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestAppService, RequestAppService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILocationAppService, LocationAppService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionAppService, TransactionAppService>();
builder.Services.AddScoped<IExpertService, ExpertService>();
builder.Services.AddScoped<IExpertAppService, ExpertAppService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillAppService, SkillAppService>();



builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();