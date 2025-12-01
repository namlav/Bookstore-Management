import customtkinter as ctk
from tkinter import ttk, messagebox
from DATABASE import Database


# --- CLASS QUẢN LÝ NHÂN VIÊN (ĐÃ CÓ TÌM KIẾM) ---
class KhungNhanVien(ctk.CTkFrame):
    def __init__(self, parent, db):
        super().__init__(parent)
        self.db = db

        # 1. Tiêu đề
        self.lbl_title = ctk.CTkLabel(
            self, text="QUẢN LÝ HỒ SƠ NHÂN VIÊN", font=("Arial", 24, "bold")
        )
        self.lbl_title.pack(pady=15)

        # 2. Form Nhập liệu
        self.create_input_form()

        # 3. Thanh công cụ (Nút chức năng)
        self.create_buttons()

        # 4. Thanh Tìm kiếm
        self.create_search_bar()

        # 5. Bảng dữ liệu
        self.setup_treeview()
        self.load_data()

    def create_input_form(self):
        self.input_frame = ctk.CTkFrame(self)
        self.input_frame.pack(pady=5, padx=20, fill="x")

        # Hàng 1
        self.entry_ma = ctk.CTkEntry(self.input_frame, placeholder_text="Mã Nhân Viên")
        self.entry_ma.grid(row=0, column=0, padx=10, pady=10, sticky="ew")

        self.entry_ten = ctk.CTkEntry(
            self.input_frame, placeholder_text="Họ Tên Nhân Viên"
        )
        self.entry_ten.grid(row=0, column=1, padx=10, pady=10, sticky="ew")

        self.cbo_chucvu = ctk.CTkComboBox(
            self.input_frame, values=["Nhân viên", "Thu ngân", "Quản lý"]
        )
        self.cbo_chucvu.grid(row=0, column=2, padx=10, pady=10, sticky="ew")
        self.cbo_chucvu.set("Nhân viên")

        # Hàng 2
        self.entry_sdt = ctk.CTkEntry(
            self.input_frame, placeholder_text="Số Điện Thoại"
        )
        self.entry_sdt.grid(row=1, column=0, padx=10, pady=10, sticky="ew")

        self.entry_email = ctk.CTkEntry(self.input_frame, placeholder_text="Email")
        self.entry_email.grid(row=1, column=1, padx=10, pady=10, sticky="ew")

    def create_buttons(self):
        self.btn_frame = ctk.CTkFrame(self, fg_color="transparent")
        self.btn_frame.pack(pady=10)

        # Nút Thêm - Sửa - Xóa - Làm mới
        buttons = [
            ("Thêm NV", self.add_employee, "#2CC985"),
            ("Cập Nhật", self.update_employee, "#FFA500"),
            ("Xóa NV", self.delete_employee, "#FF5555"),
            ("Reset Form", self.clear_form, "#3B8ED0"),
        ]

        for i, (text, cmd, color) in enumerate(buttons):
            btn = ctk.CTkButton(
                self.btn_frame, text=text, command=cmd, fg_color=color, width=100
            )
            btn.grid(row=0, column=i, padx=5)

    def create_search_bar(self):
        # Frame chứa thanh tìm kiếm
        self.search_frame = ctk.CTkFrame(self, fg_color="transparent")
        self.search_frame.pack(pady=(10, 0), padx=20, fill="x")

        self.lbl_search = ctk.CTkLabel(
            self.search_frame, text="Tìm kiếm:", font=("Arial", 14)
        )
        self.lbl_search.pack(side="left", padx=(0, 10))

        self.entry_search = ctk.CTkEntry(
            self.search_frame,
            placeholder_text="Nhập tên hoặc mã nhân viên...",
            width=300,
        )
        self.entry_search.pack(side="left", padx=5)

        self.btn_search = ctk.CTkButton(
            self.search_frame, text="Tìm", command=self.load_data, width=80
        )
        self.btn_search.pack(side="left", padx=5)

        self.btn_show_all = ctk.CTkButton(
            self.search_frame,
            text="Hiện tất cả",
            command=lambda: [self.entry_search.delete(0, "end"), self.load_data()],
            fg_color="gray",
            width=80,
        )
        self.btn_show_all.pack(side="left", padx=5)

    def setup_treeview(self):
        style = ttk.Style()
        style.theme_use("clam")  # Chọn theme 'clam' để dễ chỉnh màu

        # Cấu hình khung cho bảng
        style.configure(
            "Treeview",
            background="#2b2b2b",
            foreground="white",
            fieldbackground="#2b2b2b",
            rowheight=30,
        )
        # Hiệu ứng khi rê chuột vào
        style.map("Treeview", background=[("selected", "#1f6aa5")])  # Màu khi chọn
        # Cấu hình cho tiêu đề
        style.configure(
            "Treeview.Heading",
            background="#1f538d",
            foreground="white",
            font=("Arial", 12, "bold"),
            borderwidth=1,  # độ dày viền
            relief="raised",  # kiểu viền (hiệu ứng nổi)
        )

        columns = ("MaNV", "TenNV", "ChucVu", "DienThoai", "Email")
        self.tree = ttk.Treeview(self, columns=columns, show="headings")

        titles = ["Mã Nhân Viên", "Họ Tên", "Chức Vụ", "Điện Thoại", "Email"]
        widths = [80, 200, 100, 120, 200]

        for col, title, w in zip(columns, titles, widths):
            self.tree.heading(col, text=title)
            self.tree.column(col, width=w, anchor="center")

        # Định nghĩa màu sắc cho các dòng chẵn/lẻ (Tạo hiệu ứng kẻ lưới ngang)
        self.tree.tag_configure("oddrow", background="#2b2b2b")  # Dòng lẻ: Màu nền gốc
        self.tree.tag_configure(
            "evenrow", background="#3a3a3a"
        )  # Dòng chẵn: Màu sáng hơn chút

        self.tree.pack(fill="both", expand=True, padx=20, pady=20)
        self.tree.bind("<<TreeviewSelect>>", self.on_tree_select)

    def load_data(self):
        search_key = self.entry_search.get().strip()

        # Xóa dữ liệu cũ
        for item in self.tree.get_children():
            self.tree.delete(item)

        if search_key:
            # Tìm theo Mã hoặc Tên (sử dụng LIKE)
            sql = "SELECT MaNV, TenNV, ChucVu, DienThoai, Email FROM NhanVien WHERE MaNV LIKE ? OR TenNV LIKE ?"
            params = (f"%{search_key}%", f"%{search_key}%")
        else:
            # Nếu không tìm kiếm thì lấy hết
            sql = "SELECT MaNV, TenNV, ChucVu, DienThoai, Email FROM NhanVien"
            params = ()

        rows = self.db.fetch_data(sql, params)

        # Màu sắc xen kẽ cho các dòng
        for i, row in enumerate(rows):
            tag = "evenrow" if i % 2 == 0 else "oddrow"
            self.tree.insert("", "end", values=list(row), tags=(tag,))

    def on_tree_select(self, event):
        selected_item = self.tree.selection()
        if selected_item:
            values = self.tree.item(selected_item[0])["values"]
            self.clear_form()
            self.entry_ma.insert(0, values[0])
            self.entry_ten.insert(0, values[1])
            self.cbo_chucvu.set(values[2])
            self.entry_sdt.insert(0, values[3])
            self.entry_email.insert(0, values[4])

    def clear_form(self):
        self.entry_ma.delete(0, "end")
        self.entry_ten.delete(0, "end")
        self.entry_sdt.delete(0, "end")
        self.entry_email.delete(0, "end")
        self.cbo_chucvu.set("Nhân viên")

    def add_employee(self):
        ten = self.entry_ten.get()
        chucvu = self.cbo_chucvu.get()
        sdt = self.entry_sdt.get()
        email = self.entry_email.get()

        if not ten:
            messagebox.showerror("Lỗi", "Vui lòng nhập Tên nhân viên!")
            return

        # Stored Procedure này nhận 4 tham số: Ten, ChucVu, SDT, Email
        sql = "EXEC sp_ThemNhanVienMoi ?, ?, ?, ?"

        success, msg = self.db.execute_query(sql, (ten, chucvu, sdt, email))

        if success:
            messagebox.showinfo(
                "Thành công",
                f"Đã thêm nhân viên {ten} và tạo tài khoản đăng nhập cho nhân viên này!",
            )
            self.load_data()  # Reload lại bảng
            self.clear_form()
        else:
            messagebox.showerror("Lỗi CSDL", msg)

    def update_employee(self):
        ma = self.entry_ma.get()
        ten = self.entry_ten.get()
        chucvu = self.cbo_chucvu.get()
        sdt = self.entry_sdt.get()
        email = self.entry_email.get()

        if not ma:
            messagebox.showerror("Lỗi", "Chọn nhân viên để cập nhật!")
            return

        sql = "UPDATE NhanVien SET TenNV=?, ChucVu=?, DienThoai=?, Email=? WHERE MaNV=?"
        success, msg = self.db.execute_query(sql, (ten, chucvu, sdt, email, ma))

        if success:
            messagebox.showinfo("Thành công", "Cập nhật dữ liệu thành công!")
            self.load_data()
        else:
            messagebox.showerror("Lỗi CSDL", msg)

    def delete_employee(self):
        # Lấy Mã NV đang chọn
        selected_item = self.tree.selection()
        if not selected_item:
            messagebox.showerror("Lỗi", "Vui lòng chọn nhân viên cần xóa!")
            return

        # Lấy giá trị cột đầu tiên (MaNV) từ dòng được chọn
        ma = self.tree.item(selected_item[0])["values"][0]

        ten = self.entry_ten.get()
        confirm = messagebox.askyesno(
            "Cảnh báo",
            f"Chắc chắn muốn xóa nhân viên {ma} có tên {ten} không?\nHành động này không thể hoàn tác.",
        )
        if confirm:
            sql = "EXEC sp_XoaNhanVien ?"
            success, msg = self.db.execute_query(sql, (ma,))

            if success:
                messagebox.showinfo(
                    "Thành công",
                    f"Đã xóa nhân viên {ma} và tài khoản đăng nhập tương ứng!",
                )
                self.load_data()
                self.clear_form()
            else:
                messagebox.showerror("Không thể xóa", msg)
