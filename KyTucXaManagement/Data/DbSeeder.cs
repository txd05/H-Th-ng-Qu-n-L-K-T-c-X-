using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace KyTucXaManagement.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Tạo roles
            string[] roles = { "Admin", "NhanVien", "SinhVien" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Tạo tài khoản Admin
            var adminEmail = "admin@ktx.edu.vn";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Tạo tài khoản Nhân viên
            var nvEmail = "nhanvien@ktx.edu.vn";
            if (await userManager.FindByEmailAsync(nvEmail) == null)
            {
                var nv = new IdentityUser
                {
                    UserName = nvEmail,
                    Email = nvEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(nv, "NhanVien@123");
                await userManager.AddToRoleAsync(nv, "NhanVien");
            }

            // Seed phòng mẫu nếu chưa có
            if (!context.Phongs.Any())
            {
                var phongs = new List<Phong>
                {
                    new Phong { MaPhong = "A101", LoaiPhong = "4 người", SucChua = 4, Tang = "1", ToaNha = "A", GioiTinh = "Nam", TrangThai = "Còn chỗ", GiaPhong = 500000 },
                    new Phong { MaPhong = "A102", LoaiPhong = "4 người", SucChua = 4, Tang = "1", ToaNha = "A", GioiTinh = "Nam", TrangThai = "Còn chỗ", GiaPhong = 500000 },
                    new Phong { MaPhong = "A201", LoaiPhong = "8 người", SucChua = 8, Tang = "2", ToaNha = "A", GioiTinh = "Nam", TrangThai = "Còn chỗ", GiaPhong = 350000 },
                    new Phong { MaPhong = "B101", LoaiPhong = "4 người", SucChua = 4, Tang = "1", ToaNha = "B", GioiTinh = "Nữ", TrangThai = "Còn chỗ", GiaPhong = 500000 },
                    new Phong { MaPhong = "B102", LoaiPhong = "4 người", SucChua = 4, Tang = "1", ToaNha = "B", GioiTinh = "Nữ", TrangThai = "Còn chỗ", GiaPhong = 500000 },
                    new Phong { MaPhong = "B201", LoaiPhong = "8 người", SucChua = 8, Tang = "2", ToaNha = "B", GioiTinh = "Nữ", TrangThai = "Còn chỗ", GiaPhong = 350000 },
                };
                context.Phongs.AddRange(phongs);
                await context.SaveChangesAsync();
            }
        }
    }
}
