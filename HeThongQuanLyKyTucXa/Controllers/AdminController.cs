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
