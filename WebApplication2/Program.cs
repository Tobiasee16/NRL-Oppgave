using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data; // ApplicationDbContext + dine Dapper-interfaces/klasser

var builder = WebApplication.CreateBuilder(args);

// ---------- EF Core for Identity (bruker connection string fra appsettings.json) ----------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 6, 0)) // MariaDB 10.6+
    ));

// ---------- ASP.NET Core Identity ----------
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()                       // fjern denne linjen hvis du ikke vil bruke roller
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ---------- MVC ----------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // nødvendig for Identity UI (/Identity/Account/...)

// ---------- Repository (Dapper + MySqlConnector inni repo-klassen) ----------
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<IObstacleRepository, ObstacleRepository>();

var app = builder.Build();

// ---------- Pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // VIKTIG: må komme før UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Aktiver Identity sine innebygde sider (Login/Register/Logout)
app.MapRazorPages();

// (valgfritt) seed roller + admin første gang appen kjører
await SeedRolesAndAdminAsync(app.Services);

app.Run();

static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = ["Admin", "Registerfoerer", "Pilot"];
    foreach (var r in roles)
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    var adminEmail = "admin@example.com";
    var adminUser = await userMgr.FindByEmailAsync(adminEmail);
    if (adminUser is null)
    {
        var u = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var ok = await userMgr.CreateAsync(u, "AdminPassw0rd!");
        if (ok.Succeeded)
            await userMgr.AddToRoleAsync(u, "Admin");
    }
}
