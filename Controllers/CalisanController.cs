using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MartinBlautweb.Controllers
{
    //CALISAN EKLEYEMİYORUUUMM
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalisanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Çalışanlar sayfası (Listeleme)
        public async Task<IActionResult> Index()
        {
            var calisanlar = await _context.Calisanlar
                .Include(c => c.Islem) // İlişkili Islem (Uzmanlık Alanı) bilgisini yükle
                .ToListAsync();
            return View(calisanlar);
        }

        // Çalışan ekleme sayfası
        public IActionResult CalisanEkle()
        {
            // İşlemler listesini veritabanından al
            var islemler = _context.Islemler?.ToList() ?? new List<Islem>();
            ViewData["IslemListesi"] = islemler; // ViewData'ya aktar
            return View();
        }

        [HttpPost]
        public IActionResult CalisanEkle(Calisan model)
        {
            if (ModelState.IsValid)
            {
                // Model geçerliyse kaydedin
                _context.Calisanlar.Add(model);
                _context.SaveChanges();

                TempData["msj"] = "Çalışan başarıyla eklendi.";
                return RedirectToAction("Calisanlar");
            }

            // Eğer hata varsa tekrar ViewData'yı doldurun ve formu geri döndürün
            ViewData["IslemListesi"] = _context.Islemler?.ToList() ?? new List<Islem>();
            return View(model);
        }


        // Çalışan düzenleme sayfası
        public async Task<IActionResult> CalisanDuzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calisan = await _context.Calisanlar.FindAsync(id);
            if (calisan == null)
            {
                return NotFound();
            }

            // Uzmanlık alanlarını veritabanından al ve dropdown list olarak View'a gönder
            ViewBag.IslemListesi = new SelectList(_context.Islemler, "IslemID", "IslemAdi", calisan.IslemID);
            return View(calisan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalisanDuzenle(int id, [Bind("CalisanID,CalisanAd,CalisanSoyad,IslemID,CalisanTelefon,CalisanMesaiBaslangic,CalisanMesaiBitis")] Calisan calisan)
        {
            if (id != calisan.CalisanID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calisan);
                    await _context.SaveChangesAsync();
                    TempData["msj"] = "Çalışan başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalisanExists(calisan.CalisanID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IslemListesi = new SelectList(_context.Islemler, "IslemID", "IslemAdi", calisan.IslemID);
            return View(calisan);
        }

        // Yardımcı metod: Çalışan var mı?
        private bool CalisanExists(int id)
        {
            return _context.Calisanlar.Any(e => e.CalisanID == id);
        }
    }
}
