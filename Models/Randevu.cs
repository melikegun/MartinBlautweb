using System;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuID { get; set; }

        [Display(Name = "Randevu Tarihi")]
        [Required(ErrorMessage = "Randevu tarihi boş bırakılmamalıdır.")]
        public DateTime RandevuTarihi { get; set; }

        [Display(Name = "Randevu Saati")]
        [Required(ErrorMessage = "Randevu saati boş bırakılmamalıdır.")]
        [DataType(DataType.Time, ErrorMessage = "Geçerli bir saat giriniz.")]
        public TimeSpan RandevuSaati { get; set; }

        [Display(Name = "Onay Durumu")]
        public bool OnayDurumu { get; set; }

        // İlişkili Propertyler
        public int? CalisanID { get; set; }
        public Calisan Calisan { get; set; }

        public string KullaniciId { get; set; }  // IdentityUser sınıfındaki Id ile ilişkilendirilir
        public Kullanici Kullanici { get; set; }  // Kullanıcı ile olan ilişki

        public int? IslemID { get; set; }
        public Islem Islem { get; set; }

        public int? SalonID { get; set; }
        public Salon Salon { get; set; }

        [Display(Name = "Randevu Ücreti")]
        public double RandevuUcreti => Islem?.Ucret ?? 0;

        public Randevu()
        {
        }
    }

}
