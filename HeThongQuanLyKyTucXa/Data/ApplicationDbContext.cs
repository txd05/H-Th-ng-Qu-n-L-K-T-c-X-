using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiSoDien> ChiSoDiens { get; set; }
        public DbSet<ChiSoNuoc> ChiSoNuocs { get; set; }
        public DbSet<TaiSan> TaiSans { get; set; }
        public DbSet<SuCoTaiSan> SuCoTaiSans { get; set; }
        public DbSet<DonYeuCau> DonYeuCaus { get; set; }
        public DbSet<TheKTX> TheKTXs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Phong
            modelBuilder.Entity<Phong>()
                .HasIndex(p => p.MaPhong)
                .IsUnique();

            // SinhVien
            modelBuilder.Entity<SinhVien>()
                .HasIndex(s => s.MaSinhVien)
                .IsUnique();

            modelBuilder.Entity<SinhVien>()
                .HasOne(s => s.Phong)
                .WithMany(p => p.SinhViens)
                .HasForeignKey(s => s.PhongId)
                .OnDelete(DeleteBehavior.SetNull);

            // TheKTX 1-1 với SinhVien
            modelBuilder.Entity<TheKTX>()
                .HasOne(t => t.SinhVien)
                .WithOne(s => s.TheKTX)
                .HasForeignKey<TheKTX>(t => t.SinhVienId)
                .OnDelete(DeleteBehavior.Cascade);

            // HoaDon
            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.SinhVien)
                .WithMany(s => s.HoaDons)
                .HasForeignKey(h => h.SinhVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignore computed properties
            modelBuilder.Entity<HoaDon>()
                .Ignore(h => h.TongTien);

            modelBuilder.Entity<ChiSoDien>()
                .Ignore(c => c.TieuThu)
                .Ignore(c => c.ThanhTien);

            modelBuilder.Entity<ChiSoNuoc>()
                .Ignore(c => c.TieuThu)
                .Ignore(c => c.ThanhTien);

            // SuCoTaiSan
            modelBuilder.Entity<SuCoTaiSan>()
                .HasOne(s => s.SinhVienBao)
                .WithMany()
                .HasForeignKey(s => s.SinhVienBaoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
