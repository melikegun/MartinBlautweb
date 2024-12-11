using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartinBlautweb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kulllanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciMail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KullaniciSifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KullaniciAd = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KullaniciSoyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kulllanicilar", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "Salonlar",
                columns: table => new
                {
                    SalonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SalonAdres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SalonTelefon = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    SalonAcilisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    SalonKapanisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    SalonAciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salonlar", x => x.SalonID);
                });

            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    IslemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IslemAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ucret = table.Column<double>(type: "float", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Sure = table.Column<int>(type: "int", nullable: false),
                    SalonID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.IslemID);
                    table.ForeignKey(
                        name: "FK_Islemler_Salonlar_SalonID",
                        column: x => x.SalonID,
                        principalTable: "Salonlar",
                        principalColumn: "SalonID");
                });

            migrationBuilder.CreateTable(
                name: "Calisanlar",
                columns: table => new
                {
                    CalisanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CalisanSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IslemID = table.Column<int>(type: "int", nullable: true),
                    CalisanTelefon = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CalisanMesaiBaslangic = table.Column<TimeSpan>(type: "time", nullable: false),
                    CalisanMesaiBitis = table.Column<TimeSpan>(type: "time", nullable: false),
                    SalonID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calisanlar", x => x.CalisanID);
                    table.ForeignKey(
                        name: "FK_Calisanlar_Islemler_IslemID",
                        column: x => x.IslemID,
                        principalTable: "Islemler",
                        principalColumn: "IslemID");
                    table.ForeignKey(
                        name: "FK_Calisanlar_Salonlar_SalonID",
                        column: x => x.SalonID,
                        principalTable: "Salonlar",
                        principalColumn: "SalonID");
                });

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    RandevuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RandevuTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RandevuSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    IslemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.RandevuID);
                    table.ForeignKey(
                        name: "FK_Randevular_Calisanlar_CalisanID",
                        column: x => x.CalisanID,
                        principalTable: "Calisanlar",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Randevular_Islemler_IslemID",
                        column: x => x.IslemID,
                        principalTable: "Islemler",
                        principalColumn: "IslemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Randevular_Kulllanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kulllanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Salonlar",
                columns: new[] { "SalonID", "SalonAciklama", "SalonAcilisSaati", "SalonAdi", "SalonAdres", "SalonKapanisSaati", "SalonTelefon" },
                values: new object[] { 1, "En kaliteli hizmeti sunuyoruz!", new TimeSpan(0, 9, 0, 0, 0), "Martin's Salon", "123 Salon Caddesi, İstanbul", new TimeSpan(0, 19, 0, 0, 0), "5551234567" });

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_IslemID",
                table: "Calisanlar",
                column: "IslemID");

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_SalonID",
                table: "Calisanlar",
                column: "SalonID");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_SalonID",
                table: "Islemler",
                column: "SalonID");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_CalisanID",
                table: "Randevular",
                column: "CalisanID");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_IslemID",
                table: "Randevular",
                column: "IslemID");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_KullaniciID",
                table: "Randevular",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.DropTable(
                name: "Calisanlar");

            migrationBuilder.DropTable(
                name: "Kulllanicilar");

            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "Salonlar");
        }
    }
}
