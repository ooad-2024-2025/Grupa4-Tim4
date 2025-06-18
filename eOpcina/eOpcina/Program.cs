using eOpcina.Data;
using eOpcina.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHostedService<LicnaKartaExpiryChecker>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Error";
});

builder.Services.AddDefaultIdentity<Korisnik>(options =>
{  
    options.SignIn.RequireConfirmedAccount = false;

    //Ovo sam dodao 1.dan u 15:54 da bi se koristio Identity framework
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 100);// koliko traje zakljucavanje : beskonacno
    options.Lockout.MaxFailedAccessAttempts = 5; 
    options.Lockout.AllowedForNewUsers = true; // da li se primjenjuje i na nove korisnike
    //Ovo sam dodao 1.dan u 15:54 da bi se koristio Identity framework
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseStatusCodePagesWithReExecute("/Error/{0}");


app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
