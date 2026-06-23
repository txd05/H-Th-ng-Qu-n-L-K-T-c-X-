using KyTucXaManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tongSinhVien = await _context.SinhViens.CountAsync(s => s.TrangThai == "Đang nội trú");
            var tongPhong = await _context.Phongs.CountAsync();
            var phongConCho = await _context.Phongs.CountAsync(p => p.TrangThai == "Còn chỗ");
            var hoaDonChuaThanhToan = await _context.HoaDons.CountAsync(h => h.TrangThai == "Chưa thanh toán");
            var suCoChoXuLy = await _context.SuCoTaiSans.CountAsync(s => s.TrangThai == "Chờ xử lý");
            var donChoXetDuyet = await _context.DonYeuCaus.CountAsync(d => d.TrangThai == "Chờ duyệt");

            var doanhThuThang = await _context.HoaDons
                .Where(h => h.TrangThai == "Đã thanh toán"
                    && h.Thang == DateTime.Now.Month
                    && h.Nam == DateTime.Now.Year)
                .SumAsync(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac);

            ViewBag.TongSinhVien = tongSinhVien;
            ViewBag.TongPhong = tongPhong;
            ViewBag.PhongConCho = phongConCho;
            ViewBag.HoaDonChuaThanhToan = hoaDonChuaThanhToan;
            ViewBag.SuCoChoXuLy = suCoChoXuLy;
            ViewBag.DonChoXetDuyet = donChoXetDuyet;
            ViewBag.DoanhThuThang = doanhThuThang;

            // Thống kê sinh viên theo trạng thái
            var thongKeSinhVien = await _context.SinhViens
                .GroupBy(s => s.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();
            ViewBag.ThongKeSinhVien = thongKeSinhVien;

            // Hóa đơn quá hạn
            var hoaDonQuaHan = await _context.HoaDons
                .Where(h => h.TrangThai == "Chưa thanh toán" && h.HanThanhToan < DateTime.Now)
                .CountAsync();
            ViewBag.HoaDonQuaHan = hoaDonQuaHan;

            // Danh sách sinh viên gần đây
            var sinhVienGanDay = await _context.SinhViens
                .Include(s => s.Phong)
                .OrderByDescending(s => s.NgayVaoKTX)
                .Take(5)
                .ToListAsync();
            ViewBag.SinhVienGanDay = sinhVienGanDay;

            return View();
        }

        // Danh sách đơn yêu cầu
        public async Task<IActionResult> DonYeuCau(string? trangThai)
        {
            var query = _context.DonYeuCaus
                .Include(d => d.SinhVien)
                .Include(d => d.Phong)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            var dons = await query.OrderByDescending(d => d.NgayNop).ToListAsync();

            ViewBag.TrangThaiFilter = trangThai;
            ViewBag.SoChoXetDuyet = await _context.DonYeuCaus.CountAsync(d => d.TrangThai == "Chờ duyệt");
            return View(dons);
        }

        // POST: Duyệt đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetDon(int id, string hanhDong, string? lyDoTuChoi)
        {
            var don = await _context.DonYeuCaus.FindAsync(id);
            if (don == null) return NotFound();

            don.NgayDuyet = DateTime.Now;
            don.NguoiDuyet = User.Identity?.Name;

            if (hanhDong == "duyet")
            {
                don.TrangThai = "Đã duyệt";
                TempData["Success"] = $"Đã duyệt đơn <b>{don.MaDon}</b> thành công.";
            }
            else
            {
                don.TrangThai = "Từ chối";
                don.LyDoTuChoi = lyDoTuChoi;
                TempData["Warning"] = $"Đã từ chối đơn <b>{don.MaDon}</b>.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DonYeuCau));
        }

        // Thống kê toàn bộ sinh viên
        public async Task<IActionResult> ThongKe()
        {
            var sinhViens = await _context.SinhViens
                .Include(s => s.Phong)
                .Include(s => s.TheKTX)
                .Include(s => s.HoaDons)
                .OrderBy(s => s.MaSinhVien)
                .ToListAsync();

            // Thống kê tổng
            ViewBag.TongSinhVien = sinhViens.Count;
            ViewBag.DangNoiTru = sinhViens.Count(s => s.TrangThai == "Đang nội trú");
            ViewBag.DaRoi = sinhViens.Count(s => s.TrangThai == "Đã rời KTX");
            ViewBag.TamKhoa = sinhViens.Count(s => s.TrangThai == "Tạm khóa");

            // Theo giới tính
            ViewBag.SinhVienNam = sinhViens.Count(s => s.GioiTinh == "Nam");
            ViewBag.SinhVienNu = sinhViens.Count(s => s.GioiTinh == "Nữ");

            // Tổng nợ chưa thanh toán
            var tongNo = await _context.HoaDons
                .Where(h => h.TrangThai == "Chưa thanh toán")
                .SumAsync(h => h.TienPhong + h.TienDien + h.TienNuoc + h.PhiKhac);
            ViewBag.TongNo = tongNo;

            return View(sinhViens);
        }
    }
}
