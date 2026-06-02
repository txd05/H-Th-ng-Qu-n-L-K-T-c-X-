using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SinhVienController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, string? trangThai, string? gioiTinh)
        {
            var query = _context.SinhViens.Include(s => s.Phong).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.HoTen.Contains(search) || s.MaSinhVien.Contains(search));

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(s => s.TrangThai == trangThai);

            if (!string.IsNullOrEmpty(gioiTinh))
                query = query.Where(s => s.GioiTinh == gioiTinh);

            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            ViewBag.GioiTinh = gioiTinh;

            return View(await query.OrderBy(s => s.MaSinhVien).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var sv = await _context.SinhViens
                .Include(s => s.Phong)
                .Include(s => s.TheKTX)
                .Include(s => s.HoaDons)
                .Include(s => s.DonYeuCaus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sv == null) return NotFound();
            return View(sv);
        }

        public IActionResult Create()
        {
            // Truyền toàn bộ phòng còn chỗ kèm GioiTinh để JS filter
            ViewBag.Phongs = _context.Phongs
                .Where(p => p.TrangThai != "Đóng cửa" && p.TrangThai != "Đầy")
                .OrderBy(p => p.MaPhong)
                .Select(p => new { p.Id, p.MaPhong, p.GioiTinh, p.SoNguoiHienTai, p.SucChua })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien model)
        {
            if (await _context.SinhViens.AnyAsync(s => s.MaSinhVien == model.MaSinhVien))
                ModelState.AddModelError("MaSinhVien", "Mã sinh viên đã tồn tại.");

            if (ModelState.IsValid)
            {
                model.NgayVaoKTX = DateTime.Now;

                // Email đăng nhập: dùng gmail SV nếu có, không thì dùng MSSV@ktx.edu.vn
                var loginEmail = !string.IsNullOrEmpty(model.Email)
                    ? model.Email.Trim()
                    : $"{model.MaSinhVien}@ktx.edu.vn";
                var defaultPassword = model.MaSinhVien;

                // Xóa tài khoản cũ nếu email đã tồn tại (orphan)
                var existingUser = await _userManager.FindByEmailAsync(loginEmail);
                if (existingUser != null) await _userManager.DeleteAsync(existingUser);

                var user = new IdentityUser
                {
                    UserName = loginEmail,
                    Email = loginEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SinhVien");
                    model.UserId = user.Id;
                    // Đồng bộ email vào hồ sơ nếu chưa có
                    if (string.IsNullOrEmpty(model.Email))
                        model.Email = loginEmail;
                }
                else
                {
                    // Không tạo được tài khoản → vẫn lưu hồ sơ, thông báo lỗi nhẹ
                    TempData["Warning"] = $"Hồ sơ đã lưu nhưng không tạo được tài khoản: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }

                _context.SinhViens.Add(model);
                await _context.SaveChangesAsync();

                if (model.PhongId.HasValue)
                    await CapNhatSoNguoiPhong(model.PhongId.Value);

                TempData["Success"] = $"Thêm sinh viên <b>{model.HoTen}</b> thành công!<br/>" +
                    $"<i class='bi bi-key me-1'></i>Tài khoản: <b>{loginEmail}</b> &nbsp;|&nbsp; " +
                    $"Mật khẩu mặc định: <b>{defaultPassword}</b> (chính là MSSV)";

                // Redirect về Details để Admin thấy ngay thông tin tài khoản
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            ViewBag.Phongs = _context.Phongs
                .Where(p => p.TrangThai != "Đóng cửa" && p.TrangThai != "Đầy")
                .OrderBy(p => p.MaPhong)
                .Select(p => new { p.Id, p.MaPhong, p.GioiTinh, p.SoNguoiHienTai, p.SucChua })
                .ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();
            ViewBag.Phongs = _context.Phongs
                .Where(p => p.TrangThai != "Đóng cửa")
                .OrderBy(p => p.MaPhong)
                .Select(p => new { p.Id, p.MaPhong, p.GioiTinh, p.SoNguoiHienTai, p.SucChua })
                .ToList();
            return View(sv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SinhVien model)
        {
            if (id != model.Id) return NotFound();

            if (await _context.SinhViens.AnyAsync(s => s.MaSinhVien == model.MaSinhVien && s.Id != id))
                ModelState.AddModelError("MaSinhVien", "Mã sinh viên đã tồn tại.");

            if (ModelState.IsValid)
            {
                var old = await _context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                _context.Update(model);
                await _context.SaveChangesAsync();

                // Cập nhật số người phòng cũ và mới
                if (old?.PhongId != model.PhongId)
                {
                    if (old?.PhongId.HasValue == true) await CapNhatSoNguoiPhong(old.PhongId.Value);
                    if (model.PhongId.HasValue) await CapNhatSoNguoiPhong(model.PhongId.Value);
                }

                TempData["Success"] = "Cập nhật sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Phongs = _context.Phongs
                .Where(p => p.TrangThai != "Đóng cửa")
                .OrderBy(p => p.MaPhong)
                .Select(p => new { p.Id, p.MaPhong, p.GioiTinh, p.SoNguoiHienTai, p.SucChua })
                .ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            // Xóa tài khoản Identity kèm theo
            if (!string.IsNullOrEmpty(sv.UserId))
            {
                var user = await _userManager.FindByIdAsync(sv.UserId);
                if (user != null) await _userManager.DeleteAsync(user);
            }

            var phongId = sv.PhongId;
            _context.SinhViens.Remove(sv);
            await _context.SaveChangesAsync();

            if (phongId.HasValue) await CapNhatSoNguoiPhong(phongId.Value);

            TempData["Success"] = "Đã xóa sinh viên và tài khoản đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Tạo tài khoản cho sinh viên đã có hồ sơ nhưng chưa có tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTaiKhoan(int id)
        {
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            // Đã có tài khoản rồi
            if (!string.IsNullOrEmpty(sv.UserId))
            {
                TempData["Warning"] = "Sinh viên này đã có tài khoản đăng nhập.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Dùng gmail SV nếu có, không thì MSSV@ktx.edu.vn
            var loginEmail = !string.IsNullOrEmpty(sv.Email)
                ? sv.Email.Trim()
                : $"{sv.MaSinhVien}@ktx.edu.vn";
            var defaultPassword = sv.MaSinhVien;

            // Xóa tài khoản cũ nếu email đã tồn tại (orphan)
            var existing = await _userManager.FindByEmailAsync(loginEmail);
            if (existing != null)
                await _userManager.DeleteAsync(existing);

            var user = new IdentityUser
            {
                UserName = loginEmail,
                Email = loginEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, defaultPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SinhVien");
                sv.UserId = user.Id;
                if (string.IsNullOrEmpty(sv.Email)) sv.Email = loginEmail;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Tạo tài khoản thành công! Email: <b>{loginEmail}</b> — Mật khẩu: <b>{defaultPassword}</b>";
            }
            else
            {
                TempData["Error"] = "Tạo tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Reset mật khẩu sinh viên về mặc định
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetMatKhau(int id)
        {
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            if (string.IsNullOrEmpty(sv.UserId))
            {
                TempData["Error"] = "Sinh viên này chưa có tài khoản.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var user = await _userManager.FindByIdAsync(sv.UserId);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var newPassword = sv.MaSinhVien;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
                TempData["Success"] = $"Đã reset mật khẩu về: <b>{newPassword}</b>";
            else
                TempData["Error"] = "Reset mật khẩu thất bại.";

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task CapNhatSoNguoiPhong(int phongId)
        {
            var phong = await _context.Phongs.FindAsync(phongId);
            if (phong != null)
            {
                phong.SoNguoiHienTai = await _context.SinhViens
                    .CountAsync(s => s.PhongId == phongId && s.TrangThai == "Đang nội trú");
                phong.TrangThai = phong.SoNguoiHienTai >= phong.SucChua ? "Đầy" : "Còn chỗ";
                await _context.SaveChangesAsync();
            }
        }
    }
}
