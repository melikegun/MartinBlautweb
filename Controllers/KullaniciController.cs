using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MartinBlautweb.Models;
using System.Threading.Tasks;

namespace MartinBlautweb.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;

        public KullaniciController(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Kullanıcı Giriş
        [HttpGet]
        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Giris(string email, string sifre)
        {
            var kullanici = await _userManager.FindByEmailAsync(email);
            if (kullanici != null)
            {
                var result = await _signInManager.PasswordSignInAsync(kullanici, sifre, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Hata = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        // Kullanıcı Kayıt
        [HttpGet]
        public IActionResult Kayit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Kayit(Kullanici model, string sifre)
        {
            if (ModelState.IsValid)
            {
                var kullanici = new Kullanici
                {
                    UserName = model.Email,
                    Email = model.Email,
                    KullaniciAd = model.KullaniciAd,
                    KullaniciSoyad = model.KullaniciSoyad,
                    KullaniciTelefon = model.KullaniciTelefon
                };

                var result = await _userManager.CreateAsync(kullanici, sifre);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(kullanici, isPersistent: false);
                    await _userManager.AddToRoleAsync(kullanici, "User");  // Kullanıcıyı "User" rolüne ata
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
