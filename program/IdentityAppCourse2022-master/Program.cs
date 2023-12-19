using IdentityAppCourse2022.Authorization;
using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Helpers;
using IdentityAppCourse2022.Interfaces;
using IdentityAppCourse2022.Models;
using IdentityAppCourse2022.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AlmurutStoreDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("AlmurutConnection")));
builder.Services.AddDbContext<AsiaStoreDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("AsiaConnection")));
builder.Services.AddDbContext<KivanoDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("KivanoConnection")));
builder.Services.AddDbContext<SoftTechDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("SoftTechConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ISendGridEmail, SendGridEmail>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddAuthentication()
.AddFacebook(options =>
{
    options.AppId = "test";
    options.AppSecret = "test";
})
.AddGoogle(options =>
{
    options.ClientId = "test";
    options.ClientSecret = "test";
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("CreateAccess", policy => policy.RequireClaim("create", "True"));
    options.AddPolicy("EditAccess", policy => policy.RequireClaim("edit", "True"));
    options.AddPolicy("DeleteAccess", policy => policy.RequireClaim("delete", "True"));

});
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireLowercase = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
    opt.Lockout.MaxFailedAccessAttempts = 5;
    //opt.SignIn.RequireConfirmedAccount = true;
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
