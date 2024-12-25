using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�rma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yap�land�rmas�: Kullan�c� ve rol yap�land�rmas�
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    // �ifre politikas�
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
    options.LoginPath = "/Kullanici/Giris"; // Kullan�c� giri�i sayfas� yolu
    options.AccessDeniedPath = "/Home/AccessDenied"; // Eri�im reddi sayfas� yolu
    options.LogoutPath = "/Kullanici/Logout"; // Kullan�c� ��k��� i�in y�nlendirme

    // Admin kullan�c� i�in �zel bir y�nlendirme
    options.Events.OnRedirectToLogin = context =>
    {
        // E�er admin sayfas�na eri�meye �al���l�yorsa
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.Redirect("/Admin/Login"); // Admin login sayfas�na y�nlendir
        }
        else
        {
            context.Response.Redirect("/Kullanici/Giris"); // Kullan�c� login sayfas�na y�nlendir
        }
        return Task.CompletedTask;
    };
});


// Controllers ve Views deste�i ekleme
builder.Services.AddControllersWithViews();

// Uygulama yap�land�rmas�n� tamamlamak
var app = builder.Build();

// Hata sayfas� ve HSTS y�netimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Uygulama i�leme s�ras�
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kimlik do�rulama ve yetkilendirme i�lemleri
app.UseAuthentication();
app.UseAuthorization();

// Default route yap�land�rmas�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritaban� seed i�lemleri
await SeedDatabase(app);

// Uygulama ba�latma
app.Run();

// Veritaban� seed i�lemi
static async Task SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<Kullanici>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Admin ve User rolleri olu�turuluyor
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullan�c�s� var m� kontrol�
        var adminUser = await userManager.FindByEmailAsync("b221210089@sakarya.edu.tr");
        if (adminUser == null)
        {
            adminUser = new Kullanici
            {
                UserName = "b221210089@sakarya.edu.tr",
                Email = "b221210089@sakarya.edu.tr"
            };

            var createResult = await userManager.CreateAsync(adminUser, "sau");
            if (createResult.Succeeded)
            {
                // Admin rol�n� ata
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            // E�er kullan�c� zaten varsa, Admin rol�ne ekle
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // User rol� i�in bir kullan�c� olu�turuluyor (�rnek kullan�c�)
        var user = await userManager.FindByEmailAsync("user@example.com");
        if (user == null)
        {
            user = new Kullanici
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                KullaniciAd = "�rnek",
                KullaniciSoyad = "Kullan�c�",
                KullaniciTelefon = "5551234567"
            };

            var createResult = await userManager.CreateAsync(user, "userpassword");
            if (createResult.Succeeded)
            {
                // User rol�n� ata
                await userManager.AddToRoleAsync(user, "User");
            }
        }
        else
        {
            // E�er kullan�c� zaten varsa, User rol�ne ekle
            if (!await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
