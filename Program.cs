using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýlandýrma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý: Kullanýcý ve rol yapýlandýrmasý
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    // Þifre politikasý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3; // Minimum þifre uzunluðu
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Kullanici/Giris"; // Kullanýcý giriþi sayfasý yolu
    options.AccessDeniedPath = "/Home/AccessDenied"; // Eriþim reddi sayfasý yolu
    options.LogoutPath = "/Kullanici/Logout"; // Kullanýcý çýkýþý için yönlendirme

    // Admin kullanýcý için özel bir yönlendirme
    options.Events.OnRedirectToLogin = context =>
    {
        // Eðer admin sayfasýna eriþmeye çalýþýlýyorsa
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.Redirect("/Admin/Login"); // Admin login sayfasýna yönlendir
        }
        else
        {
            context.Response.Redirect("/Kullanici/Giris"); // Kullanýcý login sayfasýna yönlendir
        }
        return Task.CompletedTask;
    };
});


// Controllers ve Views desteði ekleme
builder.Services.AddControllersWithViews();

// Uygulama yapýlandýrmasýný tamamlamak
var app = builder.Build();

// Hata sayfasý ve HSTS yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Uygulama iþleme sýrasý
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kimlik doðrulama ve yetkilendirme iþlemleri
app.UseAuthentication();
app.UseAuthorization();

// Default route yapýlandýrmasý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritabaný seed iþlemleri
await SeedDatabase(app);

// Uygulama baþlatma
app.Run();

// Veritabaný seed iþlemi
static async Task SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<Kullanici>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Admin ve User rolleri oluþturuluyor
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullanýcýsý var mý kontrolü
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
                // Admin rolünü ata
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            // Eðer kullanýcý zaten varsa, Admin rolüne ekle
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // User rolü için bir kullanýcý oluþturuluyor (Örnek kullanýcý)
        var user = await userManager.FindByEmailAsync("user@example.com");
        if (user == null)
        {
            user = new Kullanici
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                KullaniciAd = "Örnek",
                KullaniciSoyad = "Kullanýcý",
                KullaniciTelefon = "5551234567"
            };

            var createResult = await userManager.CreateAsync(user, "userpassword");
            if (createResult.Succeeded)
            {
                // User rolünü ata
                await userManager.AddToRoleAsync(user, "User");
            }
        }
        else
        {
            // Eðer kullanýcý zaten varsa, User rolüne ekle
            if (!await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
