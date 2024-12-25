using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MartinBlautweb.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // Randevular Listeleme
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Include(r => r.Kullanici)
                .ToListAsync();

            return View(randevular);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RandevuEkle(int IslemID)
        {
            var islemler = _context.Islemler.ToList() ?? new List<Islem>();
            if (islemler == null || !islemler.Any())
            {
                TempData["hata"] = "IslemListesi boş! Islemler tablosunda veri yok.";
                return View("CalisanHata");
            }

            // İşlem Listesi'ni ViewData'ya gönder
            ViewData["IslemListesi"] = islemler;

            // Seçili işlem ID'sini ViewData'ya ekle
            ViewData["SelectedIslemID"] = IslemID;

            if (IslemID > 0)
            {
                // Eğer IslemID gönderildiyse, o işleme uygun çalışanları filtrele
                var calisanlar = _context.Calisanlar
                                         .Include(c => c.Yetenekler)
                                         .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                                         .ToList();

                if (calisanlar == null || !calisanlar.Any())
                {
                    // Çalışan yoksa ViewData'ya boş liste aktarılır
                    ViewData["CalisanListesi"] = new List<Calisan>();
                    TempData["hata"] = "Seçilen işlem için uygun çalışan bulunamadı.";
                }
                else
                {
                    ViewData["CalisanListesi"] = calisanlar;
                }
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RandevuEkle(Randevu randevu, int IslemID, int calisanID)
        {
            if (randevu == null)
            {
                TempData["hata"] = "Randevu bilgileri eksik ya da hatalı!";
                return View(randevu);
            }

            // Kullanıcı kimliğini al
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["hata"] = "Kullanıcı kimliği alınamadı. Lütfen giriş yapın.";
                return View(randevu);
            }

            randevu.SalonID = 1;
            randevu.KullaniciId = kullaniciId;  // Kullanıcı kimliğini atama

            randevu.IslemID = IslemID;
            randevu.CalisanID = calisanID;

            // Randevu ekle
            _context.Randevular.Add(randevu);
            _context.SaveChanges();

            TempData["msj"] = "Randevu başarıyla eklendi!";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult RandevuDetay(int? id)
        {
            var randevu = _context.Randevular
                                  .Include(r => r.Islem)
                                  .Include(r => r.Calisan)
                                  .Include(r => r.Kullanici)
                                  .FirstOrDefault(r => r.RandevuID == id);

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index");
            }

            return View(randevu);
        }

        [Authorize(Roles = "Admin")]
        // Randevu Düzenle
        public IActionResult RandevuDuzenle(int? id, int? IslemID)
        {
            if (id == null)
            {
                TempData["hata"] = "Düzenleme işlemi için işlem ID gerekli.";
                return View("CalisanHata");
            }

            var randevu = _context.Randevular
                                  .Include(r => r.Islem)
                                  .Include(r => r.Calisan)
                                  .Include(r => r.Kullanici)
                                  .FirstOrDefault(r => r.RandevuID == id);

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index");
            }

            IslemID ??= randevu.IslemID;

            ViewData["SelectedIslemID"] = IslemID;
            ViewData["IslemListesi"] = _context.Islemler.ToList();

            // Çalışanlar için, seçili işlem ile uyumlu çalışanları filtreleyelim
            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                .ToList();

            return View(randevu);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RandevuDuzenle(int id, Randevu randevu, int IslemID, int calisanID)
        {
            if (id != randevu.RandevuID)
            {
                TempData["hata"] = "Randevu ID hatalı.";
                return View("RandevuHata");
            }

            randevu.SalonID = 1;
            randevu.KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (randevu != null)
            {
                randevu.IslemID = IslemID;
                randevu.CalisanID = calisanID;
                _context.Randevular.Update(randevu);
                _context.SaveChanges();

                TempData["msj"] = "Randevu başarıyla güncellendi!";
                return RedirectToAction("Index");
            }

            // Hata durumunda tekrar formu göster
            ViewData["IslemListesi"] = _context.Islemler.ToList();
            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == randevu.IslemID))
                .ToList();
            return View(randevu);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RandevuSil(int? id)
        {
            var randevu = _context.Randevular
                                  .Include(r => r.Calisan)
                                  .Include(r => r.Islem)
                                  .Include(r => r.Kullanici)
                                  .FirstOrDefault(r => r.RandevuID == id);

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index");
            }

            _context.Randevular.Remove(randevu);
            _context.SaveChanges();

            TempData["msj"] = "Randevu başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}
