using System;
using System.Web.Mvc;
using QLNS.Services;
using System.Collections.Generic;

namespace QLNS.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo Service kết nối CSDL
        private readonly DatabaseService _db = new DatabaseService();

        #region Helpers (Hàm phụ trợ)

        // 1. Hàm kiểm tra quyền Admin (tránh lặp lại code)
        private bool IsAdmin()
        {
            // Kiểm tra session User có tồn tại và Role có phải Admin không
            return Session["User"] != null && Session["Role"]?.ToString() == "Admin";
        }

        // 2. Hàm tạo danh sách Role cho DropdownList
        private SelectList GetRoleList(string selected = null)
        {
            // Danh sách các quyền trong hệ thống
            var roles = new[] { "Admin", "NhanVien", "KhachHang" };
            return new SelectList(roles, selected);
        }

        #endregion

        #region Actions (Xử lý chính)

        // 1. TRANG CHỦ: HIỂN THỊ DANH SÁCH USER
        public ActionResult Index()
        {
            // Chặn truy cập nếu không phải Admin
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Lấy danh sách user từ DatabaseService (trả về DataTable hoặc List)
            ViewBag.Users = _db.GetUsers();
            return View();
        }

        // 2.1. TẠO MỚI (GET): Hiển thị Form nhập liệu
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Truyền danh sách quyền sang View để hiện Dropdown
            ViewBag.Roles = GetRoleList();
            return RedirectToAction("Index");
        }

        // 2.2. TẠO MỚI (POST): Nhận dữ liệu từ Form và Lưu
        [HttpPost]
        public ActionResult Create(string username, string password, string HoTen, string role)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Gọi hàm tạo User (Lưu ý: tham số HoTen đã khớp với View)
            string result = _db.CreateUser(username, password, HoTen, role);

            if (result == "OK")
            {
                TempData["Msg"] = "Đã thêm nhân viên thành công!";
            }
            else
            {
                // Nếu lỗi (VD: Trùng tên), gán vào TempData để hiện thông báo đỏ
                TempData["Error"] = result;
            }

            // QUAN TRỌNG: Quay về trang Index để hiện thông báo
            // KHÔNG ĐƯỢC return View() ở đây vì sẽ gây lỗi "View not found"
            return RedirectToAction("Index");
        }

        // Xử lý trường hợp ai đó cố tình truy cập /Home/Create bằng trình duyệt



        // 3.1. SỬA TÀI KHOẢN (GET): Hiển thị thông tin cũ
        public ActionResult Edit(string id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Lấy thông tin user dựa vào id (username)
            var user = _db.GetUserByUsername(id);
            if (user == null) return HttpNotFound();

            // Truyền danh sách quyền vào ViewBag để hiện Dropdown
            ViewBag.Roles = new SelectList(new[] { "Admin", "NhanVien", "KhachHang" }, user.Role);

            return View(user); // Trả về view Edit.cshtml vừa tạo
        }

        // 2. EDIT (POST): Lưu dữ liệu
        [HttpPost]
        public ActionResult Edit(string username, string HoTen, string role)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Gọi hàm Update trong Service
            bool result = _db.UpdateUser(username, HoTen, role);

            if (result)
            {
                TempData["Msg"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Cập nhật thất bại!";
                // Load lại thông tin cũ để không bị lỗi trang
                return RedirectToAction("Edit", new { id = username });
            }
        }

        // 3.2. SỬA TÀI KHOẢN (POST): Lưu thay đổi
        
        // 4. XÓA TÀI KHOẢN
        [HttpPost]
        public ActionResult DeleteUser(string username, bool rollback = false)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // Không cho phép tự xóa chính mình
            if (Session["User"].ToString() == username)
            {
                TempData["Error"] = "Bạn không thể xóa tài khoản đang đăng nhập!";
                return RedirectToAction("Index");
            }

            // Gọi Service xóa (có hỗ trợ chế độ rollback để test transaction)
            string resultMsg = _db.DeleteUserSafe(username, rollback);

            // Kiểm tra kết quả trả về để hiển thị màu thông báo phù hợp
            if (resultMsg.Contains("thành công"))
                TempData["Msg"] = resultMsg;
            else
                TempData["Error"] = resultMsg;

            return RedirectToAction("Index");
        }

        #endregion

        #region Các trang phụ

        // Trang thông báo không có quyền truy cập
        public ActionResult NoPermission()
        {
            return View();
        }

        // Trang chạy con trỏ (Demo Cursor SQL)
        public ActionResult RunCursor()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        #endregion
    }
}