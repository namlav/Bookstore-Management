using QL_Sach_Nhom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_Sach_Nhom.Controllers
{
    public class LoginController : Controller
    {
        private QLNHASACHEntities db = new QLNHASACHEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật chống giả mạo request
        public ActionResult Login(TaiKhoan user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trong Database
                // Lưu ý: Thực tế password nên được mã hóa (MD5, SHA256, Bcrypt) trước khi so sánh
                var data = db.TaiKhoans.Where(s => s.TenDangNhap.Equals(user.TenDangNhap) && s.MatKhau.Equals(user.MatKhau)).FirstOrDefault();

                if (data != null)
                {
                    // Đăng nhập thành công
                    // Lưu thông tin vào Session
                    Session["UserID"] = data.MaNV;
                    Session["UserName"] = data.TenDangNhap;

                    // Chuyển hướng về trang chủ hoặc trang quản trị
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Đăng nhập thất bại
                    ViewBag.error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    return View();
                }
            }
            return View();
        }

        // Logout: Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa session
            return RedirectToAction("Login");
        }
    }
}