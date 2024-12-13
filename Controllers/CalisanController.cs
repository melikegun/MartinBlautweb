using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        // Çalışan ekleme sayfası (GET)
        public IActionResult CalisanEkle()
        {
            // Islemler tablosundan tüm verileri al
            var islemler = _context.Islemler.ToList();            // _context: DbContext nesnesi
            if (islemler == null)
            {
                throw new Exception("IslemListesi boş! Islemler tablosunda veri yok.");
            }

            // IslemListesi'ni ViewData'ya gönder
            ViewData["IslemListesi"] = islemler;

            return View();
        }

        // Çalışan ekleme sayfası (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CalisanEkle(Calisan calisan)
        {
            Console.WriteLine($"CalisanID: {calisan.CalisanID}");
            Console.WriteLine($"IslemID: {calisan.IslemID}");

            var islem = _context.Islemler.FirstOrDefault(i => i.IslemID == calisan.IslemID);
            // Model doğrulama
            if (calisan != null)
            {
                // IslemID'yi kullanarak doğru işlemi bul


                // Eğer geçerli bir IslemID yoksa hata mesajı ekle
                if (islem == null)
                {
                    ModelState.AddModelError("IslemID", "Seçilen Uzmanlık Alanı bulunamadı!");
                    // Islem listesi tekrar yüklenip ViewData'ya atanacak
                    ViewData["IslemListesi"] = _context.Islemler.ToList();
                    return View(calisan);
                }

                // Çalışanı ekle
                calisan.Islem = islem; // İlgili uzmanlık alanını ilişkilendir
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();

                // Başarılı işlem sonrası yönlendirme
                return RedirectToAction("Index");
            }


            // Hata durumunda IslemListesi'ni yeniden gönder
            ViewData["IslemListesi"] = _context.Islemler.ToList();
            return View(calisan);
        }



        // Çalışan düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> CalisanDuzenle(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme için salon ID gerekli.";
                return View("CalisanHata");
            }

            // Çalışanı ID'ye göre ve ilişkili Islem verisiyle birlikte asenkron olarak buluyoruz
            var calisan = await _context.Calisanlar
                                        .Include(c => c.Islem)  // İlişkili Islem verisini dahil et
                                        .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null)
            {
                TempData["hata"] = "Geçerli bir calisan ID gerekli. Lütfen kontrol ediniz.";
                return View("CalisanHata");
            }

            // Islemler listesini al
            var islemler = await _context.Islemler.ToListAsync();

            // IslemListesi'ni ViewData'ya gönderiyoruz
            ViewData["IslemListesi"] = islemler;

            // SelectList'i ViewBag ile gönderiyoruz, seçili olan IslemID ile ilişkilendiriyoruz
            ViewBag.IslemListesi = new SelectList(islemler, "IslemID", "IslemAdi", calisan.IslemID);

            // Çalışan verisi ve Islem listesi ile View'ı döndürüyoruz
            return View(calisan);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CalisanDuzenle(int id, Calisan calisan)
        {
            if (id != calisan.CalisanID)
            {
                TempData["hata"] = "Calışan ID hatalı.";
                return View("CalisanHata");
            }

            if (calisan != null)
            {
                _context.Update(calisan);
                _context.SaveChangesAsync();
                TempData["msj"] = "Çalışan başarıyla güncellendi.";


                return RedirectToAction("Index");
            }

            ViewBag.IslemListesi = new SelectList(_context.Islemler, "IslemID", "IslemAdi", calisan.IslemID);
            return View("CalisanHata");
        }

        public IActionResult CalisanDetay(int? id)
        {
            if(id is null)
            {
                ViewBag["hata"] = "Lütfen geçerli bir çalışan ID giriniz.";
                    return View("CalisanHata");
            }

            var calisan = _context.Calisanlar
                                        .Include(c => c.Islem)  // İlişkili Islem verisini dahil et
                                        .FirstOrDefault(c => c.CalisanID == id);
            if(calisan == null)
            {
                TempData["hata"] = "Geçerli bir çalışan ID giriniz.";
                return View("CalisanHata");
            }
            return View(calisan);
        }

        public IActionResult CalisanSil(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Silme işlemi için çalışan ID gerekli.";
                return View("CalisanHata");
            }

            var calisan = _context.Calisanlar.Find(id);
            if (calisan == null)
            {
                TempData["hata"] = "Geçerli bir çalışan bulunamadı.";
                return View("CalisanHata");
            }

            if (calisan.Randevular.Count > 0)
            {
                TempData["hata"] = "Bu işlemle ilişkilendirilmiş randevular bulunmaktadır. Silme işlemi gerçekleştirilemez.";
                return View("CalisanHata");
            }

            _context.Calisanlar.Remove(calisan);
            _context.SaveChanges();

            TempData["msj"] = calisan.CalisanAd + " adlı çalışan başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // Yardımcı metod: Çalışan var mı?
        private bool CalisanExists(int id)
        {
            return _context.Calisanlar.Any(e => e.CalisanID == id);
        }
    }
}
