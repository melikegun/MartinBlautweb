using MartinBlautweb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Islem
    {
        [Key]
        public int IslemID { get; set; }

        [Display(Name = "İşlem Adı")]
        [Required(ErrorMessage = "İşlem adı boş bırakılmamalıdır.")]
        [StringLength(100, ErrorMessage = "İşlem adı 100 karakteri geçemez.")]
        public string IslemAdi { get; set; }

        [Display(Name = "Ücret")]
        [Required(ErrorMessage = "Ücret boş bırakılmamalıdır.")]
        [Range(0, double.MaxValue, ErrorMessage = "Ücret geçerli bir değer olmalıdır.")]
        public double Ucret { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(500, ErrorMessage = "Açıklama 500 karakteri geçemez.")]
        public string Aciklama { get; set; }

        [Display(Name = "Süre (Dakika)")]
        [Required(ErrorMessage = "Süre boş bırakılmamalıdır.")]
        [Range(1, int.MaxValue, ErrorMessage = "Süre 1 dakikadan küçük olamaz.")]
        public int Sure { get; set; }

        // Bir işlem birden fazla çalışan tarafından yapılabilir
        public ICollection<Calisan> Calisanlar { get; set; }

        // İşleme ait randevuları tutmak için ilişki
        public ICollection<Randevu> Randevular { get; set; }

        public Islem()
        {
            Calisanlar = new List<Calisan>();
            Randevular = new List<Randevu>();
        }
    }
}

