using MartinBlautweb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MartinBlautweb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Admin Giriş Sayfası
        [HttpGet]
        public IActionResult Giris()
        {
            return View();
        }

        // Admin Giriş İşlemi
        [HttpPost]
        public async Task<IActionResult> Giris(string email, string password)
        {
            var admin = await _context.Adminler
                .FirstOrDefaultAsync(a => a.AdminMail == email && a.AdminSifre == password);

            if (admin != null)
            {
                // Burada adminin rolünü kontrol ediyoruz
                // Eğer admin rolüne sahipse, giriş yapılabilir
                // (Gelişmiş bir doğrulama için roller ve yetkiler kullanılabilir)
                if (admin.AdminMail == email && admin.AdminSifre == password)
                {
                    TempData["msj"] = $"Hoş geldiniz, {admin.AdminAd}!";
                    // Admin sayfasına yönlendir
                    return RedirectToAction("Index", "Admin");
                }
            }

            // Giriş başarısız
            ViewBag.ErrorMessage = "E-posta veya şifre hatalı!";
            return View();
        }


        // Admin Index Sayfası
        public IActionResult Index()
        {
            // Admin giriş yaptıktan sonra buraya yönlendirilir
            return View();
        }

        // Salon Yönetimi (SalonController yönlendirmesi)
        public IActionResult SalonYonetimi()
        {
            return RedirectToAction("Index", "Salon");
        }

        // İşlem Yönetimi (IslemController yönlendirmesi)
        public IActionResult IslemYonetimi()
        {
            return RedirectToAction("Index", "Islem");
        }

        // Çalışan Yönetimi (CalisanController yönlendirmesi)
        public IActionResult CalisanYonetimi()
        {
            return RedirectToAction("Index", "Calisan");
        }      

        // Randevu Yönetimi (RandevuController yönlendirmesi)
        public IActionResult RandevuYonetimi()
        {
            return RedirectToAction("Index", "Randevu");
        }

        // Kullanıcı Yönetimi (KullaniciController yönlendirmesi)
        public IActionResult KullaniciYonetimi()
        {
            return RedirectToAction("Index", "Kullanici");
        }

    }
}
