using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MartinBlautweb.Models;
using System.Threading.Tasks;

namespace MartinBlautweb.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<Kullanici> _signInManager;
        private readonly UserManager<Kullanici> _userManager;

        // AdminController'ın constructor'ı
        public AdminController(SignInManager<Kullanici> signInManager, UserManager<Kullanici> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult AccessDenied()
        {
            return View(); // AccessDenied.cshtml adlı bir sayfa oluştur
        }

        // Admin Login - GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Admin Login - POST
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Boşlukları temizleme (Trim)
            email = email?.Trim();
            password = password?.Trim();

            // Admin bilgileriniz
            const string AdminEmail = "b221210089@sakarya.edu.tr";

            // E-posta ve şifreyi kontrol ediyoruz
            if (email == AdminEmail)
            {
                // Eğer giriş bilgileri doğruysa, sistemdeki kullanıcıyı buluyoruz
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Şifreyi manuel olarak kontrol etmek için PasswordHasher kullanımı
                    var passwordHasher = new PasswordHasher<Kullanici>();
                    var hashVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                    if (hashVerification == PasswordVerificationResult.Failed)
                    {
                        ModelState.AddModelError("", "Şifre doğrulama başarısız.");
                        return View();
                    }

                    // Şifre doğruysa, giriş yapmayı deniyoruz
                    var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");  // Başarılı girişten sonra Admin Dashboard'a yönlendirme
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Hesabınız kilitlenmiş. Lütfen daha sonra tekrar deneyin.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError("", "Hesabınız girişe kapalı.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bilinmeyen bir hata oluştu.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre!");
            }

            return View();  // Hatalı girişte tekrar login sayfasına dön
        }

        // Admin Paneli
        public IActionResult Index()
        {
            return View();  // Admin paneline yönlendirme
        }
    }
}
