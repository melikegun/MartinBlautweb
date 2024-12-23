using System.ComponentModel.DataAnnotations;
using MartinBlautweb.Models;
using System;

namespace MartinBlautweb.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string AdminMail { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter uzunluğunda olmalıdır.")]
        public string AdminSifre { get; set; }

        [Required]
        [StringLength(50)]
        public string AdminAd { get; set; }

        [Required]
        [StringLength(50)]
        public string AdminSoyad { get; set; }

        // Constructor
        public Admin()
        {
            
        }
    }
}
