using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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
            if(calisan != null)
            {
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
                TempData["hata"] = "Düzenleme için çalışan ID gerekli.";
                return View("CalisanHata");
            }

            // Çalışanı ID'ye göre ve ilişkili Islem verisiyle birlikte buluyoruz
            var calisan = await _context.Calisanlar
                                        .Include(c => c.Yetenekler) // Yetenekleri (Islem) dahil et
                                        .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null)
            {
                TempData["hata"] = "Geçerli bir çalışan bulunamadı.";
                return View("CalisanHata");
            }

            // Islemler listesini al
            var islemler = await _context.Islemler.ToListAsync();

            // IslemListesi'ni ViewData'ya gönderiyoruz
            ViewData["IslemListesi"] = islemler;

            // Seçilen Uzmanlık Alanı (IslemID) ile SelectList'i ViewBag ile gönderiyoruz
            ViewBag.IslemListesi = new SelectList(islemler, "IslemID", "IslemAdi", calisan.UzmanlikAlanID);

            // Çalışan verisi ve Islem listesi ile View'ı döndürüyoruz
            return View(calisan);
        }

        // Çalışan düzenleme sayfası (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalisanDuzenle(int id, Calisan calisan)
        {
            if (id != calisan.CalisanID)
            {
                TempData["hata"] = "Çalışan ID hatalı.";
                return View("CalisanHata");
            }

            if (calisan != null)
            {
                try
                {
                    // Seçilen Uzmanlık Alanını (IslemID) tekrar kontrol et
                    var islem = _context.Islemler.FirstOrDefault(i => i.IslemID == calisan.UzmanlikAlanID);
                    if (islem == null)
                    {
                        TempData["hata"] = "Geçerli bir Uzmanlık Alanı bulunamadı.";
                        ViewData["IslemListesi"] = _context.Islemler.ToList();
                        return View(calisan);
                    }

                    calisan.Yetenekler.Clear();  // Eski yetenekleri temizle
                    calisan.Yetenekler.Add(islem); // Yeni yetenekleri ekle

                    // Çalışanı güncelle
                    _context.Update(calisan);
                    await _context.SaveChangesAsync();
                    TempData["msj"] = "Çalışan başarıyla güncellendi.";
                }
                catch (Exception ex)
                {
                    TempData["hata"] = $"Hata oluştu: {ex.Message}";
                }

                return RedirectToAction("Index");
            }

            // Hata durumunda IslemListesi'ni yeniden gönder
            ViewData["IslemListesi"] = _context.Islemler.ToList();
            return View(calisan);
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
