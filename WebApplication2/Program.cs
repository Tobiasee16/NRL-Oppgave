using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WebApplication2.Data;   // ApplicationDbContext
using WebApplication2.Models; // hvis du har modeller her

var builder = WebApplication.CreateBuilder(args);

// ------------------------
//  Connection string
// ------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();


// ------------------------
//  EF Core + MariaDB
// ------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(10, 6, 0))   // MariaDB 10.6+ (ok også for 11.x)
    ));

// ------------------------
//  ASP.NET Core Identity
// ------------------------
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()                             // vi bruker roller (Admin osv.)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ------------------------
//  MVC + Razor Pages (for Identity UI)
// ------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // /Identity/Account/Login osv.

// ------------------------
//  Dapper-repository
// ------------------------
builder.Services.AddScoped<IObstacleRepository, ObstacleRepository>();

var app = builder.Build();

// ------------------------
//  Migrer DB + seed roller/admin
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeedRolesAndAdminAsync(roleMgr, userMgr);
}

// ------------------------
//  HTTP-pipeline
// ------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // må komme før Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();       // Identity UI

app.Run();

// ------------------------
//  Seeding av roller + admin
// ------------------------
static async Task SeedRolesAndAdminAsync(
    RoleManager<IdentityRole> roleMgr,
    UserManager<IdentityUser> userMgr)
{
    // Rollene du vil ha i systemet
    string[] roles = { "Admin", "Registerfoerer", "Pilot" };

    foreach (var role in roles)
    {
        if (!await roleMgr.RoleExistsAsync(role))
        {
            await roleMgr.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin-bruker
    var adminEmail = "admin@example.com";
    var adminUser = await userMgr.FindByEmailAsync(adminEmail);

    if (adminUser is null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userMgr.CreateAsync(adminUser, "AdminPassw0rd!");

        if (result.Succeeded)
        {
            await userMgr.AddToRoleAsync(adminUser, "Admin");
        }
        // Hvis du vil, kan du logge/feilhåndtere hvis det ikke lykkes
    }
}

