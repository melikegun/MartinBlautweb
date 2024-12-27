using MartinBlautweb.Data;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MartinBlautweb.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IActionResult RedirectBasedOnRole()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("KullaniciRandevulari");
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> KullaniciRandevulari()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Where(r => r.KullaniciId == kullaniciId)
                .ToListAsync();
            return View(randevular);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Include(r => r.Kullanici)
                .ToListAsync();

            return View(randevular);
        }

        [Authorize(Roles = "User")]
        public IActionResult RandevuEkle(int IslemID)
        {
            var islemler = _context.Islemler.ToList() ?? new List<Islem>();
            if (islemler == null || !islemler.Any())
            {
                TempData["hata"] = "IslemListesi boş! Islemler tablosunda veri yok.";
                return View("CalisanHata");
            }

            ViewData["IslemListesi"] = islemler;
            ViewData["SelectedIslemID"] = IslemID;

            if (IslemID > 0)
            {
                var calisanlar = _context.Calisanlar
                                         .Include(c => c.Yetenekler)
                                         .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                                         .ToList();

                if (calisanlar == null || !calisanlar.Any())
                {
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

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> RandevuEkle(Randevu randevu, int IslemID, int calisanID, DateTime randevuTarihi, TimeSpan randevuSaati)
        {
            if (randevu == null)
            {
                TempData["hata"] = "Randevu bilgileri eksik ya da hatalı!";
                return View(randevu);
            }

            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["hata"] = "Kullanıcı kimliği alınamadı. Lütfen giriş yapın.";
                return View(randevu);
            }

            // Çalışan bilgisini al
            var calisan = await _context.Calisanlar
                                         .Include(c => c.Randevular)
                                         .FirstOrDefaultAsync(c => c.CalisanID == calisanID);

            if (calisan == null)
            {
                TempData["hata"] = "Seçilen çalışan bulunamadı.";
                return View(randevu);
            }

            // Çalışanın mesai saatlerini kontrol et
            if (randevuSaati < calisan.CalisanMesaiBaslangic || randevuSaati > calisan.CalisanMesaiBitis)
            {
                TempData["hata"] = "Çalışan mesai saatleri dışında çalışmıyor.";
                return View(randevu);
            }

            // Çalışanın mevcut randevularını kontrol et
            var mevcutRandevular = calisan.Randevular
                .Where(r => r.RandevuTarihi.Date == randevuTarihi.Date &&
                            r.RandevuSaati == randevuSaati)
                .ToList();

            // Çalışanın belirli bir saatte başka bir randevusu olup olmadığını kontrol et
            if (mevcutRandevular.Any())
            {
                TempData["hata"] = "Seçilen saat için çalışan zaten bir randevuya sahip.";
                return View(randevu);
            }

            // Randevu ekleme işlemi
            randevu.SalonID = 1;  // Varsayılan salon
            randevu.KullaniciId = kullaniciId;
            randevu.IslemID = IslemID;
            randevu.CalisanID = calisanID;
            randevu.RandevuTarihi = randevuTarihi;

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["msj"] = "Randevu başarıyla eklendi!";
            return RedirectBasedOnRole(); // Kullanıcıya göre yönlendirme
        }



        [Authorize(Roles = "Admin , User")]
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
                return RedirectBasedOnRole();
            }

            return View(randevu);
        }

        [Authorize(Roles = "Admin, User")]
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
                return RedirectBasedOnRole();
            }

            IslemID ??= randevu.IslemID;

            ViewData["SelectedIslemID"] = IslemID;
            ViewData["IslemListesi"] = _context.Islemler.ToList();

            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == IslemID))
                .ToList();

            return View(randevu);
        }

        [Authorize(Roles = "Admin, User")]
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
                return RedirectBasedOnRole();
            }

            ViewData["IslemListesi"] = _context.Islemler.ToList();
            ViewData["CalisanListesi"] = _context.Calisanlar
                .Where(c => c.Yetenekler.Any(y => y.IslemID == randevu.IslemID))
                .ToList();
            return View(randevu);
        }

        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> RandevuSil(int? id)
        {
            var randevu = await _context.Randevular
                                        .Include(r => r.Calisan)
                                        .Include(r => r.Islem)
                                        .Include(r => r.Kullanici)
                                        .FirstOrDefaultAsync(r => r.RandevuID == id); // Asenkron hale getirildi

            if (randevu == null)
            {
                TempData["hata"] = "Randevu bulunamadı!";
                return RedirectBasedOnRole(); // Asenkron hale getirildi
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync(); // Asenkron hale getirildi



            //// Silme işleminden sonra ID sıralamasını sıfırlama
            var maxId = await _context.Randevular.MaxAsync(x => x.RandevuID); // Asenkron hale getirildi
            await _context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('Randevular', RESEED, {maxId});"); // Asenkron hale getirildi


            TempData["msj"] = "Randevu başarıyla silindi!";
            return RedirectBasedOnRole(); // Asenkron hale getirildi
        }

    }
}
