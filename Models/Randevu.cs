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

        // İlişkili Propertyler
        public int CalisanID { get; set; }  // İlgili çalışanın ID'si
        public Calisan Calisan { get; set; }  // Çalışanla olan ilişki

        public int KullaniciID { get; set; }  // İlgili kullanıcının ID'si
        public Kullanici Kullanici { get; set; }  // Kullanıcıyla olan ilişki

        public int IslemID { get; set; }  // Yapılacak işlemin ID'si
        public Islem Islem { get; set; }  // İşlemle olan ilişki

        // Constructor
        public Randevu()
        {
        }
    }
}
