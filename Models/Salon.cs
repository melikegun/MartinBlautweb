using System;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Salon
    {
        [Key]
        public int SalonID { get; set; }  // Salonun benzersiz kimliği

        [Display(Name = "Salon Adı")]
        [Required(ErrorMessage = "Salon adı boş bırakılmamalıdır.")]
        [StringLength(100, ErrorMessage = "Salon adı 100 karakterden fazla olamaz.")]
        public string SalonAdi { get; set; }  // Salonun adı

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Salon adresi boş bırakılmamalıdır.")]
        [StringLength(200, ErrorMessage = "Adres 200 karakterden fazla olamaz.")]
        public string SalonAdres { get; set; }  // Salonun adresi

        [Display(Name = "Telefon Numarası")]
        [Required(ErrorMessage = "Telefon numarası boş bırakılmamalıdır.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string SalonTelefon { get; set; }  // Telefon numarası

        [Display(Name = "Açılış Saati")]
        [Required(ErrorMessage = "Açılış saati boş bırakılmamalıdır.")]
        public TimeSpan SalonAcilisSaati { get; set; }  // Açılış saati

        [Display(Name = "Kapanış Saati")]
        [Required(ErrorMessage = "Kapanış saati boş bırakılmamalıdır.")]
        public TimeSpan SalonKapanisSaati { get; set; }  // Kapanış saati

        [Display(Name = "Açıklama")]
        [StringLength(500, ErrorMessage = "Açıklama 500 karakterden fazla olamaz.")]
        public string SalonAciklama { get; set; }  // Ek açıklamalar


        // İlişkili Propertyler
        public ICollection<Calisan>? Calisanlar { get; set; }
        public ICollection<Islem>? Islemler { get; set; }
        public ICollection<Randevu>? Randevular { get; set; }

        // Constructor
        public Salon()
        {
            Calisanlar = new List<Calisan>();
            Islemler = new List<Islem>();
        }
    }
}
