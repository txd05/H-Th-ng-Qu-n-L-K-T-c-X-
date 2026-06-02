using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KyTucXaManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin") || User.IsInRole("NhanVien"))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole("SinhVien"))
                    return RedirectToAction("Index", "SinhVienPortal");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
