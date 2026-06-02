using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class PhongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? toaNha, string? trangThai)
        {
            var query = _context.Phongs.Include(p => p.SinhViens).AsQueryable();

            if (!string.IsNullOrEmpty(toaNha))
                query = query.Where(p => p.ToaNha == toaNha);
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

            ViewBag.ToaNha = toaNha;
            ViewBag.TrangThai = trangThai;
            ViewBag.DanhSachToaNha = await _context.Phongs.Select(p => p.ToaNha).Distinct().ToListAsync();

            return View(await query.OrderBy(p => p.ToaNha).ThenBy(p => p.MaPhong).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var phong = await _context.Phongs
                .Include(p => p.SinhViens)
                .Include(p => p.TaiSans)
                .Include(p => p.ChiSoDiens)
                .Include(p => p.ChiSoNuocs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phong == null) return NotFound();
            return View(phong);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong model)
        {
            if (await _context.Phongs.AnyAsync(p => p.MaPhong == model.MaPhong))
                ModelState.AddModelError("MaPhong", "Mã phòng đã tồn tại.");

            if (ModelState.IsValid)
            {
                _context.Phongs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phong model)
        {
            if (id != model.Id) return NotFound();

            if (await _context.Phongs.AnyAsync(p => p.MaPhong == model.MaPhong && p.Id != id))
                ModelState.AddModelError("MaPhong", "Mã phòng đã tồn tại.");

            // Kiểm tra sức chứa mới không nhỏ hơn số người hiện tại
            var soNguoiHienTai = await _context.SinhViens.CountAsync(s => s.PhongId == id && s.TrangThai == "Đang nội trú");
            if (model.SucChua < soNguoiHienTai)
                ModelState.AddModelError("SucChua", $"Sức chứa không thể nhỏ hơn số người đang ở ({soNguoiHienTai} người).");

            if (ModelState.IsValid)
            {
                model.SoNguoiHienTai = soNguoiHienTai;
                model.TrangThai = soNguoiHienTai >= model.SucChua ? "Đầy" : model.TrangThai;
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return NotFound();

            var coSinhVien = await _context.SinhViens.AnyAsync(s => s.PhongId == id);
            if (coSinhVien)
            {
                TempData["Error"] = "Không thể xóa phòng đang có sinh viên.";
                return RedirectToAction(nameof(Index));
            }

            _context.Phongs.Remove(phong);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa phòng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
