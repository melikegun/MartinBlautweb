using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Salon
    {
        [Key]
        public int SalonID { get; set; }

        [Display(Name = "Salon Adı")]
        [Required(ErrorMessage = "Salon adı boş bırakmayın.")]
        [StringLength(100, ErrorMessage = "Salon adı 100 karakteri geçemez.")]
        public string SalonAdi { get; set; }

        [Display(Name = "Salon Adresi")]
        [StringLength(200, ErrorMessage = "Adres 200 karakteri geçemez.")]
        public string SalonAdres { get; set; }

        [Display(Name = "Salon Telefon")]
        [StringLength(11, ErrorMessage = "Telefon numarası 11 karakter olmalıdır.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefon numarası geçerli olmalıdır (örneğin: 5551234567).")]
        public string SalonTelefon { get; set; }

        [Display(Name = "Açılış Saati")]
        [Required(ErrorMessage = "Açılış saati boş bırakmayın.")]
        public TimeSpan SalonAcilisSaati { get; set; }

        [Display(Name = "Kapanış Saati")]
        [Required(ErrorMessage = "Kapanış saati boş bırakmayın.")]
        public TimeSpan SalonKapanisSaati { get; set; }

        [Display(Name = "Salon Açıklaması")]
        [StringLength(500, ErrorMessage = "Açıklama 500 karakteri geçemez.")]
        public string SalonAciklama { get; set; }

        // İlişkili Propertyler
        public ICollection<Calisan> Calisanlar { get; set; }
        public ICollection<Islem> Islemler { get; set; }

        // Constructor
        public Salon()
        {
            Calisanlar = new List<Calisan>();
            Islemler = new List<Islem>();
        }
    }
}
