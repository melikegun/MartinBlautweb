using Microsoft.AspNetCore.Mvc;
using MartinBlautweb.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;

namespace MartinBlautweb.Controllers
{
    public class IslemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IslemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public IActionResult Index()
        {
            var islemler = _context.Islemler.ToList();
            return View(islemler);
        }

        // İşlem Detayı
        public IActionResult IslemDetay(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Lütfen geçerli bir işlem ID giriniz.";
                return View("IslemHata");
            }

            var islem = _context.Islemler.FirstOrDefault(m => m.IslemID == id);
            if (islem == null)
            {
                TempData["hata"] = "Geçerli bir işlem ID giriniz.";
                return View("IslemHata");
            }

            return View(islem);
        }

        // Yeni İşlem Ekleme
        public IActionResult IslemEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IslemEkle([Bind("IslemID,IslemAdi,Ucret,Aciklama,Sure")] Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(islem);
                _context.SaveChanges();

                TempData["msj"] = islem.IslemAdi + " adlı işlem başarıyla eklenmiştir.";
                return RedirectToAction("Index");
            }

            TempData["hata"] = "İşlem eklenemedi. Lütfen tüm alanları doldurduğunuzdan emin olun.";
            return View(islem);
        }

        // İşlem Düzenleme (GET)
        public IActionResult IslemDuzenle(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme işlemi için işlem ID gerekli.";
                return View("IslemHata");
            }

            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                TempData["hata"] = "Geçerli bir işlem ID gerekli.";
                return View("IslemHata");
            }

            return View(islem);
        }

        // İşlem Düzenleme (POST)
        [HttpPost]
        public IActionResult IslemDuzenle(int id, [Bind("IslemID,IslemAdi,Ucret,Aciklama,Sure")] Islem islem)
        {
            if (id != islem.IslemID)
            {
                TempData["hata"] = "İşlem ID hatalı.";
                return View("IslemHata");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Islemler.Update(islem);
                    _context.SaveChanges();

                    TempData["msj"] = islem.IslemAdi + " adlı işlem başarıyla güncellenmiştir.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IslemExists(islem.IslemID))
                    {
                        TempData["hata"] = "İşlem güncellenirken bir hata oluştu.";
                        return View("IslemHata");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            TempData["hata"] = "Lütfen tüm alanları eksiksiz doldurun.";
            return View(islem);
        }

        // İşlem Silme
        public IActionResult IslemSil(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Silme işlemi için işlem ID gerekli.";
                return View("IslemHata");
            }

            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                TempData["hata"] = "Geçerli bir işlem bulunamadı.";
                return View("IslemHata");
            }

            if (islem.Randevular.Count > 0)
            {
                TempData["hata"] = "Bu işlemle ilişkilendirilmiş randevular bulunmaktadır. Silme işlemi gerçekleştirilemez.";
                return View("IslemHata");
            }

            _context.Islemler.Remove(islem);
            _context.SaveChanges();

            TempData["msj"] = islem.IslemAdi + " adlı işlem başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // Yardımcı Fonksiyon: İşlem Mevcut mu
        private bool IslemExists(int id)
        {
            return _context.Islemler.Any(e => e.IslemID == id);
        }
    }
}
