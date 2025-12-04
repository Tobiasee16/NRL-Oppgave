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
    .AddRoles<IdentityRole>()                             // vi bruker roller (Admin, Registerforer, Pilot)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ------------------------
//  Identity-cookie (login-cookie)
// ------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    // Hvor brukeren sendes hvis de må logge inn
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    // Viktig for at cookies funker fint i dev/Docker
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;              // Løsner litt på SameSite, ellers kan cookies blokkeres
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;   // Tillat HTTP (greit i dev uten HTTPS)
    options.SlidingExpiration = true;                        // Forlenger levetid når brukeren er aktiv
});


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
//  Migrer DB + seed roller / admin / registerfører
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeedIdentityDataAsync(roleMgr, userMgr);
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


// =========================================================
//  Seeding av roller + Admin + Registerfører + Pilot
// =========================================================

static async Task SeedIdentityDataAsync(
    RoleManager<IdentityRole> roleMgr,
    UserManager<IdentityUser> userMgr)
{
    // Roller (systemroller)
    string[] roles = { "Admin", "Registerforer", "Pilot" };

    foreach (var role in roles)
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));
    }

    // ------------------------
    // 1) Admin-bruker
    // ------------------------
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

        await userMgr.CreateAsync(adminUser, "AdminPassw0rd!");
    }

    // Sørg for at admin faktisk har rollen
    if (!await userMgr.IsInRoleAsync(adminUser, "Admin"))
    {
        await userMgr.AddToRoleAsync(adminUser, "Admin");
    }

    // ------------------------
    // 2) Registerfører-bruker (dummy)
    // ------------------------
    var regEmail = "registerforer@example.com";
    var regUser = await userMgr.FindByEmailAsync(regEmail);

    if (regUser is null)
    {
        regUser = new IdentityUser
        {
            UserName = regEmail,
            Email = regEmail,
            EmailConfirmed = true
        };

        await userMgr.CreateAsync(regUser, "RegisterPassw0rd!");
    }

    // 🔴 VIKTIG: alltid sjekk/legg til rolle, også hvis brukeren fantes fra før
    if (!await userMgr.IsInRoleAsync(regUser, "Registerforer"))
    {
        await userMgr.AddToRoleAsync(regUser, "Registerforer");
    }
 } 