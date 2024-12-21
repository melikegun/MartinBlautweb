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
        public IActionResult RandevuEkle(int islemID = 0)
        {
            var islemler = _context.Islemler.ToList() ?? new List<Islem>();
            if (islemler == null || !islemler.Any())
            {
                TempData["hata"] = "IslemListesi boş! Islemler tablosunda veri yok.";
                return View("CalisanHata");
            }

            // İşlem Listesi'ni ViewData'ya gönder
            ViewData["IslemListesi"] = islemler;

            // Eğer islemID gönderildiyse, o işleme uygun çalışanları filtrele
            var calisanlar = _context.Calisanlar
                                     .Include(c => c.Yetenekler)
                                     .Where(c => c.Yetenekler.Any(y => y.IslemID == islemID))
                                     .ToList();


            // Çalışanları ViewData'ya gönder
            ViewData["CalisanListesi"] = calisanlar;

            return View();
        }



        [HttpPost]
        public IActionResult RandevuEkle(Randevu model, int islemID, int calisanID)
        {
            if (model != null)
            {
                // Randevu oluştur ve kaydet
                model.IslemID = islemID;
                model.CalisanID = calisanID;
                _context.Randevular.Add(model);
                _context.SaveChanges();

                TempData["msj"] = "Randevu başarıyla eklendi!";
                return RedirectToAction("Index");
            }

            TempData["hata"] = "Randevu bilgileri eksik ya da hatalı!";
            return View(model);
        }



        public async Task<IActionResult> RandevuDetay(int? id)
        {
            if (id == null)
            {
                TempData["hata"] = "Geçerli bir randevu ID giriniz.";
                return View("RandevuHata");
            }

            var randevu = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.RandevuID == id);

            if (randevu == null)
            {
                TempData["hata"] = "Geçerli bir randevu bulunamadı.";
                return View("RandevuHata");
            }

            return View(randevu);
        }



        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı.";
                return RedirectToAction("Index");
            }

            randevu.OnayDurumu = true; // Randevu onaylanır
            _context.Randevular.Update(randevu);
            await _context.SaveChangesAsync();

            TempData["msj"] = "Randevunuz başarıyla onaylandı.";
            return RedirectToAction("Index");
        }



    }
}
