using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using MartinBlautweb.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýlandýrma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý: Kullanýcý ve rol yapýlandýrmasý
builder.Services.AddIdentity<Kullanici, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Þifre politikalarýný özelleþtirme
builder.Services.Configure<IdentityOptions>(options =>
{
    // Þifre politikasý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;  // Þifrenin minimum uzunluðunu belirle
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
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullanýcýsý var mý kontrolü
        var adminUser = await userManager.FindByEmailAsync("b221210089@sakarya.edu.tr");
        if (adminUser == null)
        {
            // Admin kullanýcý oluþturuluyor
            adminUser = new Kullanici
            {
                UserName = "b221210089@sakarya.edu.tr",
                Email = "b221210089@sakarya.edu.tr"
            };

            // Admin kullanýcýsý oluþturuluyor ve parola set ediliyor
            var result = await userManager.CreateAsync(adminUser, "sau");
            if (result.Succeeded)
            {
                // Admin rolünü ekliyoruz
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
