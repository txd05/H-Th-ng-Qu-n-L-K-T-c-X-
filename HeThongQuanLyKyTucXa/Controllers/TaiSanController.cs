using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class TaiSanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiSanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tinhTrang)
        {
            var query = _context.TaiSans.Include(t => t.Phong).AsQueryable();
            if (!string.IsNullOrEmpty(tinhTrang))
                query = query.Where(t => t.TinhTrang == tinhTrang);

            ViewBag.TinhTrang = tinhTrang;
            return View(await query.OrderBy(t => t.Phong!.MaPhong).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(new TaiSan { NgayNhap = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiSan model)
        {
            if (ModelState.IsValid)
            {
                _context.TaiSans.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm tài sản thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ts = await _context.TaiSans.FindAsync(id);
            if (ts == null) return NotFound();
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong", ts.PhongId);
            return View(ts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiSan model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật tài sản thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong", model.PhongId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ts = await _context.TaiSans.FindAsync(id);
            if (ts != null) { _context.TaiSans.Remove(ts); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa tài sản.";
            return RedirectToAction(nameof(Index));
        }

        // Quản lý sự cố
        public async Task<IActionResult> SuCo(string? trangThai)
        {
            var query = _context.SuCoTaiSans
                .Include(s => s.TaiSan).ThenInclude(t => t!.Phong)
                .Include(s => s.SinhVienBao)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(s => s.TrangThai == trangThai);

            ViewBag.TrangThai = trangThai;
            return View(await query.OrderByDescending(s => s.NgayBao).ToListAsync());
        }

        public IActionResult CreateSuCo()
        {
            ViewBag.TaiSans = new SelectList(_context.TaiSans.Include(t => t.Phong).Select(t => new { t.Id, Ten = t.TenTaiSan + " - " + t.Phong!.MaPhong }), "Id", "Ten");
            return View(new SuCoTaiSan { NgayBao = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSuCo(SuCoTaiSan model)
        {
            if (ModelState.IsValid)
            {
                _context.SuCoTaiSans.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Báo sự cố thành công!";
                return RedirectToAction(nameof(SuCo));
            }
            ViewBag.TaiSans = new SelectList(_context.TaiSans.Include(t => t.Phong).Select(t => new { t.Id, Ten = t.TenTaiSan + " - " + t.Phong!.MaPhong }), "Id", "Ten");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLySuCo(int id, string nguoiSua, double chiPhiSua, string ghiChu)
        {
            var sc = await _context.SuCoTaiSans.Include(s => s.TaiSan).FirstOrDefaultAsync(s => s.Id == id);
            if (sc == null) return NotFound();

            sc.TrangThai = "Đã xử lý";
            sc.NgaySua = DateTime.Now;
            sc.NguoiSua = nguoiSua;
            sc.ChiPhiSua = chiPhiSua;
            sc.GhiChu = ghiChu;

            if (sc.TaiSan != null)
                sc.TaiSan.TinhTrang = "Bình thường";

            await _context.SaveChangesAsync();
            TempData["Success"] = "Xử lý sự cố thành công!";
            return RedirectToAction(nameof(SuCo));
        }
    }
}
