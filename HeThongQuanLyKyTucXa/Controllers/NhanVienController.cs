using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KyTucXaManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NhanVienController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public NhanVienController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: NhanVien/Index — Danh sách nhân viên
        public async Task<IActionResult> Index()
        {
            var nhanViens = await _userManager.GetUsersInRoleAsync("NhanVien");
            return View(nhanViens.ToList());
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email và mật khẩu không được để trống.");
                return View();
            }

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                ModelState.AddModelError("", "Email này đã được sử dụng.");
                return View();
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "NhanVien");
                TempData["Success"] = $"Tạo tài khoản nhân viên {email} thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        // POST: NhanVien/ToggleKhoa — Khóa/Mở khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleKhoa(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                // Đang bị khóa → mở khóa
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Đã mở khóa tài khoản {user.Email}.";
            }
            else
            {
                // Chưa bị khóa → khóa (khóa 100 năm)
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                TempData["Success"] = $"Đã khóa tài khoản {user.Email}.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: NhanVien/ResetPassword — Đặt lại mật khẩu về mặc định
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "NhanVien@123");

            if (result.Succeeded)
                TempData["Success"] = $"Đã reset mật khẩu {user.Email} về 'NhanVien@123'.";
            else
                TempData["Error"] = "Reset mật khẩu thất bại.";

            return RedirectToAction(nameof(Index));
        }

        // POST: NhanVien/Delete — Xóa tài khoản nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var email = user.Email;
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                TempData["Success"] = $"Đã xóa tài khoản {email}.";
            else
                TempData["Error"] = "Xóa tài khoản thất bại.";

            return RedirectToAction(nameof(Index));
        }
    }
}
