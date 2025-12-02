CREATE DATABASE QLNHASACH_HQT
GO
USE QLNHASACH_HQT
GO
-- 1. TÁC GIẢ
CREATE TABLE TacGia (
    MaTG VARCHAR(10) PRIMARY KEY,
    TenTG NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    DienThoai VARCHAR(15) UNIQUE, 
    Email NVARCHAR(100) UNIQUE CHECK (Email LIKE '_%@_%._%') 
);

-- 2. THỂ LOẠI
CREATE TABLE TheLoai (
    MaTL VARCHAR(10) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL UNIQUE, 
    MoTa NVARCHAR(200)
);

-- 3. NHÀ XUẤT BẢN
CREATE TABLE NhaXuatBan (
    MaNXB VARCHAR(10) PRIMARY KEY,
    TenNXB NVARCHAR(200) NOT NULL UNIQUE, 
    DiaChi NVARCHAR(200),
    DienThoai VARCHAR(15) UNIQUE,
    Email NVARCHAR(100) UNIQUE CHECK (Email LIKE '_%@_%._%')
);

-- 4. SÁCH
CREATE TABLE Sach (
    MaSach VARCHAR(10) PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    MaTG VARCHAR(10) NOT NULL,
    MaTL VARCHAR(10) NOT NULL,
    MaNXB VARCHAR(10) NOT NULL,
    NamXuatBan INT CHECK (NamXuatBan >= 1900 AND NamXuatBan <= YEAR(GETDATE())),
    GiaBia DECIMAL(18,2) CHECK (GiaBia > 0),
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0),
    FOREIGN KEY (MaTG) REFERENCES TacGia(MaTG),
    FOREIGN KEY (MaTL) REFERENCES TheLoai(MaTL),
    FOREIGN KEY (MaNXB) REFERENCES NhaXuatBan(MaNXB),
    CONSTRAINT UQ_Sach UNIQUE (TenSach, MaTG, MaNXB) 
);

-- 5. NHÂN VIÊN
CREATE TABLE NhanVien (
    MaNV VARCHAR(10) PRIMARY KEY,
    TenNV NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) DEFAULT N'Nhân viên',
    DienThoai VARCHAR(15) UNIQUE,
    Email NVARCHAR(100) UNIQUE CHECK (Email LIKE '_%@_%._%')
);

-- 6. KHÁCH HÀNG
CREATE TABLE KhachHang (
    MaKH VARCHAR(10) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    Email NVARCHAR(100) UNIQUE CHECK (Email LIKE '_%@_%._%'),
    DiaChi NVARCHAR(200)
);

-- 7. PHIẾU NHẬP
CREATE TABLE PhieuNhap (
    SoPN VARCHAR(10) PRIMARY KEY,
    NgayNhap DATE NOT NULL CHECK (NgayNhap <= GETDATE()), 
    MaNV VARCHAR(10) NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

-- 8. CHI TIẾT PHIẾU NHẬP
CREATE TABLE ChiTietPhieuNhap (
    MaSach VARCHAR(10) NOT NULL,
    SoPN VARCHAR(10) NOT NULL,
    SoLuongNhap INT CHECK (SoLuongNhap > 0),
    GiaNhap DECIMAL(18,2) CHECK (GiaNhap > 0),
    PRIMARY KEY (MaSach, SoPN),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach),
    FOREIGN KEY (SoPN) REFERENCES PhieuNhap(SoPN)
);

-- 9. HÓA ĐƠN
CREATE TABLE HoaDon (
    SoHD VARCHAR(10) PRIMARY KEY,
    NgayLap DATE NOT NULL CHECK (NgayLap <= GETDATE()),
    MaNV VARCHAR(10) NOT NULL,
    MaKH VARCHAR(10) NOT NULL,
    TongTien DECIMAL(18,2) DEFAULT 0 CHECK (TongTien >= 0),
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- 10. CHI TIẾT HÓA ĐƠN
CREATE TABLE ChiTietHoaDon (
    MaSach VARCHAR(10) NOT NULL,
    SoHD VARCHAR(10) NOT NULL,
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) CHECK (DonGia > 0),
    GiamGia DECIMAL(5,2) DEFAULT 0 CHECK (GiamGia >= 0 AND GiamGia <= 100),
    PRIMARY KEY (MaSach, SoHD),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach),
    FOREIGN KEY (SoHD) REFERENCES HoaDon(SoHD)
);

-- 11. TÀI KHOẢN
CREATE TABLE TaiKhoan (
    TenDangNhap VARCHAR(50) PRIMARY KEY,         -- Username duy nhất
    MatKhau NVARCHAR(200) NOT NULL,              -- Có thể mã hoá sau
    VaiTro NVARCHAR(20) CHECK (VaiTro IN (N'Admin', N'NhanVien', N'KhachHang')),
    MaNV VARCHAR(10) NULL,                       -- Nếu là nhân viên
    MaKH VARCHAR(10) NULL,                       -- Nếu là khách hàng
    NgayTao DATE DEFAULT GETDATE(),              -- Ngày tạo tài khoản
    TrangThai BIT DEFAULT 1,                     -- 1 = hoạt động, 0 = khoá
    
    CONSTRAINT FK_TaiKhoan_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_TaiKhoan_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- 12. Lịch sử giá
CREATE TABLE LichSuGia (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    MaSach VARCHAR(10),
    GiaCu DECIMAL(18,2),
    GiaMoi DECIMAL(18,2),
    NgayThayDoi DATETIME DEFAULT GETDATE(),
    NguoiDung NVARCHAR(50), -- Lưu user thực hiện
    LyDo NVARCHAR(200)
);

--13. Doanh Thu
CREATE TABLE DoanhThuThang (
    Nam INT NOT NULL,                          -- Năm
    Thang INT NOT NULL CHECK (Thang BETWEEN 1 AND 12), -- Tháng
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0, -- Tổng doanh thu trong tháng
    PRIMARY KEY (Nam, Thang)
);
--Doanh thu tháng
SELECT 
    YEAR(HD.NgayLap) AS Nam,
    MONTH(HD.NgayLap) AS Thang,
    SUM(CTHD.SoLuong * CTHD.DonGia * (1 - CTHD.GiamGia/100)) AS TongTien
FROM HoaDon HD
JOIN ChiTietHoaDon CTHD ON HD.SoHD = CTHD.SoHD
GROUP BY YEAR(HD.NgayLap), MONTH(HD.NgayLap)
ORDER BY Nam, Thang;
--Doanh thu ngày
SELECT 
    HD.NgayLap AS NgayBan,
    SUM(CTHD.SoLuong * CTHD.DonGia * (1 - CTHD.GiamGia/100)) AS TongTien
FROM HoaDon HD
JOIN ChiTietHoaDon CTHD ON HD.SoHD = CTHD.SoHD
GROUP BY HD.NgayLap
ORDER BY HD.NgayLap;

-- 1. TÁC GIẢ
INSERT INTO TacGia VALUES
('TG01', N'Nguyễn Nhật Ánh', N'TP. Hồ Chí Minh', '0909000001', 'nna@gmail.com'),
('TG02', N'J.K. Rowling', N'Anh Quốc', '0909000002', 'jkrowling@hp.com'),
('TG03', N'Paulo Coelho', N'Brazil', '0909000003', 'paulo@writer.com'),
('TG04', N'Haruki Murakami', N'Nhật Bản', '0909000004', 'murakami@jp.jp'),
('TG05', N'Tô Hoài', N'Hà Nội', '0909000005', 'tohoai@gmail.com');

-- 2. THỂ LOẠI
INSERT INTO TheLoai VALUES
('TL01', N'Truyện thiếu nhi', N'Sách dành cho thiếu nhi'),
('TL02', N'Khoa học', N'Sách nghiên cứu khoa học'),
('TL03', N'Tiểu thuyết', N'Sách văn học, truyện dài'),
('TL04', N'Tâm lý - Kỹ năng', N'Phát triển bản thân'),
('TL05', N'Lịch sử', N'Sách lịch sử, khảo cứu');

-- 3. NHÀ XUẤT BẢN
INSERT INTO NhaXuatBan VALUES
('NXB01', N'NXB Trẻ', N'TP.HCM', '0911000001', 'nxbtresg@gmail.com'),
('NXB02', N'NXB Kim Đồng', N'Hà Nội', '0911000002', 'nxbkimdong@gmail.com'),
('NXB03', N'NXB Hội Nhà Văn', N'Hà Nội', '0911000003', 'nxbhoinhavan@gmail.com'),
('NXB04', N'NXB Văn Học', N'Hà Nội', '0911000004', 'nxbvanhoc@gmail.com'),
('NXB05', N'Bloomsbury', N'Anh Quốc', '0911000005', 'bloomsbury@uk.com');

-- 4. SÁCH
INSERT INTO Sach VALUES
('S01', N'Tôi thấy hoa vàng trên cỏ xanh', 'TG01', 'TL01', 'NXB01', 2010, 75000, 50),
('S02', N'Cho tôi xin một vé đi tuổi thơ', 'TG01', 'TL01', 'NXB02', 2007, 65000, 30),
('S03', N'Harry Potter và Hòn đá Phù thủy', 'TG02', 'TL03', 'NXB05', 1997, 120000, 100),
('S04', N'Nhà giả kim', 'TG03', 'TL03', 'NXB03', 1988, 90000, 40),
('S05', N'Rừng Na Uy', 'TG04', 'TL03', 'NXB04', 1987, 110000, 25),
('S06', N'Dế Mèn Phiêu Lưu Ký', 'TG05', 'TL01', 'NXB02', 1941, 55000, 60);

-- 5. NHÂN VIÊN
INSERT INTO NhanVien VALUES
('NV01', N'Nguyễn Văn An', N'Nhân viên', '0988000001', 'an.nv@nhasach.com'),
('NV02', N'Trần Thị Bình', N'Thu ngân', '0988000002', 'binh.tt@nhasach.com'),
('NV03', N'Lê Văn Cường', N'Quản lý', '0988000003', 'cuong.lv@nhasach.com'),
('NV04', N'Phạm Thị Dung', N'Nhân viên', '0988000004', 'dung.pt@nhasach.com'),
('NV05', N'Hoàng Văn Minh', N'Nhân viên', '0988000005', 'minh.hv@nhasach.com');

-- 6. KHÁCH HÀNG
INSERT INTO KhachHang VALUES
('KH01', N'Nguyễn Hoàng Nam', '0977000001', 'nam.nguyen@gmail.com', N'TP.HCM'),
('KH02', N'Lê Thị Lan', '0977000002', 'lan.le@gmail.com', N'Hà Nội'),
('KH03', N'Trần Văn Sơn', '0977000003', 'son.tran@gmail.com', N'Đà Nẵng'),
('KH04', N'Phạm Quỳnh Anh', '0977000004', 'anh.pham@gmail.com', N'Cần Thơ'),
('KH05', N'Hoàng Minh Tuấn', '0977000005', 'tuan.hoang@gmail.com', N'Hải Phòng');

-- 7. PHIẾU NHẬP
INSERT INTO PhieuNhap VALUES
('PN01', '2024-01-05', 'NV01'),
('PN02', '2024-02-10', 'NV02'),
('PN03', '2024-03-15', 'NV03'),
('PN04', '2024-04-20', 'NV04'),
('PN05', '2024-05-25', 'NV05');

-- 8. CHI TIẾT PHIẾU NHẬP
INSERT INTO ChiTietPhieuNhap VALUES
('S01', 'PN01', 20, 50000),
('S02', 'PN01', 15, 40000),
('S03', 'PN02', 30, 80000),
('S04', 'PN03', 10, 60000),
('S05', 'PN04', 12, 70000),
('S06', 'PN05', 25, 35000);

-- 9. HÓA ĐƠN
INSERT INTO HoaDon VALUES
('HD01', '2024-06-01', 'NV01', 'KH01', 0),
('HD02', '2024-06-02', 'NV02', 'KH02', 0),
('HD03', '2024-06-03', 'NV03', 'KH03', 0),
('HD04', '2024-06-04', 'NV04', 'KH04', 0),
('HD05', '2024-06-05', 'NV05', 'KH05', 0);
INSERT INTO HoaDon VALUES
('HD06', '2024-07-01', 'NV01', 'KH01', 0),
('HD07', '2024-07-02', 'NV02', 'KH02', 0),
('HD08', '2024-07-03', 'NV03', 'KH03', 0),
('HD09', '2024-07-04', 'NV04', 'KH04', 0),
('HD010', '2024-07-05', 'NV05', 'KH05', 0);


-- 10. CHI TIẾT HÓA ĐƠN
INSERT INTO ChiTietHoaDon VALUES
('S01', 'HD01', 2, 75000, 0),
('S02', 'HD01', 1, 65000, 10),
('S03', 'HD02', 3, 120000, 0),
('S04', 'HD03', 1, 90000, 5),
('S05', 'HD04', 2, 110000, 0),
('S06', 'HD05', 4, 55000, 0);
INSERT INTO ChiTietHoaDon VALUES
('S01', 'HD07', 2, 75000, 0),
('S02', 'HD07', 1, 65000, 10),
('S03', 'HD08', 3, 120000, 0),
('S04', 'HD08', 1, 90000, 5),
('S05', 'HD06', 2, 110000, 0),
('S06', 'HD06', 4, 55000, 0),
('S05', 'HD09', 2, 110000, 0),
('S06', 'HD09', 4, 55000, 0),
('S01', 'HD010', 2, 75000, 0),
('S02', 'HD010', 1, 65000, 10);

-- 11. TÀI KHOẢN
-- Admin (nhân viên)
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaNV)
VALUES ('admin01', '123456', N'Admin', 'NV03');

-- Nhân viên
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaNV)
VALUES ('an_nv', '123456', N'NhanVien', 'NV01');

-- Khách hàng
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaKH)
VALUES ('nam_kh', '123456', N'KhachHang', 'KH01');

---------------------------------
SELECT * FROM ChiTietHoaDon
SELECT * FROM ChiTietPhieuNhap
SELECT * FROM HoaDon
SELECT * FROM KhachHang
SELECT * FROM NhanVien
SELECT * FROM NhaXuatBan
SELECT * FROM PhieuNhap
SELECT * FROM Sach
SELECT * FROM TacGia
SELECT * FROM TheLoai
SELECT * FROM TaiKhoan
go

--==================================================================
--          CÁC HÀM VÀ THỦ TỤC CỦA CÁC THÀNH VIÊN
--==================================================================
-- ==============================
-- La Văn Nam (Quản lý nhân viên)
-- ==============================
PRINT N'--- Đang tạo mới ---';
go
-- 1. FUNCTION: TỰ ĐỘNG SINH MÃ NHÂN VIÊN
-- Mục đích: Giúp app không cần lo việc tính toán mã NV
CREATE FUNCTION fn_TuDongTangMaNV()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @MaxMaNV VARCHAR(10)
    DECLARE @NextMaNV VARCHAR(10)
    DECLARE @SoThuTu INT

    -- Lấy mã nhân viên lớn nhất hiện tại
    SELECT @MaxMaNV = MAX(MaNV) FROM NhanVien

    IF @MaxMaNV IS NULL
        SET @NextMaNV = 'NV01'
    ELSE
    BEGIN
        -- Tách số, cộng 1 và format lại (NV09 -> NV10)
        SET @SoThuTu = CAST(SUBSTRING(@MaxMaNV, 3, LEN(@MaxMaNV)) AS INT) + 1
        SET @NextMaNV = 'NV' + RIGHT('00' + CAST(@SoThuTu AS VARCHAR(2)), 2)
    END

    RETURN @NextMaNV
END
GO

-- 2. STORED PROCEDURE: THÊM NHÂN VIÊN (TRANSACTION)
-- Mục đích: Thêm NV và tự động tạo Tài khoản
CREATE PROCEDURE sp_ThemNhanVienMoi
    @TenNV NVARCHAR(100),
    @ChucVu NVARCHAR(50),
    @DienThoai VARCHAR(15),
    @Email NVARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- B1: Lấy mã tự động
        DECLARE @NewMaNV VARCHAR(10) = dbo.fn_TuDongTangMaNV();

        -- B2: Insert Nhân Viên
        INSERT INTO NhanVien (MaNV, TenNV, ChucVu, DienThoai, Email)
        VALUES (@NewMaNV, @TenNV, @ChucVu, @DienThoai, @Email);

        -- B3: Insert Tài Khoản (Pass mặc định 123456)
        DECLARE @VaiTro NVARCHAR(20)
        IF @ChucVu = N'Quản lý' SET @VaiTro = 'Admin'
        ELSE SET @VaiTro = 'NhanVien'

        INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaNV, TrangThai)
        VALUES (@Email, '123456', @VaiTro, @NewMaNV, 1);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- 3. STORED PROCEDURE: XÓA NHÂN VIÊN (TRANSACTION)
-- Mục đích: Xóa an toàn, kiểm tra lịch sử giao dịch
CREATE PROCEDURE sp_XoaNhanVien
    @MaNV VARCHAR(10)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Kiểm tra ràng buộc kinh doanh
        IF EXISTS (SELECT 1 FROM HoaDon WHERE MaNV = @MaNV) 
           OR EXISTS (SELECT 1 FROM PhieuNhap WHERE MaNV = @MaNV)
        BEGIN
            RAISERROR(N'Không thể xóa nhân viên này vì họ đã lập Hóa đơn hoặc Phiếu nhập.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Xóa Tài khoản trước
        DELETE FROM TaiKhoan WHERE MaNV = @MaNV;
        -- Xóa Nhân viên sau
        DELETE FROM NhanVien WHERE MaNV = @MaNV;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @Msg NVARCHAR(MAX) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

-- 4. STORED PROCEDURE: KIỂM TRA ĐĂNG NHẬP
-- Mục đích: Phục vụ cho App Python Login
CREATE PROCEDURE sp_KiemTraDangNhap
    @TenDangNhap VARCHAR(50),
    @MatKhau NVARCHAR(200)
AS
BEGIN
    SELECT VaiTro, MaNV, TrangThai
    FROM TaiKhoan
    WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau
END
GO

--=========================================
-- Trần Thị Minh Phương (Quản Lý Tài Khoản)
--=========================================
--1. Đăng ký tài khoản khách--
CREATE PROCEDURE sp_DangKyTaiKhoanKhach
@MaKH VARCHAR(10),
@TenDangNhap VARCHAR(50),
@MatKhau NVARCHAR(200),
@VaiTro NVARCHAR(20) 
AS
BEGIN
	IF  EXISTS ( SELECT 1
				 FROM TaiKhoan
				 WHERE MaKH = @MaKH OR TenDangNhap = @TenDangNhap
				 )
				 BEGIN
					PRINT N'Tài khoản này đã tồn tại!!';
				 END
	ELSE 
	BEGIN
		INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaKH, NgayTao)
		VALUES (@MaKH, @TenDangNhap, @MatKhau, @VaiTro, GETDATE());
		PRINT N'Đăng ký tài khoản thành công!!';
	END
END;
--test--
EXEC sp_DangKyTaiKhoanKhach 
    @MaKH = 'KH01', 
    @TenDangNhap = 'nam_kh', 
    @MatKhau = '123456', 
    @VaiTro = N'KhachHang';
go

--2. Cập nhật thông tin khách hàng--
CREATE PROCEDURE sp_UpdateTaikhoan
@MaKH VARCHAR (10),
@TenDangNhap VARCHAR(50),
@MatKhau NVARCHAR(200),
@VaiTro NVARCHAR(20)  
AS
BEGIN 
SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM TaiKhoan WHERE MaKH = @MaKH)
	BEGIN
		UPDATE TaiKhoan
		SET TenDangNhap = @TenDangNhap,
			MatKhau = @MatKhau,
			VaiTro = @VaiTro
		WHERE MaKH = @MaKH;

		PRINT N'Cập nhật thông tin thành công';

	END
	ELSE
		BEGIN
			PRINT N'Mã khách hàng không tồn tại';
		END
END;
--TEST--
EXEC sp_UpdateTaikhoan
    @MaKH = 'KH01', 
    @TenDangNhap = 'nguyen_kh', 
    @MatKhau = '1234567', 
    @VaiTro = N'KhachHang'; 
go

--3. Tìm kiếm tài khoản khách hàng--
CREATE PROCEDURE sp_findTaiKhoan
@MaKH VARCHAR (10)
AS
BEGIN 
	IF EXISTS (SELECT 1 FROM TaiKhoan WHERE MaKH = @MaKH)
	BEGIN
		SELECT tk.MaKH, tk.TenDangNhap, tk.TrangThai, tk.VaiTro, tk.NgayTao
		FROM TaiKhoan tk
		JOIN KhachHang kh ON tk.MaKH = kh.MaKH
		WHERE @MaKH = tk.MaKH
	END
	ELSE
		BEGIN
			PRINT N'Mã khách hàng không tồn tại';
		END
END;
--TEST--
EXEC sp_findTaiKhoan
    @MaKH = 'KH01';
go

--4. Thống kê thu nhập tháng--
CREATE PROCEDURE sp_sumDoanhThu
AS
BEGIN 
	SELECT 
		YEAR (hd.NgayLap) AS NAM,
		MONTH (hd.NgayLap) AS THANG,
		SUM (ct.SoLuong * ct.DonGia * (1 - ct.GiamGia/100 )) AS TONGDOANHTHUTHANG
	FROM HoaDon hd
	JOIN ChiTietHoaDon ct ON ct.SoHD = hd.SoHD
	GROUP BY YEAR(hd.NgayLap), MONTH (hd.NgayLap)
	ORDER BY NAM, THANG;
END;
EXEC sp_sumDoanhThu
go

IF COL_LENGTH('TaiKhoan', 'MatKhauHash') IS NULL
BEGIN
    ALTER TABLE TaiKhoan ADD 
        MatKhauHash VARBINARY(64) NULL,
        FailedLoginCount INT NOT NULL DEFAULT 0,
        LockedUntil DATETIME NULL,
        LastLogin DATETIME NULL,
        LastLoginIP VARCHAR(50) NULL,
        AllowedLoginStart TIME NULL,
        AllowedLoginEnd TIME NULL,
        LastPasswordChange DATETIME NULL;
END
GO

-- 1.2. Chuyển đổi mật khẩu cũ sang Hash (Chỉ chạy 1 lần)
UPDATE dbo.TaiKhoan
SET MatKhauHash = HASHBYTES('SHA2_256', MatKhau)
WHERE MatKhau IS NOT NULL AND MatKhauHash IS NULL;
GO

-- 1.3. Bảng Lịch sử đăng nhập (Có cột ThietBi, TrinhDuyet)
IF OBJECT_ID('dbo.LichSuDangNhap', 'U') IS NULL
BEGIN
    CREATE TABLE LichSuDangNhap (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap VARCHAR(50) NULL,
        ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
        IP VARCHAR(50) NULL,
        ThietBi NVARCHAR(250) NULL,   -- Cột mới
        TrinhDuyet NVARCHAR(250) NULL, -- Cột mới
        ThanhCong BIT NOT NULL,
        GhiChu NVARCHAR(400) NULL
    );
    CREATE INDEX IX_LichSuDangNhap_Ten ON dbo.LichSuDangNhap(TenDangNhap);
    
    ALTER TABLE LichSuDangNhap 
    ADD CONSTRAINT FK_LichSu_TaiKhoan FOREIGN KEY (TenDangNhap) REFERENCES TaiKhoan(TenDangNhap);
END
ELSE
BEGIN
    -- Nếu bảng đã tồn tại nhưng thiếu cột, thì thêm vào
    IF COL_LENGTH('LichSuDangNhap', 'ThietBi') IS NULL
        ALTER TABLE LichSuDangNhap ADD ThietBi NVARCHAR(250) NULL;
    
    IF COL_LENGTH('LichSuDangNhap', 'TrinhDuyet') IS NULL
        ALTER TABLE LichSuDangNhap ADD TrinhDuyet NVARCHAR(250) NULL;
END
GO

-- 1.4. Bảng Reset Token (Quên mật khẩu)
IF OBJECT_ID('dbo.ResetToken', 'U') IS NULL
BEGIN
    CREATE TABLE ResetToken (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap VARCHAR(50) NOT NULL,
        Token VARCHAR(100) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        ExpiresAt DATETIME NOT NULL,
        Used BIT NOT NULL DEFAULT 0
    );
    CREATE INDEX IX_ResetToken_Token ON dbo.ResetToken(Token);

    ALTER TABLE ResetToken 
    ADD CONSTRAINT FK_Reset_TaiKhoan FOREIGN KEY (TenDangNhap) REFERENCES TaiKhoan(TenDangNhap);
END
GO

-- 1.5. Bảng Audit (Ghi vết hệ thống)
IF OBJECT_ID('dbo.TaiKhoanAudit', 'U') IS NULL
BEGIN
    CREATE TABLE TaiKhoanAudit (
        AuditID INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap VARCHAR(50) NULL,
        ActionType NVARCHAR(30) NOT NULL,
        ActionBy SYSNAME NOT NULL DEFAULT SUSER_SNAME(),
        ActionAt DATETIME NOT NULL DEFAULT GETDATE(),
        Details NVARCHAR(MAX) NULL
    );
END
GO

-- 2.1. Check IP Mới
CREATE OR ALTER FUNCTION dbo.fn_IsIPNew(@TenDangNhap VARCHAR(50), @IP VARCHAR(50))
RETURNS BIT
AS
BEGIN
    IF @IP IS NULL OR @IP='' RETURN 0;
    IF EXISTS (SELECT 1 FROM dbo.LichSuDangNhap WHERE TenDangNhap=@TenDangNhap AND IP=@IP AND ThanhCong=1) RETURN 0;
    RETURN 1;
END;
GO

-- 2.2. Check Khung Giờ
CREATE OR ALTER FUNCTION dbo.fn_IsWithinAllowedTime(@User VARCHAR(50), @Now DATETIME)
RETURNS BIT
AS
BEGIN
    DECLARE @start TIME, @end TIME, @role NVARCHAR(20);
    SELECT @start = AllowedLoginStart, @end = AllowedLoginEnd, @role = VaiTro
    FROM dbo.TaiKhoan WHERE TenDangNhap = @User;

    IF @start IS NOT NULL AND @end IS NOT NULL
        RETURN CASE WHEN CAST(@Now AS TIME) BETWEEN @start AND @end THEN 1 ELSE 0 END;

    IF @role='NhanVien'
        RETURN CASE WHEN CAST(@Now AS TIME) BETWEEN '08:00' AND '17:30' THEN 1 ELSE 0 END;

    RETURN 1;
END;
GO

-- 3.1. Audit mọi thay đổi trên bảng TaiKhoan
CREATE OR ALTER TRIGGER dbo.trg_AuditTaiKhoan
ON dbo.TaiKhoan
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    -- INSERT
    INSERT INTO dbo.TaiKhoanAudit(TenDangNhap, ActionType, Details)
    SELECT i.TenDangNhap, 'INSERT', 'Created account' FROM inserted i LEFT JOIN deleted d ON i.TenDangNhap=d.TenDangNhap WHERE d.TenDangNhap IS NULL;
    -- DELETE
    INSERT INTO dbo.TaiKhoanAudit(TenDangNhap, ActionType, Details)
    SELECT d.TenDangNhap, 'DELETE', 'Deleted account' FROM deleted d LEFT JOIN inserted i ON d.TenDangNhap=i.TenDangNhap WHERE i.TenDangNhap IS NULL;
    -- UPDATE
    INSERT INTO dbo.TaiKhoanAudit(TenDangNhap, ActionType, Details)
    SELECT i.TenDangNhap, 'UPDATE', 'Updated info' FROM inserted i JOIN deleted d ON i.TenDangNhap=d.TenDangNhap WHERE EXISTS (SELECT i.TenDangNhap EXCEPT SELECT d.TenDangNhap);
END;
GO

-- 3.2. Xử lý logic sau khi ghi log đăng nhập
CREATE OR ALTER TRIGGER dbo.trg_LichSuDangNhap_AfterInsert
ON dbo.LichSuDangNhap
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @id INT, @user VARCHAR(50), @ip VARCHAR(50), @now DATETIME;
    SELECT TOP 1 @id=ID, @user=TenDangNhap, @ip=IP, @now=ThoiGian FROM inserted;

    IF @user IS NULL RETURN;

    -- Check giờ
    IF dbo.fn_IsWithinAllowedTime(@user,@now)=0
        UPDATE dbo.LichSuDangNhap SET ThanhCong=0, GhiChu = ISNULL(GhiChu,'') + ' | Outside allowed time' WHERE ID=@id;

    -- Check IP lạ
    IF dbo.fn_IsIPNew(@user,@ip)=1
    BEGIN
        UPDATE dbo.LichSuDangNhap SET GhiChu = ISNULL(GhiChu,'') + ' | New IP detected' WHERE ID=@id;
        INSERT INTO dbo.TaiKhoanAudit(TenDangNhap, ActionType, Details) VALUES(@user,'SUSPICIOUS_IP','New IP: '+ISNULL(@ip,''));
    END
END;
GO

-- 4.1. Xử lý Đăng nhập
CREATE OR ALTER PROCEDURE dbo.sp_AttemptLogin
    @TenDangNhap VARCHAR(50),
    @MatKhau NVARCHAR(200),
    @IP VARCHAR(50) = NULL,
    @ThietBi NVARCHAR(200) = NULL,   -- Cập nhật mới
    @TrinhDuyet NVARCHAR(200) = NULL -- Cập nhật mới
AS
BEGIN
    SET NOCOUNT ON;
    -- Lấy giá trị mặc định nếu C# gửi null
    SET @IP = COALESCE(@IP, CAST(CONNECTIONPROPERTY('client_net_address') AS VARCHAR(50)));
    SET @ThietBi = COALESCE(@ThietBi, HOST_NAME());
    SET @TrinhDuyet = COALESCE(@TrinhDuyet, APP_NAME());

    DECLARE @now DATETIME = GETDATE();
    DECLARE @hash VARBINARY(64);
    DECLARE @failed INT;
    DECLARE @locked DATETIME;
    DECLARE @valid BIT = 0;

    -- Case 1: Không tồn tại user
    IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap=@TenDangNhap)
    BEGIN
        INSERT INTO dbo.LichSuDangNhap(TenDangNhap,IP,ThietBi,TrinhDuyet,ThanhCong,GhiChu)
        VALUES(@TenDangNhap, @IP, @ThietBi, @TrinhDuyet, 0, N'User not found');
        RETURN;
    END

    SELECT @hash=MatKhauHash, @failed=FailedLoginCount, @locked=LockedUntil
    FROM dbo.TaiKhoan WHERE TenDangNhap=@TenDangNhap;

    -- Case 2: Tài khoản đang bị khóa
    IF @locked IS NOT NULL AND @locked > @now
    BEGIN
        INSERT INTO dbo.LichSuDangNhap(TenDangNhap,IP,ThietBi,TrinhDuyet,ThanhCong,GhiChu)
        VALUES(@TenDangNhap, @IP, @ThietBi, @TrinhDuyet, 0, 'Locked until '+CONVERT(NVARCHAR(30),@locked));
        RETURN;
    END

    -- Case 3: Chặn bởi khung giờ
    IF dbo.fn_IsWithinAllowedTime(@TenDangNhap,@now)=0
    BEGIN
        INSERT INTO dbo.LichSuDangNhap(TenDangNhap,IP,ThietBi,TrinhDuyet,ThanhCong,GhiChu)
        VALUES(@TenDangNhap, @IP, @ThietBi, @TrinhDuyet, 0, 'Blocked by Time Policy');
        RETURN;
    END

    -- Kiểm tra Hash
    IF @hash = HASHBYTES('SHA2_256', @MatKhau) SET @valid = 1;

    IF @valid = 1
    BEGIN
        -- Case 4: Thành công
        UPDATE dbo.TaiKhoan SET FailedLoginCount=0, LockedUntil=NULL, LastLogin=@now, LastLoginIP=@IP
        WHERE TenDangNhap=@TenDangNhap;

        INSERT INTO dbo.LichSuDangNhap(TenDangNhap,IP,ThietBi,TrinhDuyet,ThanhCong,GhiChu)
        VALUES(@TenDangNhap, @IP, @ThietBi, @TrinhDuyet, 1, N'Success');
    END
    ELSE
    BEGIN
        -- Case 5: Sai mật khẩu
        UPDATE dbo.TaiKhoan SET FailedLoginCount = FailedLoginCount + 1 WHERE TenDangNhap=@TenDangNhap;
        SELECT @failed = FailedLoginCount FROM dbo.TaiKhoan WHERE TenDangNhap=@TenDangNhap;

        -- Khóa nếu sai >= 5 lần
        IF @failed >= 5
        BEGIN
            UPDATE dbo.TaiKhoan SET LockedUntil = DATEADD(MINUTE, 30, GETDATE()) WHERE TenDangNhap=@TenDangNhap;
            INSERT INTO dbo.TaiKhoanAudit(TenDangNhap,ActionType,Details) VALUES(@TenDangNhap,'AUTO_LOCK','Locked 30m after 5 failed attempts');
        END

        INSERT INTO dbo.LichSuDangNhap(TenDangNhap,IP,ThietBi,TrinhDuyet,ThanhCong,GhiChu)
        VALUES(@TenDangNhap, @IP, @ThietBi, @TrinhDuyet, 0, 'Wrong Password ('+CAST(@failed AS NVARCHAR)+')');
    END
END;
GO

-- 4.2. Quên mật khẩu
CREATE OR ALTER PROCEDURE dbo.sp_RequestPasswordReset 
    @TenDangNhap VARCHAR(50),
    @ExpireMinutes INT = 30,
    @OutToken VARCHAR(100) OUTPUT
AS
BEGIN
    DECLARE @token VARCHAR(100) = CONVERT(VARCHAR(36), NEWID());
    INSERT INTO dbo.ResetToken(TenDangNhap,Token,ExpiresAt)
    VALUES(@TenDangNhap, @token, DATEADD(MINUTE, @ExpireMinutes, GETDATE()));
    SET @OutToken = @token;
END
GO

-- 4.3. Đổi mật khẩu
CREATE OR ALTER PROCEDURE dbo.sp_ResetPasswordWithToken 
    @TenDangNhap VARCHAR(50),
    @Token VARCHAR(100),
    @MatKhauMoi NVARCHAR(200)
AS
BEGIN
    DECLARE @expires DATETIME, @used BIT;
    SELECT @expires = ExpiresAt, @used = Used FROM dbo.ResetToken WHERE TenDangNhap=@TenDangNhap AND Token=@Token;

    IF @expires IS NULL OR @used = 1 OR GETDATE() > @expires RAISERROR('Invalid Token',16,1);

    UPDATE dbo.TaiKhoan SET MatKhauHash = HASHBYTES('SHA2_256', @MatKhauMoi) WHERE TenDangNhap=@TenDangNhap;
    UPDATE dbo.ResetToken SET Used = 1 WHERE TenDangNhap=@TenDangNhap AND Token=@Token;
    INSERT INTO dbo.LichSuDangNhap(TenDangNhap,ThanhCong,GhiChu) VALUES(@TenDangNhap,1,'Password Reset');
END
GO

-- 5.1. DÙNG CURSOR: Quét password
CREATE OR ALTER PROCEDURE dbo.sp_CheckPasswordHealth
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @User VARCHAR(50), @LastChange DATETIME;
    DECLARE @ResultTable TABLE (TenDangNhap VARCHAR(50), TrangThai NVARCHAR(200));

    DECLARE cur_Pwd CURSOR FOR SELECT TenDangNhap, LastPasswordChange FROM dbo.TaiKhoan;
    OPEN cur_Pwd;
    FETCH NEXT FROM cur_Pwd INTO @User, @LastChange;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @LastChange IS NULL INSERT INTO @ResultTable VALUES (@User, N'Nguy hiểm: Chưa đổi mật khẩu bao giờ!');
        ELSE IF DATEDIFF(DAY, @LastChange, GETDATE()) > 90 INSERT INTO @ResultTable VALUES (@User, N'Cảnh báo: Mật khẩu quá cũ (>90 ngày)');
        ELSE INSERT INTO @ResultTable VALUES (@User, N'Tốt');

        FETCH NEXT FROM cur_Pwd INTO @User, @LastChange;
    END

    CLOSE cur_Pwd;
    DEALLOCATE cur_Pwd;
    SELECT * FROM @ResultTable;
END;
GO

-- 5.2. DÙNG TRANSACTION: Xóa user an toàn (Có Rollback)
CREATE OR ALTER PROCEDURE dbo.sp_DeleteUserSafe
    @TenDangNhap VARCHAR(50),
    @SimulateError BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
            DELETE FROM dbo.ResetToken WHERE TenDangNhap = @TenDangNhap;
            DELETE FROM dbo.LichSuDangNhap WHERE TenDangNhap = @TenDangNhap;
            
            IF @SimulateError = 1 RAISERROR(N'Lỗi giả lập để test Rollback!', 16, 1);

            DELETE FROM dbo.TaiKhoan WHERE TenDangNhap = @TenDangNhap;
        COMMIT TRANSACTION;
        SELECT 1 AS Code, N'Xóa thành công user ' + @TenDangNhap AS Msg;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        INSERT INTO TaiKhoanAudit(TenDangNhap, ActionType, Details) VALUES (@TenDangNhap, 'DELETE_FAIL', ERROR_MESSAGE());
        SELECT 0 AS Code, N'Đã ROLLBACK. Lỗi: ' + ERROR_MESSAGE() AS Msg;
    END CATCH
END;
GO

-- TẠO DATA MẪU ADMIN (Nếu chưa có)
IF NOT EXISTS(SELECT 1 FROM TaiKhoan WHERE TenDangNhap='admin')
    INSERT INTO TaiKhoan(TenDangNhap,MatKhau,MatKhauHash,LastPasswordChange)
    VALUES('admin','123',HASHBYTES('SHA2_256','123'),GETDATE());
GO

CREATE OR ALTER PROCEDURE dbo.sp_UnlockAccount
    @TenDangNhap VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra user có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = @TenDangNhap)
    BEGIN
        RAISERROR(N'Tài khoản không tồn tại!', 16, 1);
        RETURN;
    END

    -- Thực hiện mở khóa: Reset đếm lỗi về 0, Xóa thời gian khóa
    UPDATE dbo.TaiKhoan
    SET FailedLoginCount = 0,
        LockedUntil = NULL
    WHERE TenDangNhap = @TenDangNhap;

    -- Ghi lại hành động này vào Audit để biết ai đã mở khóa
    INSERT INTO dbo.TaiKhoanAudit(TenDangNhap, ActionType, Details)
    VALUES (@TenDangNhap, 'MANUAL_UNLOCK', N'Account unlocked by Admin');

    SELECT 1 AS Code, N'Đã mở khóa thành công cho tài khoản: ' + @TenDangNhap AS Msg;
END;
GO
EXEC dbo.sp_UnlockAccount @TenDangNhap = 'admin01';
go

--================================
-- Lê Lưu Gia Khang (Quản Lý Sách)
--================================
-- 1. Trigger lưu lịch sử giá 
CREATE TRIGGER trg_LuuLichSuGia
ON Sach
AFTER UPDATE
AS
BEGIN
    IF UPDATE(GiaBia)
    BEGIN
        INSERT INTO LichSuGia (MaSach, GiaCu, GiaMoi, NgayThayDoi, NguoiDung)
        SELECT d.MaSach, d.GiaBia, i.GiaBia, GETDATE(), SYSTEM_USER
        FROM deleted d JOIN inserted i ON d.MaSach = i.MaSach;
    END
END;
---test 
UPDATE Sach
SET GiaBia = 120000
WHERE MaSach = 'S01';
select * from LichSuGia
GO

-- 2. Procedure Khuyến Mãi Hàng Loạt 
CREATE PROCEDURE sp_ApDungKhuyenMai
    @MaNXB VARCHAR(10),
    @PhanTramGiam INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE Sach
        SET GiaBia = GiaBia * (100.0 - @PhanTramGiam) / 100.0
        WHERE MaNXB = @MaNXB;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT N'Lỗi cập nhật giá!';
    END CATCH
END;
--test 
EXEC sp_ApDungKhuyenMai @MaNXB = 'NXB02', @PhanTramGiam = 10;
go

-- 3. Tự động sinh mã Sách mới
CREATE FUNCTION fn_TuDongTangMaSach()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @MaMax VARCHAR(10)
    DECLARE @MaMoi VARCHAR(10)
    DECLARE @SoThuTu INT

    -- 1. Lấy mã sách lớn nhất hiện tại
    SELECT @MaMax = MAX(MaSach) FROM Sach

    -- 2. Kiểm tra nếu chưa có dữ liệu
    IF @MaMax IS NULL
    BEGIN
        SET @MaMoi = 'S01'
    END
    ELSE
    BEGIN
        -- 3. Tách phần số ra (Lấy từ ký tự thứ 2 đến hết chuỗi)
        SET @SoThuTu = CAST(SUBSTRING(@MaMax, 2, LEN(@MaMax)) AS INT) + 1

        -- 4. Ghép chuỗi với kỹ thuật thêm số 0 (Padding)
        SET @MaMoi = 'S' + RIGHT('00' + CAST(@SoThuTu AS VARCHAR(10)), 2)
    END

    RETURN @MaMoi
END
GO

-- 4. Tìm kiếm Sách nâng cao
CREATE PROC sp_TimKiemSachNangCao
    @TuKhoa NVARCHAR(100) = NULL, -- Tìm theo tên sách (gần đúng)
    @MaTG VARCHAR(10) = NULL,     -- Lọc theo tác giả
    @MaTL VARCHAR(10) = NULL,     -- Lọc theo thể loại
    @MaNXB VARCHAR(10) = NULL     -- Lọc theo NXB
AS
BEGIN
    SELECT 
        s.MaSach, s.TenSach, 
        tg.TenTG, tl.TenTL, nxb.TenNXB, 
        s.NamXuatBan, s.GiaBia, s.SoLuongTon
    FROM Sach s
    JOIN TacGia tg ON s.MaTG = tg.MaTG
    JOIN TheLoai tl ON s.MaTL = tl.MaTL
    JOIN NhaXuatBan nxb ON s.MaNXB = nxb.MaNXB
    WHERE 
        (@TuKhoa IS NULL OR s.TenSach LIKE N'%' + @TuKhoa + N'%')
        AND (@MaTG IS NULL OR s.MaTG = @MaTG)
        AND (@MaTL IS NULL OR s.MaTL = @MaTL)
        AND (@MaNXB IS NULL OR s.MaNXB = @MaNXB)
END
GO

-- 5. Trigger ngăn chặn xóa sách nếu còn hàng trong kho
CREATE TRIGGER trg_ChanXoaSach_ConTonKho
ON Sach
FOR DELETE
AS
BEGIN
    -- Kiểm tra nếu có bất kỳ sách nào bị xóa mà số lượng tồn > 0
    IF EXISTS (SELECT * FROM deleted WHERE SoLuongTon > 0)
    BEGIN
        -- Báo lỗi cho người dùng
        RAISERROR (N'Không thể xóa sách vì số lượng tồn kho vẫn còn!', 16, 1);
        
        -- Hủy bỏ lệnh xóa (Rollback)
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 6. Nhập một cuốn sách mới từ một Tác giả hoàn toàn mới
CREATE OR ALTER PROCEDURE sp_ThemTacGiaVaSach_Transaction
    -- Thông tin tác giả
    @MaTG VARCHAR(10),
    @TenTG NVARCHAR(100),
    @DiaChiTG NVARCHAR(200),
    @SDT_TG VARCHAR(15),
    @EmailTG NVARCHAR(100),
    -- Thông tin sách
    @MaSach VARCHAR(10),
    @TenSach NVARCHAR(200),
    @MaTL VARCHAR(10),
    @MaNXB VARCHAR(10),
    @NamXB INT,
    @GiaBia DECIMAL(18,2),
    @SoLuongTon INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TacGia (MaTG, TenTG, DiaChi, DienThoai, Email)
        VALUES (@MaTG, @TenTG, @DiaChiTG, @SDT_TG, @EmailTG);
        
        PRINT N'✔ Đã thêm Tác giả thành công (trong bộ nhớ tạm)';

        INSERT INTO Sach (MaSach, TenSach, MaTG, MaTL, MaNXB, NamXuatBan, GiaBia, SoLuongTon)
        VALUES (@MaSach, @TenSach, @MaTG, @MaTL, @MaNXB, @NamXB, @GiaBia, @SoLuongTon);

        PRINT N'✔ Đã thêm Sách thành công (trong bộ nhớ tạm)';

        COMMIT TRANSACTION;
        PRINT N'✅ GIAO DỊCH THÀNH CÔNG! Dữ liệu đã được lưu.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        PRINT N'❌ GIAO DỊCH THẤT BẠI! Đang Rollback dữ liệu...';
        PRINT N'Lỗi chi tiết: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- =================================
-- Trần Bảo Nguyên (Quản Lý Hóa Đơn)
-- =================================
-- Tự sinh mã
CREATE PROCEDURE sp_TaoMaHoaDonTiepTheo
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaHDMoi VARCHAR(10);
    DECLARE @MaxSo INT;

    -- Lấy phần số lớn nhất từ các SoHD (ví dụ: 'HD010' -> 10)
    SELECT @MaxSo = ISNULL(MAX(CAST(SUBSTRING(SoHD, 3, 10) AS INT)), 0)
    FROM HoaDon;

    -- Tạo mã mới (ví dụ: 10 + 1 = 11 -> 'HD011')
    SET @MaxSo = @MaxSo + 1;
    SET @MaHDMoi = 'HD' + RIGHT('000' + CAST(@MaxSo AS VARCHAR(7)), 3); -- Format thành 3 chữ số

    -- Trả mã mới về cho ứng dụng
    SELECT @MaHDMoi AS MaHDMoi;
END
GO

-- =================================
-- Lê Hồng Quốc (Quản Lý Khách Hàng)
-- =================================
-- 1. Thêm khách hàng an toàn
IF OBJECT_ID('sp_ThemKhachHang', 'P') IS NOT NULL
    DROP PROCEDURE sp_ThemKhachHang
GO
CREATE PROCEDURE sp_ThemKhachHang
    @MaKH VARCHAR(10),
    @TenKH NVARCHAR(100),
    @DienThoai VARCHAR(15),
    @Email NVARCHAR(100),
    @DiaChi NVARCHAR(200)
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
        BEGIN
            PRINT N'Lỗi: Mã khách hàng đã tồn tại!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM KhachHang WHERE DienThoai = @DienThoai)
        BEGIN
            PRINT N'Lỗi: Số điện thoại này đã được sử dụng!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO KhachHang (MaKH, TenKH, DienThoai, Email, DiaChi)
        VALUES (@MaKH, @TenKH, @DienThoai, @Email, @DiaChi);

        COMMIT TRANSACTION;
        PRINT N'Thêm khách hàng thành công!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT N'Đã xảy ra lỗi hệ thống. Giao dịch bị hủy!';
        PRINT ERROR_MESSAGE(); 
    END CATCH
END;
GO

--TH 1: Thêm thành công
EXEC sp_ThemKhachHang 'KH08', N'Nguyễn Văn A', '0909123456', 'a@gmail.com', N'Hà Nội';
--TH 2: Test Transaction Rollback (Thêm trùng Mã KH01 vừa tạo -> Sẽ báo lỗi và Rollback)
EXEC sp_ThemKhachHang 'KH08', N'Nguyễn Văn B', '0999888777', 'b@gmail.com', N'TPHCM';

SELECT * FROM KhachHang;

-- 2. Kiểm tra định dạng số điện thoại (Insert + Update)
IF OBJECT_ID('trg_KiemTraSDT', 'TR') IS NOT NULL
    DROP TRIGGER trg_KiemTraSDT;
GO
CREATE TRIGGER trg_KiemTraSDT
ON KhachHang
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (SELECT * FROM inserted WHERE LEN(DienThoai) < 10)
    BEGIN
        PRINT N'Lỗi: Số điện thoại phải có ít nhất 10 chữ số!';
        ROLLBACK TRANSACTION; 
    END

	IF EXISTS (SELECT * FROM inserted WHERE LEN(DienThoai) > 11)
    BEGIN
        PRINT N'Lỗi: Số điện thoại không được quá 11 chữ số!';
        ROLLBACK TRANSACTION; 
    END
END;
GO
-- test (Sẽ bị lỗi do sđt chỉ có 3 số)
INSERT INTO KhachHang (MaKH, TenKH, DienThoai, Email, DiaChi)
VALUES ('KH07', N'Trần Thị C', '12312313123123', 'cd@gmail.com', N'Đà Nẵng');

-- 3. Trả về Tên khách hàng dựa vào Mã khách hàng
IF OBJECT_ID('fn_CheSoDienThoai', 'FN') IS NOT NULL
    DROP FUNCTION fn_CheSoDienThoai
GO
CREATE FUNCTION fn_CheSoDienThoai (@MaKH VARCHAR(10))
RETURNS VARCHAR(20)
AS
BEGIN
    DECLARE @SDTGoc VARCHAR(15);
    DECLARE @KetQua VARCHAR(20);

    SELECT @SDTGoc = DienThoai FROM KhachHang WHERE MaKH = @MaKH;

    IF @SDTGoc IS NULL OR LEN(@SDTGoc) < 7
        SET @KetQua = N'Chưa có số điện thoại!';
    ELSE
        SET @KetQua = LEFT(@SDTGoc, 3) + '****' + RIGHT(@SDTGoc, 3);

    RETURN @KetQua;
END;
GO
-- test
SELECT 
	MaKH,
    TenKH,
    dbo.fn_CheSoDienThoai(MaKH) AS SDT_BaoMat 
FROM KhachHang;

-- 4. Kiểm tra địa chỉ khách hàng và in ra địa chỉ tương ứng 
IF OBJECT_ID('sp_BaoCaoKhachHang', 'P') IS NOT NULL
    DROP PROCEDURE sp_BaoCaoKhachHang
GO
CREATE PROCEDURE sp_BaoCaoKhachHang
AS
BEGIN
    DECLARE @MaKH VARCHAR(10)
    DECLARE @TenKH NVARCHAR(100)
    DECLARE @DiaChi NVARCHAR(200)

    DECLARE cur_KhachHang CURSOR FOR 
    SELECT MaKH, TenKH, DiaChi FROM KhachHang

    OPEN cur_KhachHang

    FETCH NEXT FROM cur_KhachHang INTO @MaKH, @TenKH, @DiaChi

    -- Chạy khi còn dữ liệu (@@FETCH_STATUS = 0 đọc thành công)
    WHILE @@FETCH_STATUS = 0
    BEGIN
        
        IF @DiaChi LIKE N'%Hà Nội%'
            PRINT N'Khách hàng ' + @TenKH + N' (' + @MaKH + N') thuộc khu vực Phía Bắc.';
        ELSE IF @DiaChi LIKE N'%HCM%' OR @DiaChi LIKE N'%Hồ Chí Minh%'
            PRINT N'Khách hàng ' + @TenKH + N' (' + @MaKH + N') thuộc khu vực Phía Nam.';
        ELSE
            PRINT N'Khách hàng ' + @TenKH + N' (' + @MaKH + N') thuộc khu vực Tỉnh khác.';

        FETCH NEXT FROM cur_KhachHang INTO @MaKH, @TenKH, @DiaChi
    END

    CLOSE cur_KhachHang

    DEALLOCATE cur_KhachHang
END;
GO
-- test
EXEC sp_BaoCaoKhachHang;
go

-- =======================================================
-- CÁC HÀM, THỦ TỤC, TRIGGER, PHÂN QUYỀN, BẢO MẬT CỦA NHÓM
-- =======================================================
-- Transaction
-- 1. Sử dụng mức cô lập cao nhất để khóa phạm vi
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

    PRINT N'[Manager] Bắt đầu quét dữ liệu để chốt sổ...';

    -- 2. Tính tổng số lượng nhập hôm nay
    DECLARE @TongNhap1 INT;
    SELECT @TongNhap1 = SUM(SoLuongNhap)
    FROM ChiTietPhieuNhap CT
    JOIN PhieuNhap PN ON CT.SoPN = PN.SoPN
    WHERE CAST(PN.NgayNhap AS DATE) = CAST(GETDATE() AS DATE);

    PRINT N'[Manager] Tổng nhập lần 1: ' + CAST(@TongNhap1 AS NVARCHAR(20));

    -- 3. Giả lập việc kiểm tra giấy tờ mất 15 giây (giữ khóa)
    PRINT N'[Manager] Đang đối chiếu chứng từ, vui lòng đợi 15s...';
    WAITFOR DELAY '00:00:15';

    -- 4. Kiểm tra lại tổng số lượng
    DECLARE @TongNhap2 INT;
    SELECT @TongNhap2 = SUM(SoLuongNhap)
    FROM ChiTietPhieuNhap CT
    JOIN PhieuNhap PN ON CT.SoPN = PN.SoPN
    WHERE CAST(PN.NgayNhap AS DATE) = CAST(GETDATE() AS DATE);

    PRINT N'[Manager] Tổng nhập lần 2: ' + CAST(@TongNhap2 AS NVARCHAR(20));

    IF @TongNhap1 = @TongNhap2
        PRINT N'[Manager]  Sổ đã chốt an toàn.';
    ELSE
        PRINT N'[Manager] Lỗi: Có phiếu nhập mới chèn vào lúc đang chốt!';

COMMIT TRANSACTION;


-----
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

    PRINT N'[Manager] Bắt đầu chốt doanh thu ca hôm nay...';

    -- 1. Lấy tổng doanh thu lần 1
    DECLARE @DoanhThu1 MONEY;
    SELECT @DoanhThu1 = SUM(TongTien)
    FROM HoaDon
    WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE);

    PRINT N'[Manager] Doanh thu lần 1: ' + CAST(@DoanhThu1 AS NVARCHAR(20));

    -- 2. Mô phỏng quá trình đối chiếu cash – POS – chuyển khoản (mất 12 giây)
    PRINT N'[Manager] Đang đối chiếu sổ sách, vui lòng đợi 12s...';
    WAITFOR DELAY '00:00:12';

    -- 3. Lấy tổng doanh thu lần 2
    DECLARE @DoanhThu2 MONEY;
    SELECT @DoanhThu2 = SUM(TongTien)
    FROM HoaDon
    WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE);

    PRINT N'[Manager] Doanh thu lần 2: ' + CAST(@DoanhThu2 AS NVARCHAR(20));

    -- 4. So sánh
    IF @DoanhThu1 = @DoanhThu2
        PRINT N'[Manager] Chốt ca thành công – Không có hóa đơn nào bị chèn trong lúc đối chiếu.';
    ELSE
        PRINT N'[Manager] CẢNH BÁO: Có hóa đơn mới phát sinh khi đang chốt ca!';

COMMIT TRANSACTION;

-- ====================================
-- BẢO MẬT & PHÂN QUYỀN (ROLES & USERS)
-- ====================================
PRINT N'--- ĐANG CẤU HÌNH BẢO MẬT... ---';

-- Tạo Role
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Role_QuanLy') CREATE ROLE [Role_QuanLy];
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Role_BanHang') CREATE ROLE [Role_BanHang];
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Role_ThuKho') CREATE ROLE [Role_ThuKho];

-- Cấp quyền cho Role
-- QUẢN LÝ
ALTER ROLE [db_owner] ADD MEMBER [Role_QuanLy];

-- BÁN HÀNG
GRANT SELECT ON SCHEMA::dbo TO [Role_BanHang];
GRANT EXECUTE ON OBJECT::sp_ThemHoaDon_Transaction TO [Role_BanHang];
GRANT EXECUTE ON OBJECT::sp_TaoMaHoaDonTiepTheo TO [Role_BanHang];
GRANT EXECUTE ON TYPE::dbo.TYPE_ChiTietHoaDon TO [Role_BanHang];
-- Chặn sửa xóa trực tiếp để bắt buộc dùng Procedure
DENY DELETE, UPDATE ON OBJECT::HoaDon TO [Role_BanHang];
DENY DELETE ON OBJECT::ChiTietHoaDon TO [Role_BanHang];

-- THỦ KHO
GRANT SELECT ON SCHEMA::dbo TO [Role_ThuKho];
DENY INSERT, UPDATE, DELETE ON OBJECT::HoaDon TO [Role_ThuKho];
DENY INSERT, UPDATE, DELETE ON OBJECT::ChiTietHoaDon TO [Role_ThuKho];
DENY EXECUTE ON OBJECT::sp_ThemHoaDon_Transaction TO [Role_ThuKho];
GO

-- Tạo Login & User
-- Tạo Login Server (Password demo: 123)
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'Login_Nguyen') CREATE LOGIN [Login_Nguyen] WITH PASSWORD = '123', CHECK_POLICY = OFF;
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'Login_Phuong') CREATE LOGIN [Login_Phuong] WITH PASSWORD = '123', CHECK_POLICY = OFF;
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'Login_Quoc') CREATE LOGIN [Login_Quoc] WITH PASSWORD = '123', CHECK_POLICY = OFF;
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'Login_Nam') CREATE LOGIN [Login_Nam] WITH PASSWORD = '123', CHECK_POLICY = OFF;
-- Tạo User Database
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'User_Nguyen') CREATE USER [User_Nguyen] FOR LOGIN [Login_Nguyen];
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'User_Phuong') CREATE USER [User_Phuong] FOR LOGIN [Login_Phuong];
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'User_Quoc') CREATE USER [User_Quoc] FOR LOGIN [Login_Quoc];
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'User_Nam') CREATE USER [User_Nam] FOR LOGIN [Login_Nam];
-- Gán User vào Role
ALTER ROLE [Role_QuanLy] ADD MEMBER [User_Nguyen];
ALTER ROLE [Role_BanHang] ADD MEMBER [User_Phuong];
ALTER ROLE [Role_ThuKho] ADD MEMBER [User_Quoc];
ALTER ROLE [Role_ThuKho] ADD MEMBER [User_Nam];
GO

PRINT N'--- HOÀN TẤT CÀI ĐẶT HỆ THỐNG ---';
GO

---------------------------------------------------------------------------------------------------
-- FUNCTION (HÀM):

--Hàm tính giảm giá
CREATE FUNCTION fn_TinhGiaSauGiam (
    @DonGia DECIMAL(18,2),
    @GiamGia DECIMAL(5,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    RETURN @DonGia * (1 - @GiamGia / 100.0)
END
GO

SELECT dbo.fn_TinhGiaSauGiam(100000, 10) 
go

--Hàm tính tồng tiền của một hóa đơn
CREATE FUNCTION fn_TinhTongTienHoaDon (
    @SoHD VARCHAR(10)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongTien DECIMAL(18,2)
    
    SELECT @TongTien = ISNULL(SUM(SoLuong * dbo.fn_TinhGiaSauGiam(DonGia, GiamGia)), 0)
    FROM ChiTietHoaDon
    WHERE SoHD = @SoHD
    
    RETURN @TongTien
END
GO

SELECT dbo.fn_TinhTongTienHoaDon('HD01') 
go

--Hàm tìm kiếm sách theo tác giả
CREATE FUNCTION fn_TimSachTheoTacGia (@TenTG NVARCHAR(100))
RETURNS TABLE
AS
RETURN (
    SELECT s.MaSach, s.TenSach, tg.TenTG, nxb.TenNXB, s.NamXuatBan, s.GiaBia, s.SoLuongTon
    FROM Sach s
    JOIN TacGia tg ON s.MaTG = tg.MaTG
    JOIN NhaXuatBan nxb ON s.MaNXB = nxb.MaNXB
    WHERE tg.TenTG LIKE '%' + @TenTG + '%'
)
GO

SELECT * FROM dbo.fn_TimSachTheoTacGia(N'Nguyễn Nhật Ánh')
go

--Hàm tìm kiếm sách theo thể loại
CREATE FUNCTION fn_TimSachTheoTheLoai (@TenTL NVARCHAR(100))
RETURNS TABLE
AS
RETURN (
    SELECT s.MaSach, s.TenSach, tg.TenTG, tl.TenTL, s.NamXuatBan, s.GiaBia, s.SoLuongTon
    FROM Sach s
    JOIN TacGia tg ON s.MaTG = tg.MaTG
    JOIN TheLoai tl ON s.MaTL = tl.MaTL
    WHERE tl.TenTL LIKE '%' + @TenTL + '%'
)
GO

SELECT * FROM dbo.fn_TimSachTheoTheLoai(N'Tiểu thuyết')
go

--Hàm tìm kiếm sách sắp hết hàng
CREATE FUNCTION fn_LietKeSachSapHet (@NguongSoLuong INT)
RETURNS TABLE
AS
RETURN (
    SELECT MaSach, TenSach, SoLuongTon
    FROM Sach
    WHERE SoLuongTon < @NguongSoLuong
)
GO

SELECT * FROM dbo.fn_LietKeSachSapHet(30)

go
--Hàm tìm kiếm sách bán chạy trong khoảng thời gian
CREATE FUNCTION fn_TopSachBanChay (
    @TopN INT, 
    @TuNgay DATE, 
    @DenNgay DATE
)
RETURNS TABLE
AS
RETURN (
    SELECT TOP (@TopN)
        s.MaSach,
        s.TenSach,
        SUM(cthd.SoLuong) AS TongSoLuongBan
    FROM ChiTietHoaDon cthd
    JOIN HoaDon hd ON cthd.SoHD = hd.SoHD
    JOIN Sach s ON cthd.MaSach = s.MaSach
    WHERE hd.NgayLap BETWEEN @TuNgay AND @DenNgay
    GROUP BY s.MaSach, s.TenSach
    ORDER BY TongSoLuongBan DESC
)
GO

SELECT * FROM dbo.fn_TopSachBanChay(5, '2024-06-01', '2024-06-30')


---------------------------------------------------------------------------------------------------
-- CURSOR (CON TRỎ):

-- Cập nhật tổng tiền cho hóa đơn bằng Cursor
DECLARE @SoHD VARCHAR(10);
DECLARE @TongTien DECIMAL(18,2);

DECLARE cur_TinhTongTien CURSOR FOR
    SELECT SoHD FROM HoaDon;

OPEN cur_TinhTongTien;

FETCH NEXT FROM cur_TinhTongTien INTO @SoHD;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Tính tổng tiền hóa đơn hiện tại
    SELECT @TongTien = SUM(SoLuong * DonGia * (1 - GiamGia/100.0))
    FROM ChiTietHoaDon
    WHERE SoHD = @SoHD;

    -- Nếu không có chi tiết thì gán = 0
    IF @TongTien IS NULL
        SET @TongTien = 0;

    -- Cập nhật lại hóa đơn
    UPDATE HoaDon
    SET TongTien = @TongTien
    WHERE SoHD = @SoHD;

    -- Sang hóa đơn tiếp theo
    FETCH NEXT FROM cur_TinhTongTien INTO @SoHD;
END;

CLOSE cur_TinhTongTien;
DEALLOCATE cur_TinhTongTien;

-- Xem kết quả
SELECT * FROM HoaDon;



-- Cập nhật tồn kho sau bán
-- Cursor duyệt chi tiết hóa đơn để trừ số lượng tồn
DECLARE @MaSach VARCHAR(10), @SoLuong INT;

DECLARE cur_UpdateTonKho CURSOR FOR
    SELECT MaSach, SoLuong
    FROM ChiTietHoaDon;

OPEN cur_UpdateTonKho;

FETCH NEXT FROM cur_UpdateTonKho INTO @MaSach, @SoLuong;

WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE Sach
    SET SoLuongTon = SoLuongTon - @SoLuong
    WHERE MaSach = @MaSach;

    FETCH NEXT FROM cur_UpdateTonKho INTO @MaSach, @SoLuong;
END;

CLOSE cur_UpdateTonKho;
DEALLOCATE cur_UpdateTonKho;

-- Kiểm tra kết quả
SELECT * FROM Sach;
go

---------------------------------------------------------------------------------------------------
-- BẪY SỰ KIỆN (TRIGGER):

--Trigger cập nhật số lượng tồn kho khi bán sách
CREATE TRIGGER tg_CapNhatSoLuongTon_KhiBanSach
ON ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Tắt thông báo "x rows affected" để tránh nhiễu
    SET NOCOUNT ON;

    DECLARE @MaSach VARCHAR(10);
    DECLARE @SoLuongThayDoi INT;

    -- Trường hợp INSERT hoặc UPDATE (Tính toán số lượng thay đổi)
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        -- Tính toán sự chênh lệch số lượng cho từng mã sách
        SELECT 
            i.MaSach, 
            ISNULL(i.SoLuong, 0) - ISNULL(d.SoLuong, 0) AS SoLuongThayDoi
        INTO #TempChanges
        FROM inserted i
        LEFT JOIN deleted d ON i.MaSach = d.MaSach AND i.SoHD = d.SoHD;

        -- Kiểm tra số lượng tồn kho trước khi trừ
        IF EXISTS (
            SELECT 1
            FROM Sach s
            JOIN #TempChanges t ON s.MaSach = t.MaSach
            WHERE s.SoLuongTon < t.SoLuongThayDoi
        )
        BEGIN
            -- Nếu không đủ hàng, hủy giao dịch và báo lỗi
            RAISERROR(N'Không đủ số lượng sách trong kho để thực hiện giao dịch!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Cập nhật giảm số lượng tồn kho
        UPDATE Sach
        SET SoLuongTon = SoLuongTon - t.SoLuongThayDoi
        FROM Sach s
        JOIN #TempChanges t ON s.MaSach = t.MaSach;

        DROP TABLE #TempChanges;
    END
    -- Trường hợp DELETE (khách trả hàng)
    ELSE IF EXISTS (SELECT * FROM deleted)
    BEGIN
        -- Cập nhật tăng lại số lượng tồn kho
        UPDATE Sach
        SET SoLuongTon = SoLuongTon + d.SoLuong
        FROM Sach s
        JOIN deleted d ON s.MaSach = d.MaSach;
    END
END;
GO

--Trigger cập nhật số lượng tồn kho khi nhập sách
CREATE TRIGGER tg_CapNhatSoLuongTon_KhiNhapSach
ON ChiTietPhieuNhap
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật cho trường hợp INSERT và UPDATE
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        UPDATE Sach
        SET SoLuongTon = SoLuongTon + (ISNULL(i.SoLuongNhap, 0) - ISNULL(d.SoLuongNhap, 0))
        FROM Sach s
        JOIN inserted i ON s.MaSach = i.MaSach
        LEFT JOIN deleted d ON i.MaSach = d.MaSach AND i.SoPN = d.SoPN;
    END
    -- Cập nhật cho trường hợp DELETE
    ELSE IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE Sach
        SET SoLuongTon = SoLuongTon - d.SoLuongNhap
        FROM Sach s
        JOIN deleted d ON s.MaSach = d.MaSach;
    END
END;
GO

--Trigger cập nhật thông tin hóa đơn
CREATE TRIGGER tg_CapNhatTongTienHoaDon
ON ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SoHD VARCHAR(10);

    -- Lấy SoHD từ cả hai bảng inserted và deleted để xử lý cho cả 3 trường hợp
    SELECT @SoHD = ISNULL(i.SoHD, d.SoHD)
    FROM inserted i FULL OUTER JOIN deleted d ON i.SoHD = d.SoHD;

    -- Tính toán lại tổng tiền cho hóa đơn bị ảnh hưởng
    UPDATE HoaDon
    SET TongTien = (
        SELECT SUM(SoLuong * DonGia * (1 - GiamGia / 100.0))
        FROM ChiTietHoaDon
        WHERE SoHD = @SoHD
    )
    WHERE SoHD = @SoHD;
END;
GO

--Trigger ngăn việc xóa sách
CREATE TRIGGER tg_NganChanXoa_Sach
ON Sach
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSach VARCHAR(10);
    SELECT @MaSach = MaSach FROM deleted;

    -- Kiểm tra xem sách có tồn tại trong chi tiết hóa đơn hoặc phiếu nhập không
    IF EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaSach = @MaSach) OR
       EXISTS (SELECT 1 FROM ChiTietPhieuNhap WHERE MaSach = @MaSach)
    BEGIN
        RAISERROR(N'Không thể xóa sách này vì đã có lịch sử giao dịch (hóa đơn hoặc phiếu nhập).', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        -- Nếu không có giao dịch, tiến hành xóa thật
        DELETE FROM Sach WHERE MaSach = @MaSach;
    END
END;
GO

-- Trigger cho Tác Giả
CREATE TRIGGER tg_NganChanXoa_TacGia
ON TacGia
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Sach WHERE MaTG IN (SELECT MaTG FROM deleted))
    BEGIN
        RAISERROR (N'Không thể xóa tác giả này vì vẫn còn sách của tác giả trong hệ thống.', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        DELETE FROM TacGia WHERE MaTG IN (SELECT MaTG FROM deleted);
    END
END;
GO

-- Trigger cho Thể Loại
CREATE TRIGGER tg_NganChanXoa_TheLoai
ON TheLoai
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Sach WHERE MaTL IN (SELECT MaTL FROM deleted))
    BEGIN
        RAISERROR (N'Không thể xóa thể loại này vì vẫn còn sách thuộc thể loại này trong hệ thống.', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        DELETE FROM TheLoai WHERE MaTL IN (SELECT MaTL FROM deleted);
    END
END;
GO

-- Trigger cho Nhà Xuất Bản
CREATE TRIGGER tg_NganChanXoa_NhaXuatBan
ON NhaXuatBan
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Sach WHERE MaNXB IN (SELECT MaNXB FROM deleted))
    BEGIN
        RAISERROR (N'Không thể xóa nhà xuất bản này vì vẫn còn sách của nhà xuất bản trong hệ thống.', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        DELETE FROM NhaXuatBan WHERE MaNXB IN (SELECT MaNXB FROM deleted);
    END
END;
GO

-- Trigger tự tạo mã Khách hàng
CREATE TRIGGER TRG_AutoMaKH
ON KhachHang
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextID INT;
    DECLARE @NewMaKH VARCHAR(10);

    -- Lấy số lớn nhất từ MaKH hiện có
    SELECT @NextID = ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, LEN(MaKH)) AS INT)), 0) + 1
    FROM KhachHang;

    -- Sinh mã dạng KH01, KH02...
    SET @NewMaKH = 'KH' + RIGHT('00' + CAST(@NextID AS VARCHAR(2)), 2);

    -- Insert bản ghi mới với MaKH Auto
    INSERT INTO KhachHang (MaKH, TenKH, DienThoai, Email, DiaChi)
    SELECT 
        @NewMaKH,
        TenKH,
        DienThoai,
        Email,
        DiaChi
    FROM INSERTED;
END;
go

--TEST TRIGGER
SET NOCOUNT ON; 

-- VÍ DỤ 1: KIỂM TRA tg_CapNhatSoLuongTon_KhiBanSach (CẬP NHẬT THÀNH CÔNG)
-- Cập nhật số lượng sách trong hóa đơn và xem số lượng tồn có giảm tương ứng không.
-- Trạng thái ban đầu
PRINT 'Trạng thái ban đầu của Sách S01 (Tôi thấy hoa vàng...):';
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S01';
PRINT 'Trạng thái ban đầu của Hóa đơn HD01: Sách S01 có số lượng là 2.';
SELECT MaSach, SoHD, SoLuong FROM ChiTietHoaDon WHERE MaSach = 'S01' AND SoHD = 'HD01';

-- Thực thi
UPDATE ChiTietHoaDon
SET SoLuong = 5
WHERE MaSach = 'S01' AND SoHD = 'HD01';

-- Kiểm tra kết quả
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S01';


-- VÍ DỤ 2: KIỂM TRA tg_CapNhatSoLuongTon_KhiBanSach (CẬP NHẬT THẤT BẠI - QUÁ TỒN KHO)
-- Cố gắng bán vượt số lượng tồn và kiểm tra trigger có báo lỗi không.
-- Trạng thái ban đầu
PRINT 'Trạng thái ban đầu của Sách S05 (Rừng Na Uy):';
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S05';

-- Thực thi (sử dụng TRY...CATCH để bắt lỗi mà không dừng script)
PRINT CHAR(13) + 'Hành động: Cố gắng cập nhật số lượng Sách S05 trong HD04 lên 30 (kho chỉ có 25).';
BEGIN TRY
    UPDATE ChiTietHoaDon
    SET SoLuong = 30
    WHERE MaSach = 'S05' AND SoHD = 'HD04';
END TRY
BEGIN CATCH
    PRINT '=> LỖI ĐÃ ĐƯỢC BẮT ĐÚNG NHƯ MONG ĐỢI:';
    PRINT '   ' + ERROR_MESSAGE();
END CATCH

-- Kiểm tra kết quả
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S05';


-- VÍ DỤ 3: KIỂM TRA tg_CapNhatSoLuongTon_KhiNhapSach (CẬP NHẬT THÀNH CÔNG)
-- Cập nhật số lượng nhập kho và xem số lượng tồn có tăng tương ứng không.
-- Trạng thái ban đầu
PRINT 'Trạng thái ban đầu của Sách S04 (Nhà giả kim):';
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S04';

-- Thực thi
UPDATE ChiTietPhieuNhap
SET SoLuongNhap = 15
WHERE MaSach = 'S04' AND SoPN = 'PN03';

-- Kiểm tra kết quả
SELECT MaSach, TenSach, SoLuongTon FROM Sach WHERE MaSach = 'S04';


-- VÍ DỤ 4: KIỂM TRA tg_CapNhatTongTienHoaDon
-- Mục đích: Cập nhật chi tiết hóa đơn và xem tổng tiền có tự động tính lại không.
-- Trạng thái ban đầu (Trigger đã chạy khi insert dữ liệu gốc, tổng tiền là 208500.00)
PRINT 'Tổng tiền ban đầu của Hóa đơn HD01:';
SELECT SoHD, TongTien FROM HoaDon WHERE SoHD = 'HD01';

-- Thực thi
UPDATE ChiTietHoaDon
SET SoLuong = 3
WHERE MaSach = 'S01' AND SoHD = 'HD01';

-- Kiểm tra kết quả
SELECT SoHD, TongTien FROM HoaDon WHERE SoHD = 'HD01';

-- VÍ DỤ 5: KIỂM TRA tg_NganChanXoa_Sach
-- Cố gắng xóa sách đã có giao dịch và kiểm tra trigger có ngăn chặn không.
-- Thực thi
PRINT 'Hành động: Cố gắng xóa Sách S01 (đã tồn tại trong HD01 và PN01).';
BEGIN TRY
    DELETE FROM Sach WHERE MaSach = 'S01';
END TRY
BEGIN CATCH
    PRINT '=> LỖI ĐÃ ĐƯỢC BẮT ĐÚNG NHƯ MONG ĐỢI:';
    PRINT '   ' + ERROR_MESSAGE();
END CATCH

-- Kiểm tra kết quả
SELECT MaSach, TenSach FROM Sach WHERE MaSach = 'S01';


-- VÍ DỤ 6: KIỂM TRA tg_NganChanXoa_TacGia
-- Cố gắng xóa tác giả còn sách và kiểm tra trigger có ngăn chặn không.
-- Thực thi
PRINT 'Hành động: Cố gắng xóa Tác giả TG01 - Nguyễn Nhật Ánh (còn sách S01, S02).';
BEGIN TRY
    DELETE FROM TacGia WHERE MaTG = 'TG01';
END TRY
BEGIN CATCH
    PRINT '=> LỖI ĐÃ ĐƯỢC BẮT ĐÚNG NHƯ MONG ĐỢI:';
    PRINT '   ' + ERROR_MESSAGE();
END CATCH

-- Kiểm tra kết quả
SELECT MaTG, TenTG FROM TacGia WHERE MaTG = 'TG01';
go

---------------------------------------------------------------------------------------------------
-- STORED PROCEDURE (THỦ TỤC LƯU TRỮ):

-- Đăng ký tài khoản khách--
CREATE PROCEDURE sp_DangKyTaiKhoanKhach
@MaKH VARCHAR(10),
@TenDangNhap VARCHAR(50),
@MatKhau NVARCHAR(200),
@VaiTro NVARCHAR(20) 
AS
BEGIN
	IF  EXISTS ( SELECT 1
				 FROM TaiKhoan
				 WHERE MaKH = @MaKH OR TenDangNhap = @TenDangNhap
				 )
				 BEGIN
					PRINT N'Tài khoản này đã tồn tại!!';
				 END
	ELSE 
	BEGIN
		INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaKH, NgayTao)
		VALUES (@MaKH, @TenDangNhap, @MatKhau, @VaiTro, GETDATE());
		PRINT N'Đăng ký tài khoản thành công!!';
	END
END;
--test--
EXEC sp_DangKyTaiKhoanKhach 
    @MaKH = 'KH01', 
    @TenDangNhap = 'nam_kh', 
    @MatKhau = '123456', 
    @VaiTro = N'KhachHang';
go

-- Cập nhật thông tin khách hàng--
CREATE PROCEDURE sp_UpdateTaikhoan
@MaKH VARCHAR (10),
@TenDangNhap VARCHAR(50),
@MatKhau NVARCHAR(200),
@VaiTro NVARCHAR(20)  
AS
BEGIN 
SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM TaiKhoan WHERE MaKH = @MaKH)
	BEGIN
		UPDATE TaiKhoan
		SET TenDangNhap = @TenDangNhap,
			MatKhau = @MatKhau,
			VaiTro = @VaiTro
		WHERE MaKH = @MaKH;

		PRINT N'Cập nhật thông tin thành công';

	END
	ELSE
		BEGIN
			PRINT N'Mã khách hàng không tồn tại';
		END
END;
--TEST--
EXEC sp_UpdateTaikhoan
    @MaKH = 'KH01', 
    @TenDangNhap = 'nguyen_kh', 
    @MatKhau = '1234567', 
    @VaiTro = N'KhachHang'; 
go

-- Tìm kiếm khách hàng--
CREATE PROCEDURE sp_findTaiKhoan
@MaKH VARCHAR (10)
AS
BEGIN 
	IF EXISTS (SELECT 1 FROM TaiKhoan WHERE MaKH = @MaKH)
	BEGIN
		SELECT tk.MaKH, tk.TenDangNhap, tk.TrangThai, tk.VaiTro, tk.NgayTao
		FROM TaiKhoan tk
		JOIN KhachHang kh ON tk.MaKH = kh.MaKH
		WHERE @MaKH = tk.MaKH
	END
	ELSE
		BEGIN
			PRINT N'Mã khách hàng không tồn tại';
		END
END;
--TEST--
EXEC sp_findTaiKhoan
    @MaKH = 'KH01';
go

-- Thống kê thu nhập tháng--
CREATE PROCEDURE sp_sumDoanhThu
AS
BEGIN 
	SELECT 
		YEAR (hd.NgayLap) AS NAM,
		MONTH (hd.NgayLap) AS THANG,
		SUM (ct.SoLuong * ct.DonGia * (1 - ct.GiamGia/100 )) AS TONGDOANHTHUTHANG
	FROM HoaDon hd
	JOIN ChiTietHoaDon ct ON ct.SoHD = hd.SoHD
	GROUP BY YEAR(hd.NgayLap), MONTH (hd.NgayLap)
	ORDER BY NAM, THANG;
END;

--TEST
EXEC sp_sumDoanhThu
