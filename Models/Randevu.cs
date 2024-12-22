using System;
using System.ComponentModel.DataAnnotations;

namespace MartinBlautweb.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuID { get; set; }  // Randevunun benzersiz kimliği

        [Display(Name = "Randevu Tarihi")]
        [Required(ErrorMessage = "Randevu tarihi boş bırakılmamalıdır.")]
        public DateTime RandevuTarihi { get; set; }  // Randevunun tarihi (Date)

        [Display(Name = "Randevu Saati")]
        [Required(ErrorMessage = "Randevu saati boş bırakılmamalıdır.")]
        [DataType(DataType.Time, ErrorMessage = "Geçerli bir saat giriniz.")]
        public TimeSpan RandevuSaati { get; set; }  // Randevunun saati (TimeSpan)

        [Display(Name = "Onay Durumu")]
        public bool OnayDurumu { get; set; }  // Randevu onay durumu

        // İlişkili Propertyler
        public int? CalisanID { get; set; }  // İlgili çalışanın ID'si
        public Calisan Calisan { get; set; }  // Çalışanla olan ilişki

        public int? KullaniciID { get; set; }  // İlgili kullanıcının ID'si
        public Kullanici Kullanici { get; set; }  // Kullanıcıyla olan ilişki

        public int? IslemID { get; set; }  // Yapılacak işlemin ID'si
        public Islem Islem { get; set; }  // İşlemle olan ilişki

        // İlgili salon ile ilişki
        public int? SalonID { get; set; } // Varsayılan olarak 1 olacak
        public Salon Salon { get; set; } // Navigation Property

        // Randevunun ücretini işlemin ücretinden alıyoruz
        [Display(Name = "Randevu Ücreti")]
        public double RandevuUcreti => Islem?.Ucret ?? 0;  // İşlemden alınan ücret

        // Constructor
        public Randevu()
        {
        }
    }
}
