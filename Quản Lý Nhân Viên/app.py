import customtkinter as ctk
from DATABASE import Database
from QLNHANVIEN import KhungNhanVien


# --- CẤU HÌNH GIAO DIỆN ---
ctk.set_appearance_mode("Dark")  # Chế độ tối (Sang trọng)
ctk.set_default_color_theme("dark-blue")  # Màu chủ đạo xanh đậm


class App(ctk.CTk):
    def __init__(self):
        super().__init__()
        self.db = Database()

        # Setup Window
        self.title("QUẢN LÝ NHÂN SỰ")
        self.geometry("1000x700")

        # Grid Layout
        self.grid_columnconfigure(1, weight=1)
        self.grid_rowconfigure(0, weight=1)

        # 1. SIDEBAR
        self.setup_sidebar()

        # 2. MAIN FRAME (Chỉ quản lý nhân viên)
        self.frame_nhanvien = KhungNhanVien(self, self.db)
        self.frame_nhanvien.grid(row=0, column=1, sticky="nsew", padx=20, pady=20)

    def setup_sidebar(self):
        self.sidebar_frame = ctk.CTkFrame(self, width=200, corner_radius=0)
        self.sidebar_frame.grid(row=0, column=0, sticky="nsew")
        self.sidebar_frame.grid_rowconfigure(4, weight=1)

        self.logo_label = ctk.CTkLabel(
            self.sidebar_frame,
            text="HR MANAGER\nSYSTEM",
            font=ctk.CTkFont(size=20, weight="bold"),
        )
        self.logo_label.grid(row=0, column=0, padx=20, pady=(20, 10))

        # Nút hiển thị trạng thái đang hoạt động
        self.btn_active = ctk.CTkButton(
            self.sidebar_frame,
            text="• Đang hoạt động",
            fg_color="transparent",
            text_color="#2CC985",
            state="disabled",
            font=("Arial", 13, "bold"),
        )
        self.btn_active.grid(row=1, column=0, padx=20, pady=10)


if __name__ == "__main__":
    app = App()
    app.mainloop()
