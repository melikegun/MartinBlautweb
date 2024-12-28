using Microsoft.EntityFrameworkCore;
using MartinBlautweb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MartinBlautweb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Kullanici>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        // DbSet tanımları
        public DbSet<Salon> Salonlar { get; set; }
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Salon başlangıç verisi
            modelBuilder.Entity<Salon>().HasData(
                new Salon
                {
                    SalonID = 1,
                    SalonAdi = "Martin Blaut",
                    SalonAdres = "İstanbul/Ataşehir",
                    SalonTelefon = "05452745680",
                    SalonAcilisSaati = new TimeSpan(8, 0, 0),
                    SalonKapanisSaati = new TimeSpan(21, 0, 0),
                    SalonAciklama = "En kaliteli hizmeti sunuyoruz!"
                }
            );

            modelBuilder.Entity<Calisan>()
                .HasMany(c => c.Yetenekler) // Çalışanın yetenekleri
                .WithMany(i => i.Calisanlar) // İşlemi yapan çalışanlar
                .UsingEntity<Dictionary<string, object>>(
                    "CalisanIslem", // Ortak tablo adı
                    j => j.HasOne<Islem>().WithMany().HasForeignKey("IslemID"), // İşlem tablosuna yabancı anahtar
                    j => j.HasOne<Calisan>().WithMany().HasForeignKey("CalisanID") // Çalışan tablosuna yabancı anahtar
                );

            // Çalışan ve Uzmanlık Alanı (Islem) arasındaki bire çok ilişki
            modelBuilder.Entity<Calisan>()
                .HasOne(c => c.UzmanlikAlan) // Çalışanın uzmanlık alanı
                .WithMany() // İşlem tarafında koleksiyon yok
                .HasForeignKey(c => c.UzmanlikAlanID) // Yabancı anahtar
                .OnDelete(DeleteBehavior.Restrict); // Silme davranışı
        }
    }
}
