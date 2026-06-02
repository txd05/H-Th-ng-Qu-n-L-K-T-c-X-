using KyTucXaManagement.Data;
using KyTucXaManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class DienNuocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DienNuocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? thang, int? nam)
        {
            thang ??= DateTime.Now.Month;
            nam ??= DateTime.Now.Year;

            var chiSoDiens = await _context.ChiSoDiens
                .Include(c => c.Phong)
                .Where(c => c.Thang == thang && c.Nam == nam)
                .ToListAsync();

            var chiSoNuocs = await _context.ChiSoNuocs
                .Include(c => c.Phong)
                .Where(c => c.Thang == thang && c.Nam == nam)
                .ToListAsync();

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.ChiSoDiens = chiSoDiens;
            ViewBag.ChiSoNuocs = chiSoNuocs;
            return View();
        }

        // Chỉ số điện
        public IActionResult CreateDien()
        {
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(new ChiSoDien { Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, NgayGhi = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDien(ChiSoDien model)
        {
            if (model.ChiSoCuoi < model.ChiSoDau)
                ModelState.AddModelError("ChiSoCuoi", "Chỉ số cuối không thể nhỏ hơn chỉ số đầu.");

            if (ModelState.IsValid)
            {
                _context.ChiSoDiens.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ghi chỉ số điện thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(model);
        }

        // Chỉ số nước
        public IActionResult CreateNuoc()
        {
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(new ChiSoNuoc { Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, NgayGhi = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNuoc(ChiSoNuoc model)
        {
            if (model.ChiSoCuoi < model.ChiSoDau)
                ModelState.AddModelError("ChiSoCuoi", "Chỉ số cuối không thể nhỏ hơn chỉ số đầu.");

            if (ModelState.IsValid)
            {
                _context.ChiSoNuocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ghi chỉ số nước thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Phongs = new SelectList(_context.Phongs.OrderBy(p => p.MaPhong), "Id", "MaPhong");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDien(int id)
        {
            var item = await _context.ChiSoDiens.FindAsync(id);
            if (item != null) { _context.ChiSoDiens.Remove(item); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNuoc(int id)
        {
            var item = await _context.ChiSoNuocs.FindAsync(id);
            if (item != null) { _context.ChiSoNuocs.Remove(item); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa.";
            return RedirectToAction(nameof(Index));
        }
    }
}
