using System.Diagnostics;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Mvc;

namespace MartinBlautweb.Controllers
{
    //Calisanlar i�in yapabildi�i i�lemler, m�saitlik durumlar�n�, verimlili�ini ve g�nl�k kazan�lar�n� 

    //Randevu Sistemi
    // o Kullan�c�lar, uygun �al��anlara ve i�lemlere g�re sistem �zerinden randevu alabilecek.
    // o Randevu saati �nceki randevular dikkate al�n�p uygun de�ilse uyarmal�)
    // o Randevu detaylar� (i�lem, s�re, �cret) sistemde saklanacak.
    // o Randevu onay mekanizmas� olacak.


    //4. REST API Kullan�m�
    //o Projenin en az bir k�sm�nda REST API kullanarak veri taban� ile ileti�im sa�lanacak

    //Javascript ve JQUERY  kullan�m� da istiyor

    //5. Yapay Zeka Entegrasyonu
    //o Projede bir yapay zeka arac� ile entegre �al��an bir �zellik bulunmal�d�r.
    //o Kullan�c�lar sisteme bir foto�raf y�kleyerek, yapay zeka arac�l���yla sa� kesim modelleri veya sa� rengi �nerileri alabilecek.

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
