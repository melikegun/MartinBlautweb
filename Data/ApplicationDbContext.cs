using Microsoft.EntityFrameworkCore;  // Entity Framework Core kullanımı için gerekli
using MartinBlautweb.Models;  // Modellerinizi burada kullanın

namespace MartinBlautweb.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor: Bağlantı dizesi ile context yapılandırması
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        // DbSet'ler: Veritabanı tabloları ile ilişkili olan modeller
        public DbSet<Salon> Salonlar { get; set; }  // Salonlar tablosu
        public DbSet<Calisan> Calisanlar { get; set; }  // Çalışanlar tablosu
        public DbSet<Islem> Islemler { get; set; }  // İşlemler tablosu
        public DbSet<Randevu> Randevular { get; set; }  // Randevular tablosu
        public DbSet<Kullanici> Kulllanicilar { get; set; }  // Kullanıcılar tablosu

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Veritabanına başlangıç verisini ekleyelim
            modelBuilder.Entity<Salon>().HasData(
                new Salon
                {
                    SalonID = 1,
                    SalonAdi = "Martin's Salon",
                    SalonAdres = "123 Salon Caddesi, İstanbul",
                    SalonTelefon = "5551234567",
                    SalonAcilisSaati = new TimeSpan(9, 0, 0), // 09:00 AM
                    SalonKapanisSaati = new TimeSpan(19, 0, 0), // 07:00 PM
                    SalonAciklama = "En kaliteli hizmeti sunuyoruz!"
                }
            );
        }
    }
}
