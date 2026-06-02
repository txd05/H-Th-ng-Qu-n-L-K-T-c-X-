using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? trangThai, int? thang, int? nam)
        {
            var query = _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.Phong)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(h => h.TrangThai == trangThai);
            if (thang.HasValue)
                query = query.Where(h => h.Thang == thang);
            if (nam.HasValue)
                query = query.Where(h => h.Nam == nam);

            ViewBag.TrangThai = trangThai;
            ViewBag.Thang = thang ?? DateTime.Now.Month;
            ViewBag.Nam = nam ?? DateTime.Now.Year;

            var hoaDons = await query.OrderByDescending(h => h.NgayLap).ToListAsync();

            ViewBag.TongTienChuaThanhToan = hoaDons
                .Where(h => h.TrangThai == "Chưa thanh toán")
                .Sum(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac);

            return View(hoaDons);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hd = await _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.Phong)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hd == null) return NotFound();
            return View(hd);
        }

        public IActionResult Create()
        {
            ViewBag.SinhViens = new SelectList(_context.SinhViens.Where(s => s.TrangThai == "Đang nội trú"), "Id", "HoTen");
            ViewBag.Phongs = new SelectList(_context.Phongs, "Id", "MaPhong");
            return View(new HoaDon { Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, NgayLap = DateTime.Now, HanThanhToan = DateTime.Now.AddDays(15) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon model)
        {
            if (ModelState.IsValid)
            {
                model.MaHoaDon = $"HD{DateTime.Now:yyyyMMddHHmmss}";
                _context.HoaDons.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo hóa đơn thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SinhViens = new SelectList(_context.SinhViens.Where(s => s.TrangThai == "Đang nội trú"), "Id", "HoTen");
            ViewBag.Phongs = new SelectList(_context.Phongs, "Id", "MaPhong");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(int id)
        {
            var hd = await _context.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            if (hd.TrangThai == "Đã thanh toán")
            {
                TempData["Error"] = "Hóa đơn này đã được thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            hd.TrangThai = "Đã thanh toán";
            hd.NgayThanhToan = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xác nhận thanh toán thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hd = await _context.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            if (hd.TrangThai == "Đã thanh toán")
            {
                TempData["Error"] = "Không thể xóa hóa đơn đã thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            _context.HoaDons.Remove(hd);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa hóa đơn.";
            return RedirectToAction(nameof(Index));
        }

        // Lập hóa đơn tự động từ chỉ số điện nước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LapHoaDonTuDong(int thang, int nam)
        {
            var phongs = await _context.Phongs
                .Include(p => p.SinhViens)
                .Include(p => p.ChiSoDiens.Where(c => c.Thang == thang && c.Nam == nam))
                .Include(p => p.ChiSoNuocs.Where(c => c.Thang == thang && c.Nam == nam))
                .ToListAsync();

            int count = 0;
            foreach (var phong in phongs)
            {
                var svTrongPhong = phong.SinhViens.Where(s => s.TrangThai == "Đang nội trú").ToList();
                if (!svTrongPhong.Any()) continue;

                var chiSoDien = phong.ChiSoDiens.FirstOrDefault();
                var chiSoNuoc = phong.ChiSoNuocs.FirstOrDefault();

                double tienDien = chiSoDien != null ? (chiSoDien.ChiSoCuoi - chiSoDien.ChiSoDau) * chiSoDien.DonGia : 0;
                double tienNuoc = chiSoNuoc != null ? (chiSoNuoc.ChiSoCuoi - chiSoNuoc.ChiSoDau) * chiSoNuoc.DonGia : 0;
                double tienDienMoiNguoi = svTrongPhong.Count > 0 ? tienDien / svTrongPhong.Count : 0;
                double tienNuocMoiNguoi = svTrongPhong.Count > 0 ? tienNuoc / svTrongPhong.Count : 0;

                foreach (var sv in svTrongPhong)
                {
                    // Kiểm tra hóa đơn chưa tồn tại
                    bool exists = await _context.HoaDons.AnyAsync(h => h.SinhVienId == sv.Id && h.Thang == thang && h.Nam == nam);
                    if (exists) continue;

                    var hd = new HoaDon
                    {
                        MaHoaDon = $"HD{thang:D2}{nam}{sv.MaSinhVien}",
                        SinhVienId = sv.Id,
                        PhongId = phong.Id,
                        Thang = thang,
                        Nam = nam,
                        TienPhong = phong.GiaPhong,
                        TienDien = Math.Round(tienDienMoiNguoi, 0),
                        TienNuoc = Math.Round(tienNuocMoiNguoi, 0),
                        NgayLap = DateTime.Now,
                        HanThanhToan = new DateTime(nam, thang, 1).AddMonths(1).AddDays(15),
                        TrangThai = "Chưa thanh toán"
                    };
                    _context.HoaDons.Add(hd);
                    count++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã lập {count} hóa đơn tự động cho tháng {thang}/{nam}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
