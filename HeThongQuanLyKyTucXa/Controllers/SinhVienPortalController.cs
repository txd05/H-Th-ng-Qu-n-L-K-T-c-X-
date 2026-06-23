using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "SinhVien")]
    public class SinhVienPortalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienPortalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SinhVienPortal/Index — Trang profile sinh viên
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sinhVien = await _context.SinhViens
                .Include(s => s.Phong)
                .Include(s => s.TheKTX)
                .Include(s => s.HoaDons.OrderByDescending(h => h.NgayLap).Take(5))
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (sinhVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên liên kết với tài khoản này.";
                return View();
            }

            return View(sinhVien);
        }

        // GET: SinhVienPortal/HoaDon — Danh sách hóa đơn của sinh viên
        public async Task<IActionResult> HoaDon()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (sinhVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên.";
                return View(new List<HoaDon>());
            }

            var hoaDons = await _context.HoaDons
                .Include(h => h.Phong)
                .Where(h => h.SinhVienId == sinhVien.Id)
                .OrderByDescending(h => h.Nam)
                .ThenByDescending(h => h.Thang)
                .ToListAsync();

            ViewBag.SinhVien = sinhVien;
            return View(hoaDons);
        }

        // POST: SinhVienPortal/ThanhToanHoaDon — Sinh viên xác nhận đã chuyển khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToanHoaDon(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sinhVien == null) return Forbid();

            var hd = await _context.HoaDons
                .FirstOrDefaultAsync(h => h.Id == id && h.SinhVienId == sinhVien.Id);

            if (hd == null) return NotFound();

            if (hd.TrangThai == "Đã thanh toán")
            {
                TempData["Warning"] = "Hóa đơn này đã được thanh toán rồi.";
                return RedirectToAction(nameof(HoaDon));
            }

            hd.TrangThai = "Đã thanh toán";
            hd.NgayThanhToan = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Thanh toán hóa đơn <b>{hd.MaHoaDon}</b> thành công!";
            return RedirectToAction(nameof(HoaDon));
        }

        // GET: SinhVienPortal/DonYeuCau — Xem và gửi đơn yêu cầu
        public async Task<IActionResult> DonYeuCau()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sinhVien = await _context.SinhViens
                .Include(s => s.Phong)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (sinhVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên.";
                return View(new List<DonYeuCau>());
            }

            var donYeuCaus = await _context.DonYeuCaus
                .Include(d => d.Phong)
                .Where(d => d.SinhVienId == sinhVien.Id)
                .OrderByDescending(d => d.NgayNop)
                .ToListAsync();

            ViewBag.SinhVien = sinhVien;
            return View(donYeuCaus);
        }

        // POST: SinhVienPortal/DonYeuCau — Gửi đơn yêu cầu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DonYeuCau(string loaiDon, string noiDung)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sinhVien = await _context.SinhViens
                .Include(s => s.Phong)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (sinhVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction(nameof(DonYeuCau));
            }

            if (string.IsNullOrWhiteSpace(loaiDon) || string.IsNullOrWhiteSpace(noiDung))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin đơn yêu cầu.";
                return RedirectToAction(nameof(DonYeuCau));
            }

            var donYeuCau = new DonYeuCau
            {
                MaDon = $"DYC{DateTime.Now:yyyyMMddHHmmss}",
                SinhVienId = sinhVien.Id,
                PhongId = sinhVien.PhongId,
                LoaiDon = loaiDon,
                NoiDung = noiDung,
                NgayNop = DateTime.Now,
                TrangThai = "Chờ duyệt"
            };

            _context.DonYeuCaus.Add(donYeuCau);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gửi đơn yêu cầu thành công!";
            return RedirectToAction(nameof(DonYeuCau));
        }
    }
}
