﻿using Microsoft.AspNetCore.Mvc;
using MartinBlautweb.Models;
using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Data;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var islemler = await _context.Islemler.ToListAsync();
            return View(islemler);
        }

        // İşlem Detayı
        public async Task<IActionResult> IslemDetay(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Lütfen geçerli bir işlem ID giriniz.";
                return View("IslemHata");
            }

            var islem = await _context.Islemler
                .FirstOrDefaultAsync(m => m.IslemID == id);

            if (islem == null)
            {
                TempData["hata"] = "Geçerli bir işlem ID giriniz.";
                return View("IslemHata");
            }

            return View(islem);
        }

        // Yeni İşlem Ekleme (GET)
        public IActionResult IslemEkle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IslemEkle([Bind("IslemID,IslemAdi,Ucret,Aciklama,Sure")] Islem islem)
        {
            islem.SalonID = 1;

            if (islem != null)
            {          
                _context.Islemler.Add(islem);
                await _context.SaveChangesAsync();

                TempData["msj"] = islem.IslemAdi + " adlı işlem başarıyla eklenmiştir.";
                return RedirectToAction("Index");
            }

            TempData["hata"] = "İşlem eklenemedi. Lütfen tüm alanları doldurduğunuzdan emin olun.";
            return View(islem);
        }

        // İşlem Düzenleme (GET)
        public async Task<IActionResult> IslemDuzenle(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Düzenleme işlemi için işlem ID gerekli.";
                return View("IslemHata");
            }

            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null)
            {
                TempData["hata"] = "Geçerli bir işlem ID gerekli.";
                return View("IslemHata");
            }

            return View(islem);
        }

        // İşlem Düzenleme (POST)
        [HttpPost]
        public async Task<IActionResult> IslemDuzenle(int id, [Bind("IslemID,IslemAdi,Ucret,Aciklama,Sure")] Islem islem)
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
                    islem.SalonID = 1;
                    _context.Islemler.Update(islem);
                    await _context.SaveChangesAsync();

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
        public async Task<IActionResult> IslemSil(int? id)
        {
            if (id is null)
            {
                TempData["hata"] = "Silme işlemi için işlem ID gerekli.";
                return View("IslemHata");
            }

            var islem = await _context.Islemler.FindAsync(id);
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
            await _context.SaveChangesAsync();

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
