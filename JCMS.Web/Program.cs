using System.Security.Claims;
using JCMS.Application.Services;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<JcmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("JCMSDb")));

// Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IJewelryItemRepository,  JewelryItemRepository>();
builder.Services.AddScoped<ICleaningOrderRepository, CleaningOrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICleaningHistoryRepository, CleaningHistoryRepository>();

// Services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JewelryItemService>();
builder.Services.AddScoped<CleaningOrderService>();

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); //NFR6 logout after 30 min
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Seed default admin
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    authService.SeedAdminIfNeeded();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


app.Run();
