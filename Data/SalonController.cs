using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MartinBlautweb.Data;
using MartinBlautweb.Models;  // Salon modelinin bulunduğu namespace

namespace MartinBlautweb.Controllers
{
    public class SalonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Salon listeleme
        public IActionResult Index()
        {
            var salon = _context.Salonlar.FirstOrDefault();  // Sadece bir salon var
            return View(salon);
        }

        // Salon detayını görüntüleme
        public IActionResult SalonDetay(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Lütfen salon ID bilgisini giriniz.";
                return View("SalonHata");
            }

            var salon = _context.Salonlar.FirstOrDefault(x => x.SalonID == id);

            if (salon == null)
            {
                TempData["hata"] = "Geçerli bir salon ID giriniz.";
                return View("SalonHata");
            }

            return View(salon);
        }

        // Salon düzenleme
        public IActionResult SalonDuzenle(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme için salon ID gerekli.";
                return View("SalonHata");
            }

            var salon = _context.Salonlar.FirstOrDefault(x => x.SalonID == id);

            if (salon == null)
            {
                TempData["hata"] = "Geçerli bir salon ID gerekli. Lütfen kontrol ediniz.";
                return View("SalonHata");
            }

            return View(salon);
        }

        // Salon düzenleme işlemi
        [HttpPost]
        public IActionResult SalonDuzenle(int id, Salon salon)
        {
            if (id != salon.SalonID)
            {
                TempData["hata"] = "Salon ID hatalı.";
                return View("SalonHata");
            }

            if (ModelState.IsValid)
            {
                _context.Salonlar.Update(salon);
                _context.SaveChanges();

                TempData["msj"] = "Salon bilgileri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            TempData["hata"] = "Lütfen tüm alanları doldurun.";
            return View("SalonHata");
        }
    }
}
