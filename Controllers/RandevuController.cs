using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MartinBlautweb.Controllers
{
    public class RandevuController : Controller
    {
        /*
         * Randevu Sistemi
            o Kullanıcılar, uygun çalışanlara ve işlemlere göre sistem üzerinden randevu alabilecek.
            o Randevu saati seçiminde önceki randevuların saati dikkate alınmalı. Eğer seçilen saat doluysa randevu vermemeli
            o Randevu detayları (işlem, süre, ücret, randevunun sahibinin bilgileri) sistemde saklanacak.
            o Randevu onay mekanizması olacak.
        */
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        //randevular listeleme
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Include(r => r.Kullanici)
                .ToListAsync();

            return View(randevular);
        }
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

                // Eğer islemID gönderildiyse, o işleme uygun çalışanları filtrele
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



        [HttpPost]
        public IActionResult RandevuEkle(Randevu randevu, int IslemID, int calisanID)
        {
            randevu.SalonID = 1;

            if (randevu != null)
            {
                // Randevu oluştur ve kaydet
                randevu.IslemID = IslemID;
                randevu.CalisanID = calisanID;
                _context.Randevular.Add(randevu);
                _context.SaveChanges();

                TempData["msj"] = "Randevu başarıyla eklendi!";
                return RedirectToAction("Index");
            }

            TempData["hata"] = "Randevu bilgileri eksik ya da hatalı!";

            // İşlem ve çalışan listelerini doldur tekrar göster
            ViewData["IslemListesi"] = _context.Islemler.ToList();

            ViewData["CalisanListesi"] = _context.Calisanlar
                                                .Include(c => c.Yetenekler)
                                                .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                                                .ToList();
            return View(randevu);
        }

        public IActionResult RandevuDetay(int? id)
        {
            // Randevuyu, ilişkili olduğu Islem, Calisan ve Kullanici bilgileriyle birlikte getir
            var randevu = _context.Randevular
                                  .Include(r => r.Islem)       // İşlem bilgisi
                                  .Include(r => r.Calisan)     // Çalışan bilgisi
                                  .Include(r => r.Kullanici)   // Kullanıcı bilgisi
                                  .FirstOrDefault(r => r.RandevuID == id);  // ID'ye göre arama

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index");
            }

            return View(randevu);  // Bulunan randevuyu RandevuDetay view'ına gönder
        }


        // GET: RandevuDuzenle
        public IActionResult RandevuDuzenle(int? id, int? IslemID)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme işlemi için işlem ID gerekli.";
                return View("CalisanHata");
            }
           
            // Randevuyu ID'ye göre bul
            var randevu = _context.Randevular
                                  .Include(r => r.Islem)
                                  .Include(r => r.Calisan)
                                  .Include(r => r.Kullanici)
                                  .FirstOrDefault(r => r.RandevuID == id);

            // Eğer randevu bulunamazsa hata mesajı döndür
            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index");
            }

            IslemID ??= randevu.IslemID;

            // Seçili işlem ID'sini ViewData'ya ekle
            ViewData["SelectedIslemID"] = IslemID;

            // İşlem Listesi'ni ViewData'ya ekle
            ViewData["IslemListesi"] = _context.Islemler.ToList();


            // Çalışanlar için, seçili işlem ile uyumlu çalışanları filtreleyelim
            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                .ToList();

            // Randevu modelini View'a gönder
            return View(randevu);
        }

        // POST: RandevuDuzenle
        [HttpPost]
        public IActionResult RandevuDuzenle(int id, Randevu randevu, int IslemID, int calisanID)
        {
            if (id != randevu.RandevuID)
            {
                TempData["hata"] = "Randevu ID hatalı.";
                return View("RandevuHata");
            }

            randevu.SalonID = 1;

            // Eğer randevu geçerliyse düzenleme işlemi yapalım
            if (randevu != null)
            {
                randevu.IslemID = IslemID;
                randevu.CalisanID = calisanID;
                // Randevuyu güncelle
                _context.Randevular.Update(randevu);
                _context.SaveChanges();

                TempData["msj"] = "Randevu başarıyla güncellendi!";
                return RedirectToAction("Index");
            }

            // Model geçerli değilse tekrar sayfayı göster
            ViewData["IslemListesi"] = _context.Islemler.ToList();
            ViewData["SelectedIslemID"] = randevu.IslemID;  // Seçili işlem ID'sini ekle
            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == randevu.IslemID))
                .ToList();
            return View(randevu);
        }





        public IActionResult RandevuSil(int? id)
        {
            // Verilen ID'ye göre randevuyu bul
            var randevu = _context.Randevular
                                .Include(r => r.Calisan)
                                .Include(r => r.Islem)
                                .Include(r => r.Kullanici)
                                .FirstOrDefault(r => r.RandevuID == id);

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectToAction("Index"); // Randevu bulunamazsa liste sayfasına dön
            }

            // Randevuyu veritabanından sil
            _context.Randevular.Remove(randevu);
            _context.SaveChanges();

            TempData["msj"] = "Randevu başarıyla silindi!";
            return RedirectToAction("Index"); // Silme işlemi başarılıysa liste sayfasına dön
        }


    }
}
