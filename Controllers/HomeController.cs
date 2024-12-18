using System.Diagnostics;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;

namespace MartinBlautweb.Controllers
{
    //Calisanlar için yapabildiði iþlemler, müsaitlik durumlarýný, verimliliðini ve günlük kazançlarýný 

    //Randevu Sistemi
    // o Kullanýcýlar, uygun çalýþanlara ve iþlemlere göre sistem üzerinden randevu alabilecek.
    // o Randevu saati önceki randevular dikkate alýnýp uygun deðilse uyarmalý)
    // o Randevu detaylarý (iþlem, süre, ücret) sistemde saklanacak.
    // o Randevu onay mekanizmasý olacak.


    //4. REST API Kullanýmý
    //o Projenin en az bir kýsmýnda REST API kullanarak veri tabaný ile iletiþim saðlanacak

    //Javascript ve JQUERY  kullanýmý da istiyor

    //5. Yapay Zeka Entegrasyonu
    //o Projede bir yapay zeka aracý ile entegre çalýþan bir özellik bulunmalýdýr.
    //o Kullanýcýlar sisteme bir fotoðraf yükleyerek, yapay zeka aracýlýðýyla saç kesim modelleri veya saç rengi önerileri alabilecek.

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
