using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Kullanici : IdentityUser
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad boş bırakılmamalıdır.")]
        [StringLength(50, ErrorMessage = "Ad 50 karakteri geçemez.")]
        public string KullaniciAd { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad boş bırakılmamalıdır.")]
        [StringLength(50, ErrorMessage = "Soyad 50 karakteri geçemez.")]
        public string KullaniciSoyad { get; set; }

        [Display(Name = "Telefon Numarası")]
        [StringLength(11, ErrorMessage = "Telefon numarası 11 karakter olmalıdır.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefon numarası geçerli bir değer olmalıdır.")]
        public string KullaniciTelefon { get; set; }

        // Kullanıcıya ait randevuları tutmak için ilişki
        public ICollection<Randevu> Randevular { get; set; }

        public Kullanici()
        {
            Randevular = new List<Randevu>();
        }
    }

}
