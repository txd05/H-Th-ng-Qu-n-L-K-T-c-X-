using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
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
                _context.SinhViens.Add(model);
                await _context.SaveChangesAsync();

                // Cập nhật số người trong phòng
                if (model.PhongId.HasValue)
                    await CapNhatSoNguoiPhong(model.PhongId.Value);

                TempData["Success"] = "Thêm sinh viên thành công!";
                return RedirectToAction(nameof(Index));
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

            var phongId = sv.PhongId;
            _context.SinhViens.Remove(sv);
            await _context.SaveChangesAsync();

            if (phongId.HasValue) await CapNhatSoNguoiPhong(phongId.Value);

            TempData["Success"] = "Đã xóa sinh viên.";
            return RedirectToAction(nameof(Index));
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
