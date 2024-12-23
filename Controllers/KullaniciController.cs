using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Models;
using System.Threading.Tasks;
using MartinBlautweb.Data;

namespace MartinBlautweb.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KullaniciController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanıcı Giriş Sayfası (GET)
        public IActionResult Giris()
        {
            return View();
        }

        // Kullanıcı Giriş İşlemi (POST)
        [HttpPost]
        public async Task<IActionResult> Giris(string email, string password)
        {
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciMail == email && k.KullaniciSifre == password);

            if (kullanici != null)
            {
                TempData["msj"] = $"Hoş geldiniz, {kullanici.KullaniciAd}!";
                return RedirectToAction("Index", "Kullanici");
            }
            // Giriş başarısız
            ViewBag.ErrorMessage = "E-posta veya şifre hatalı!";
            return View();
        }

        // Kullanıcı Üyelik Sayfası (GET)
        public IActionResult UyeOl()
        {
            return View();
        }

        // Kullanıcı Üyelik İşlemi (POST)
        [HttpPost]
        public async Task<IActionResult> UyeOl(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();
                TempData["msj"] = "Kayıt başarılı, giriş yapabilirsiniz!";
                return RedirectToAction("Giris");
            }
            ViewBag.ErrorMessage = "Bilgiler istenlen şekilde değil, hatalı!";
            return View(kullanici);
        }

        // Kullanıcı Paneli (Giriş yaptıktan sonra ulaşacağı sayfa)
        public IActionResult Index()
        {
            return View();
        }
    }
}
