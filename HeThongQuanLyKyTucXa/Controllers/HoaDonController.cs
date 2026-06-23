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

        // GET: Danh sách hóa đơn
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

        // GET: Chi tiết hóa đơn
        public async Task<IActionResult> Details(int id)
        {
            var hd = await _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.Phong)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hd == null) return NotFound();
            return View(hd);
        }

        // GET: Tạo hóa đơn
        public IActionResult Create()
        {
            ViewBag.SinhViens = new SelectList(_context.SinhViens.Where(s => s.TrangThai == "Đang nội trú"), "Id", "HoTen");
            ViewBag.Phongs = new SelectList(_context.Phongs, "Id", "MaPhong");
            return View(new HoaDon
            {
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                NgayLap = DateTime.Now,
                HanThanhToan = DateTime.Now.AddDays(15)
            });
        }

        // POST: Tạo hóa đơn
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

        // GET: Chỉnh sửa hóa đơn
        public async Task<IActionResult> Edit(int id)
        {
            var hd = await _context.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();
            if (hd.TrangThai == "Đã thanh toán")
            {
                TempData["Error"] = "Không thể chỉnh sửa hóa đơn đã thanh toán.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SinhViens = new SelectList(_context.SinhViens.Where(s => s.TrangThai == "Đang nội trú"), "Id", "HoTen", hd.SinhVienId);
            ViewBag.Phongs = new SelectList(_context.Phongs, "Id", "MaPhong", hd.PhongId);
            return View(hd);
        }

        // POST: Chỉnh sửa hóa đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoaDon model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var hd = await _context.HoaDons.FindAsync(id);
                if (hd == null) return NotFound();
                if (hd.TrangThai == "Đã thanh toán")
                {
                    TempData["Error"] = "Không thể chỉnh sửa hóa đơn đã thanh toán.";
                    return RedirectToAction(nameof(Index));
                }
                hd.SinhVienId = model.SinhVienId;
                hd.PhongId = model.PhongId;
                hd.Thang = model.Thang;
                hd.Nam = model.Nam;
                hd.TienPhong = model.TienPhong;
                hd.TienDien = model.TienDien;
                hd.TienNuoc = model.TienNuoc;
                hd.PhiKhac = model.PhiKhac;
                hd.HanThanhToan = model.HanThanhToan;
                hd.GhiChu = model.GhiChu;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật hóa đơn thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }
            ViewBag.SinhViens = new SelectList(_context.SinhViens.Where(s => s.TrangThai == "Đang nội trú"), "Id", "HoTen", model.SinhVienId);
            ViewBag.Phongs = new SelectList(_context.Phongs, "Id", "MaPhong", model.PhongId);
            return View(model);
        }

        // GET: In hóa đơn
        public async Task<IActionResult> Print(int id)
        {
            var hd = await _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.Phong)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hd == null) return NotFound();
            return View(hd);
        }

        // POST: Xác nhận thanh toán
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

        // POST: Xóa hóa đơn
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

        // POST: Đánh dấu quá hạn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatQuaHan()
        {
            var hdQuaHan = await _context.HoaDons
                .Where(h => h.TrangThai == "Chưa thanh toán" && h.HanThanhToan < DateTime.Now)
                .ToListAsync();
            foreach (var hd in hdQuaHan)
                hd.TrangThai = "Quá hạn";
            await _context.SaveChangesAsync();
            TempData["Warning"] = $"Đã đánh dấu {hdQuaHan.Count} hóa đơn quá hạn.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Thống kê doanh thu
        public async Task<IActionResult> ThongKe(int? nam)
        {
            int year = nam ?? DateTime.Now.Year;

            var doanhThuTheoThang = await _context.HoaDons
                .Where(h => h.TrangThai == "Đã thanh toán" && h.Nam == year)
                .GroupBy(h => h.Thang)
                .Select(g => new {
                    Thang = g.Key,
                    DoanhThu = g.Sum(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac),
                    SoHoaDon = g.Count()
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            var thongKeTrangThai = await _context.HoaDons
                .Where(h => h.Nam == year)
                .GroupBy(h => h.TrangThai)
                .Select(g => new {
                    TrangThai = g.Key,
                    SoLuong = g.Count(),
                    TongTien = g.Sum(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac)
                })
                .ToListAsync();

            var topNo = await _context.HoaDons
                .Include(h => h.SinhVien)
                .Where(h => h.TrangThai == "Chưa thanh toán")
                .GroupBy(h => new { h.SinhVienId, h.SinhVien!.HoTen, h.SinhVien.MaSinhVien })
                .Select(g => new {
                    g.Key.HoTen,
                    g.Key.MaSinhVien,
                    TongNo = g.Sum(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac)
                })
                .OrderByDescending(x => x.TongNo)
                .Take(5)
                .ToListAsync();

            ViewBag.Nam = year;
            ViewBag.DoanhThuTheoThang = doanhThuTheoThang;
            ViewBag.ThongKeTrangThai = thongKeTrangThai;
            ViewBag.TongDoanhThu = doanhThuTheoThang.Sum(x => x.DoanhThu);
            ViewBag.NamList = Enumerable.Range(DateTime.Now.Year - 3, 5).Reverse().ToList();
            ViewBag.TopNo = topNo;
            return View();
        }

        // POST: Lập hóa đơn tự động
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LapHoaDonTuDong(int thang, int nam)
        {
            // Load tất cả phòng có sinh viên đang nội trú + chỉ số điện/nước tháng này
            var phongs = await _context.Phongs
                .Include(p => p.SinhViens.Where(s => s.TrangThai == "Đang nội trú"))
                .Include(p => p.ChiSoDiens.Where(c => c.Thang == thang && c.Nam == nam))
                .Include(p => p.ChiSoNuocs.Where(c => c.Thang == thang && c.Nam == nam))
                .Where(p => p.SinhViens.Any(s => s.TrangThai == "Đang nội trú"))
                .ToListAsync();

            // Các hóa đơn đã tồn tại tháng này (tránh tạo trùng)
            var hdDaTon = await _context.HoaDons
                .Where(h => h.Thang == thang && h.Nam == nam)
                .Select(h => h.SinhVienId)
                .ToHashSetAsync();

            int count = 0;
            var warnings = new List<string>();

            foreach (var phong in phongs)
            {
                var svTrongPhong = phong.SinhViens.ToList();
                int soNguoi = svTrongPhong.Count;
                if (soNguoi == 0) continue;

                // Lấy chỉ số điện/nước (nếu có)
                var chiSoDien = phong.ChiSoDiens.FirstOrDefault();
                var chiSoNuoc = phong.ChiSoNuocs.FirstOrDefault();

                // Tổng tiền điện/nước cả phòng
                double tongTienDien = chiSoDien != null
                    ? Math.Round((chiSoDien.ChiSoCuoi - chiSoDien.ChiSoDau) * chiSoDien.DonGia, 0)
                    : 0;
                double tongTienNuoc = chiSoNuoc != null
                    ? Math.Round((chiSoNuoc.ChiSoCuoi - chiSoNuoc.ChiSoDau) * chiSoNuoc.DonGia, 0)
                    : 0;

                // Chia đều theo đầu người
                double tienDienMoiNguoi = Math.Round(tongTienDien / soNguoi, 0);
                double tienNuocMoiNguoi = Math.Round(tongTienNuoc / soNguoi, 0);

                if (chiSoDien == null)
                    warnings.Add($"Phòng <b>{phong.MaPhong}</b>: chưa có chỉ số điện tháng {thang}/{nam}.");
                if (chiSoNuoc == null)
                    warnings.Add($"Phòng <b>{phong.MaPhong}</b>: chưa có chỉ số nước tháng {thang}/{nam}.");

                foreach (var sv in svTrongPhong)
                {
                    if (hdDaTon.Contains(sv.Id)) continue;

                    var hd = new HoaDon
                    {
                        MaHoaDon = $"HD{thang:D2}{nam}{sv.MaSinhVien}",
                        SinhVienId = sv.Id,
                        PhongId = phong.Id,
                        Thang = thang,
                        Nam = nam,
                        TienPhong = phong.GiaPhong,
                        TienDien = tienDienMoiNguoi,
                        TienNuoc = tienNuocMoiNguoi,
                        PhiKhac = 0,
                        NgayLap = DateTime.Now,
                        HanThanhToan = new DateTime(nam, thang, 1).AddMonths(1).AddDays(15),
                        TrangThai = "Chưa thanh toán",
                        GhiChu = $"Điện: {tongTienDien:N0}đ/{soNguoi}SV | Nước: {tongTienNuoc:N0}đ/{soNguoi}SV"
                    };
                    _context.HoaDons.Add(hd);
                    count++;
                }
            }

            await _context.SaveChangesAsync();

            if (count > 0)
                TempData["Success"] = $"Đã lập <b>{count}</b> hóa đơn tự động cho tháng {thang}/{nam}.";
            else
                TempData["Warning"] = $"Không tạo thêm hóa đơn mới — tất cả sinh viên đã có hóa đơn tháng {thang}/{nam}.";

            if (warnings.Any())
                TempData["Warning"] = (TempData["Warning"] ?? "") + "<br>" + string.Join("<br>", warnings);

            return RedirectToAction(nameof(Index));
        }
    }
}
