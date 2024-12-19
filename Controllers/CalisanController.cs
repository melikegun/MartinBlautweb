using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinBlautweb.Controllers
{
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
                .Include(c => c.Yetenekler) // Yetenekleri (Islem) dahil et
                .Include(c => c.UzmanlikAlan)
                .ToListAsync();
            return View(calisanlar);
        }

        // Çalışan ekleme sayfası (GET)
        public IActionResult CalisanEkle()
        {
            var islemler = _context.Islemler.ToList() ?? new List<Islem>();
            if (islemler == null || !islemler.Any())
            {
                TempData["hata"] = "IslemListesi boş! Islemler tablosunda veri yok.";
                return View("CalisanHata");
            }

            // IslemListesi'ni ViewData'ya gönder
            ViewData["IslemListesi"] = islemler;

            return View();
        }


        // Çalışan ekleme sayfası (POST)
        [HttpPost]
        public IActionResult CalisanEkle(Calisan calisan, int[] selectedIslemler)
        {
            if (selectedIslemler == null || !selectedIslemler.Any())
            {
                ModelState.AddModelError("", "Lütfen en az bir yetenek seçin.");
                ViewData["IslemListesi"] = _context.Islemler.ToList();
                return View(calisan);
            }

            calisan.SalonID = 1;
            var uzmanlik = _context.Islemler.FirstOrDefault(i => i.IslemID == calisan.UzmanlikAlanID);
            if (calisan != null)
            {
                // Yetenekleri ekliyoruz
                foreach (var islemId in selectedIslemler)
                {
                    var islem = _context.Islemler.Find(islemId);
                    if (islem != null)
                    {
                        calisan.Yetenekler.Add(islem);
                    }
                }
                calisan.UzmanlikAlan = uzmanlik;
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();
                TempData["msj"] = "Çalışan başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            ViewData["IslemListesi"] = _context.Islemler?.ToList() ?? new List<Islem>();
            return View(calisan);
        }

        // Çalışan düzenleme sayfası (GET)
        [HttpGet]
        public async Task<IActionResult> CalisanDuzenle(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme işlemi için işlem ID gerekli.";
                return View("CalisanHata");
            }

            // Çalışanı ID'ye göre ve ilişkili Islem verisiyle birlikte buluyoruz
            var calisan = await _context.Calisanlar
                            .Include(c => c.Yetenekler) // Yetenekleri (Islem) dahil et
                            .Include(c => c.UzmanlikAlan)
                            .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null)
            {
                TempData["hata"] = "Geçersiz işlem! Düzenlemek istediğiniz çalışan bulunamadı.";
                return View("CalisanHata");
            }

            var islemler = await _context.Islemler.ToListAsync() ?? new List<Islem>();
            if (islemler == null || !islemler.Any())
            {
                TempData["hata"] = "IslemListesi boş! Islemler tablosunda veri yok.";
                return View("CalisanHata");
            }

            // IslemListesi'ni ViewData'ya gönder
            ViewData["IslemListesi"] = islemler;

            return View(calisan);

        }

        // Çalışan düzenleme sayfası (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalisanDuzenle(int id, Calisan calisan, int[] selectedIslemler)
        {
            if (id != calisan.CalisanID)
            {
                TempData["hata"] = "Çalışan ID hatalı.";
                return View("CalisanHata");
            }

            // Çalışanı mevcut yetenekleriyle birlikte yükle
            var mevcutCalisan = await _context.Calisanlar
                .Include(c => c.Yetenekler) // Yetenekleri dahil et
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (mevcutCalisan == null)
            {
                TempData["hata"] = "Çalışan bulunamadı.";
                return View("CalisanHata");
            }

            // Mevcut yetenekleri temizleme (sadece yeni eklenenleri alacağız)
            var mevcutYetenekler = mevcutCalisan.Yetenekler.ToList();
            mevcutCalisan.Yetenekler.Clear();

            // Seçilen yetenekleri ekle
            foreach (var islemID in selectedIslemler)
            {
                // Yetenek zaten varsa ekleme
                if (!mevcutYetenekler.Any(y => y.IslemID == islemID))
                {
                    var islem = await _context.Islemler.FindAsync(islemID);
                    if (islem != null)
                    {
                        mevcutCalisan.Yetenekler.Add(islem);
                    }
                }
            }

            // Çalışanın diğer bilgilerini güncelle
            mevcutCalisan.CalisanAd = calisan.CalisanAd;
            mevcutCalisan.CalisanSoyad = calisan.CalisanSoyad;
            mevcutCalisan.CalisanTelefon = calisan.CalisanTelefon;
            mevcutCalisan.CalisanMesaiBaslangic = calisan.CalisanMesaiBaslangic;
            mevcutCalisan.CalisanMesaiBitis = calisan.CalisanMesaiBitis;
            mevcutCalisan.UzmanlikAlanID = calisan.UzmanlikAlanID;
            mevcutCalisan.SalonID = 1;

            // Değişiklikleri veritabanına kaydet
            await _context.SaveChangesAsync();

            TempData["msj"] = $"{mevcutCalisan.CalisanAd} adlı çalışan başarıyla güncellenmiştir.";
            return RedirectToAction("Index");
        }


        // Çalışan detay sayfası
        public async Task<IActionResult> CalisanDetay(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Geçerli bir çalışan ID giriniz.";
                return View("CalisanHata");
            }

            var calisan = await _context.Calisanlar
                                        .Include(c => c.Yetenekler)  // İlişkili Islem verisini dahil et
                                        .Include(c => c.UzmanlikAlan)
                                        .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null)
            {
                TempData["hata"] = "Geçerli bir çalışan bulunamadı.";
                return View("CalisanHata");
            }

            return View(calisan);
        }

        // Çalışan silme sayfası
        public async Task<IActionResult> CalisanSil(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Silme işlemi için çalışan ID gerekli.";
                return View("CalisanHata");
            }

            var calisan = await _context.Calisanlar.FindAsync(id);
            if (calisan == null)
            {
                TempData["hata"] = "Geçerli bir çalışan bulunamadı.";
                return View("CalisanHata");
            }

            // Çalışanı sil
            _context.Calisanlar.Remove(calisan);
            await _context.SaveChangesAsync();
            TempData["msj"] = $"{calisan.CalisanAd} {calisan.CalisanSoyad} adlı çalışan başarıyla silindi.";

            return RedirectToAction("Index");
        }

        // Yardımcı metod: Çalışan var mı?
        private bool CalisanExists(int id)
        {
            return _context.Calisanlar.Any(e => e.CalisanID == id);
        }
    }
}
