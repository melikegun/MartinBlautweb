using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using MartinBlautweb.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�rma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yap�land�rmas�: Kullan�c� ve rol yap�land�rmas�
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3; // Minimum �ifre uzunlu�u
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Kullanici/Giris";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.LogoutPath = "/Kullanici/Logout";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.Redirect("/Admin/Login");
        }
        else
        {
            context.Response.Redirect("/Kullanici/Giris");
        }
        return Task.CompletedTask;
    };
});

// Controllers ve Views deste�i
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Hata y�netimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware s�ras�
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritaban� seed i�lemleri
await SeedDatabase(app);

app.Run();

// Veritaban� seed i�lemi
static async Task SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<Kullanici>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Rol olu�turulamad�: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        var adminUser = await userManager.FindByEmailAsync("b221210089@sakarya.edu.tr");
        if (adminUser == null)
        {
            adminUser = new Kullanici
            {
                UserName = "b221210089@sakarya.edu.tr",
                Email = "b221210089@sakarya.edu.tr",
                KullaniciAd = "Admin",
                KullaniciSoyad = "Admin",
                KullaniciTelefon = "00000000000" // Zorunlu alan dolduruldu
            };


            var createResult = await userManager.CreateAsync(adminUser, "sau");
            if (!createResult.Succeeded)
            {
                throw new Exception($"Admin kullan�c� olu�turulamad�: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
