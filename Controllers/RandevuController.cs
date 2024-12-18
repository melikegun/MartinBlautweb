using Microsoft.AspNetCore.Mvc;

namespace MartinBlautweb.Controllers
{
    public class RandevuController : Controller
    {
        /*
         * Randevu Sistemi
            o Kullanıcılar, uygun çalışanlara ve işlemlere göre sistem üzerinden randevu alabilecek.
            o Randevu saati önceki randevular dikkate alınıp uygun değilse uyarmalı)
            o Randevu detayları (işlem, süre, ücret) sistemde saklanacak.
            o Randevu onay mekanizması olacak.
        */
        public IActionResult Index()
        {
            return View();
        }
    }
}
