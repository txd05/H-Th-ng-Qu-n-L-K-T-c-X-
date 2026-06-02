using KyTucXaManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KyTucXaManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginInput, string password, bool rememberMe, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(loginInput) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập tài khoản và mật khẩu.");
                return View();
            }

            // Tìm email thực từ loginInput (có thể là MSSV hoặc email)
            string loginEmail = loginInput.Trim();

            // Nếu không chứa @ → có thể là MSSV → tìm email từ hồ sơ sinh viên
            if (!loginEmail.Contains('@'))
            {
                var sv = await _context.SinhViens
                    .FirstOrDefaultAsync(s => s.MaSinhVien == loginEmail);

                if (sv != null && !string.IsNullOrEmpty(sv.UserId))
                {
                    // Lấy email tài khoản từ UserId
                    var userBySv = await _userManager.FindByIdAsync(sv.UserId);
                    if (userBySv?.Email != null)
                        loginEmail = userBySv.Email;
                }
                else
                {
                    // Thử dạng MSSV@ktx.edu.vn
                    loginEmail = $"{loginInput.Trim().ToLower()}@ktx.edu.vn";
                }
            }

            // Đăng nhập bằng email đã resolve
            var result = await _signInManager.PasswordSignInAsync(loginEmail, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginEmail);
                var roles = await _userManager.GetRolesAsync(user!);

                if (roles.Contains("Admin") || roles.Contains("NhanVien"))
                    return RedirectToAction("Index", "Admin");

                if (roles.Contains("SinhVien"))
                    return RedirectToAction("Index", "SinhVienPortal");

                return RedirectToLocal(returnUrl) ?? RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ quản lý.");
            }
            else
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return null;
        }
    }
}
