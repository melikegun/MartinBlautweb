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
            const string AdminPassword = "sau";

            // E-posta ve şifreyi kontrol ediyoruz
            if (email == AdminEmail)
            {
                // Eğer giriş bilgileri doğruysa, sistemdeki kullanıcıyı buluyoruz
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Şifreyi doğru şekilde kontrol etmek için CheckPasswordAsync kullanıyoruz
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
                    if (!passwordCheck)
                    {
                        ViewBag.ErrorMessage = "Geçersiz şifre!";
                        return View();
                    }

                    // Şifre doğruysa, giriş yapmayı deniyoruz
                    var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("AdminDashboard");  // Başarılı girişten sonra Admin Dashboard'a yönlendirme
                    }
                    else if (result.IsLockedOut)
                    {
                        ViewBag.ErrorMessage = "Hesabınız kilitlenmiş. Lütfen daha sonra tekrar deneyin.";
                    }
                    else if (result.IsNotAllowed)
                    {
                        ViewBag.ErrorMessage = "Hesabınız girişe kapalı.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Bilinmeyen bir hata oluştu.";
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre!";
            }

            return View();  // Hatalı girişte tekrar login sayfasına dön
        }

        // Admin Paneli
        public IActionResult AdminDashboard()
        {
            return View();  // Admin paneline yönlendirme
        }
    }
}
