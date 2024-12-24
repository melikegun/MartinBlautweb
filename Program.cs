using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using MartinBlautweb.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�rma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yap�land�rmas�: Kullan�c� ve rol yap�land�rmas�
builder.Services.AddIdentity<Kullanici, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// �ifre politikalar�n� �zelle�tirme
builder.Services.Configure<IdentityOptions>(options =>
{
    // �ifre politikas�
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;  // �ifrenin minimum uzunlu�unu belirle
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
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullan�c�s� var m� kontrol�
        var adminUser = await userManager.FindByEmailAsync("b221210089@sakarya.edu.tr");
        if (adminUser == null)
        {
            // Admin kullan�c� olu�turuluyor
            adminUser = new Kullanici
            {
                UserName = "b221210089@sakarya.edu.tr",
                Email = "b221210089@sakarya.edu.tr"
            };

            // Admin kullan�c�s� olu�turuluyor ve parola set ediliyor
            var result = await userManager.CreateAsync(adminUser, "sau");
            if (result.Succeeded)
            {
                // Admin rol�n� ekliyoruz
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
