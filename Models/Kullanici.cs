using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
   
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Display(Name = "E-posta Adresi")]
        [Required(ErrorMessage = "E-posta adresi boş bırakılmamalıdır.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta adresi 100 karakteri geçemez.")]
        public string KullaniciMail { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre boş bırakılmamalıdır.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter uzunluğunda olmalıdır.")]
        public string KullaniciSifre { get; set; }

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad boş bırakılmamalıdır.")]
        [StringLength(50, ErrorMessage = "Ad 50 karakteri geçemez.")]
        public string KullaniciAd { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad boş bırakılmamalıdır.")]
        [StringLength(50, ErrorMessage = "Soyad 50 karakteri geçemez.")]
        public string KullaniciSoyad { get; set; }

        // Kullanıcıya ait randevuları tutmak için ilişki
        public ICollection<Randevu> Randevular { get; set; }

        // Constructor
        public Kullanici()
        {
            Randevular = new List<Randevu>();
        }
    }
}
