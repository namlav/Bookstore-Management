using System;
using System.Web.Mvc;
using System.Web.Security;
using QLNS.Services;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace QLNS.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseService _db = new DatabaseService();

        // GET: Login
        public ActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì chuyển hướng luôn
            if (Session["User"] != null)
            {
                return RedirectBasedOnRole(Session["Role"]?.ToString());
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // 1. Lấy thông tin thiết bị và IP
            string ip = GetActualIp();
            string userAgent = Request.UserAgent ?? "Unknown";
            string dev = $"OS: {Request.Browser.Platform} ({userAgent.Substring(0, Math.Min(20, userAgent.Length))}...)";
            string brow = $"{Request.Browser.Browser} v{Request.Browser.Version}";

            // 2. Gọi hàm Login trong DatabaseService
            if (_db.Login(username, password, ip, dev, brow, out string msg))
            {
                // 3. Đăng nhập thành công -> Lấy thông tin chi tiết User
                var userObj = _db.GetUserByUsername(username);

                if (userObj != null)
                {
                    // Lưu Session quan trọng
                    Session["User"] = userObj.Username;
                    Session["Role"] = userObj.Role;
                    Session["FullName"] = userObj.FullName; // Lưu họ tên để hiển thị

                    // Tạo Cookie Auth (giữ đăng nhập)
                    FormsAuthentication.SetAuthCookie(username, false);

                    // 4. Chuyển hướng dựa trên Role
                    return RedirectBasedOnRole(userObj.Role);
                }
            }

            // Đăng nhập thất bại
            ViewBag.Error = msg;
            ViewBag.Username = username; // Giữ lại username để đỡ phải nhập lại
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        // --- CÁC CHỨC NĂNG QUÊN MẬT KHẨU ---

        public ActionResult ForgotPassword() => View();

        [HttpPost]
        public ActionResult ForgotPassword(string username)
        {
            string token = _db.RequestResetToken(username);
            if (!string.IsNullOrEmpty(token))
            {
                // Demo: Hiển thị Token ra màn hình (Thực tế nên gửi Email)
                TempData["Token"] = token;
                TempData["User"] = username;
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "Tài khoản không tồn tại!";
            return View();
        }

        public ActionResult ResetPassword() => View();

        [HttpPost]
        public ActionResult ResetPassword(string username, string token, string newpass)
        {
            string res = _db.ResetPassword(username, token, newpass);
            if (res == "Success")
            {
                TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            ViewBag.Error = res;
            return View();
        }

        // --- HÀM HỖ TRỢ (PRIVATE) ---

        // Hàm điều hướng tập trung
        private ActionResult RedirectBasedOnRole(string role)
        {
            switch (role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Home");
                case "NhanVien":
                    // Nhân viên vào trang Quét Cursor hoặc Dashboard riêng
                    return RedirectToAction("RunCursor", "Home");
                case "KhachHang":
                    return RedirectToAction("Index", "Customer");
                default:
                    // Nếu role lạ (chưa phân quyền), đẩy về Login
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    return RedirectToAction("Login");
            }
        }

        // Hàm lấy IP thực tế
        private string GetActualIp()
        {
            string ip = Request.UserHostAddress;
            if (ip == "::1" || ip == "127.0.0.1")
            {
                try
                {
                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus == OperationalStatus.Up &&
                            !ni.Description.ToLower().Contains("vmware") &&
                            !ni.Description.ToLower().Contains("virtual") &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            foreach (var addr in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                                    return addr.Address.ToString();
                            }
                        }
                    }
                }
                catch { return "127.0.0.1"; }
            }
            return ip;
        }
    }
}