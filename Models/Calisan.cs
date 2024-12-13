using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Calisan
    {
        [Key]
        public int CalisanID { get; set; }

        [Display(Name = "Çalışan Adı")]
        [Required(ErrorMessage = "Çalışan adı boş bırakılmamalıdır.")]
        [StringLength(100, ErrorMessage = "Çalışan adı 100 karakteri geçemez.")]
        public string CalisanAd { get; set; }

        [Display(Name = "Çalışan Soyadı")]
        [Required(ErrorMessage = "Çalışan soyadı boş bırakılmamalıdır.")]
        [StringLength(100, ErrorMessage = "Çalışan soyadı 100 karakteri geçemez.")]
        public string CalisanSoyad { get; set; }

        // İşlemle ilişkilendirilmiş uzmanlık alanı
        [Display(Name = "Uzmanlık Alanı")]
        [Required(ErrorMessage = "Uzmanlık Alanı seçilmelidir.")]
        public int IslemID { get; set; }  // Nullable olmayacak şekilde düzenlendi.

        public Islem Islem { get; set; }  // Navigation Property

        [Display(Name = "Telefon Numarası")]
        [StringLength(11, ErrorMessage = "Telefon numarası 11 karakter olmalıdır.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefon numarası geçerli bir değer olmalıdır.")]
        public string CalisanTelefon { get; set; }

        [Display(Name = "Mesai Başlangıç Saati")]
        [Required(ErrorMessage = "Mesai başlangıç saati boş bırakılmamalıdır.")]
        public TimeSpan CalisanMesaiBaslangic { get; set; }

        [Display(Name = "Mesai Bitiş Saati")]
        [Required(ErrorMessage = "Mesai bitiş saati boş bırakılmamalıdır.")]
        public TimeSpan CalisanMesaiBitis { get; set; }

        public ICollection<Randevu> Randevular { get; set; }

        public Calisan()
        {
            Randevular = new List<Randevu>();
        }
    }
}
