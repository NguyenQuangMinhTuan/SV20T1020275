using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020275.Web;
using SV20T1020275.BusinessLayers;
using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Phải nhập tên và mật khẩu!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            //Đăng nhập thành công, tạo dữ liệu để lưu thông tin đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList(),
            };
            //Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (Request.Method == "POST")
            {
                var userAccount = UserAccountService.Authorize(userName, oldPassword);
                if (userAccount == null)
                {
                    ModelState.AddModelError("Error", "sai mật khẩu!");
                    return View();
                }
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ModelState.AddModelError("Error", "Vui lòng điền mật khẩu mới!");
                    return View();
                }
                bool result = UserAccountService.ChangePassword(userName, oldPassword, newPassword);
                if (result)
                    return RedirectToAction("Logout");
            }
            else
            {
                return View();
            }
            return RedirectToAction("Logout");
        }

        public IActionResult Info(int id = 0)
        {
            Employee? model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult AccessDenined() 
        {
            return View();
        }
    }
}
