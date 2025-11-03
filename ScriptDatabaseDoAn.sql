CREATE DATABASE QuanLyQuanCafe;
GO

USE QuanLyQuanCafe;
GO

CREATE TABLE LoaiMon (
    MaLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Mon (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(150) NOT NULL,
    DonGia BIGINT DEFAULT 0,
    MaLoai INT NOT NULL,
	DaXoa BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (MaLoai) REFERENCES LoaiMon(MaLoai)
);
GO

CREATE TABLE NhanVien (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(150) NOT NULL,
    NgaySinh DATE,
    DienThoai VARCHAR(20),
    DiaChi NVARCHAR(200)
);
GO

CREATE TABLE Ban (
    MaBan INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Trống' -- Trống | Có khách
);
GO

CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),
	TrangThai BIT NOT NULL DEFAULT 0,
    TongTien BIGINT DEFAULT 0,
	MaBan INT,
    FOREIGN KEY (MaBan) REFERENCES Ban(MaBan)
);
GO

CREATE TABLE ChiTietHoaDon (
    MaHD INT NOT NULL,
    MaMon INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia BIGINT,
    PRIMARY KEY (MaHD, MaMon),
    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    FOREIGN KEY (MaMon) REFERENCES Mon(MaMon)
);
GO

CREATE TRIGGER trg_UpdateTongTien
ON ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE HoaDon
    SET TongTien =
        (SELECT SUM(SoLuong * DonGia)
         FROM ChiTietHoaDon
         WHERE ChiTietHoaDon.MaHD = HoaDon.MaHD)
    WHERE HoaDon.MaHD IN (
        SELECT MaHD FROM inserted
        UNION
        SELECT MaHD FROM deleted
    );
END;
GO

INSERT INTO LoaiMon (TenLoai) VALUES 
(N'Cà phê'), 
(N'Trà sữa'), 
(N'Nước ép'),
(N'Trà trái cây'),
(N'Sinh tố'),
(N'Khác');	


INSERT INTO Mon (TenMon, DonGia, MaLoai) VALUES
(N'Cà phê đen', 15000, 1),
(N'Cà phê sữa', 20000, 1),
(N'Trà sữa trân châu', 25000, 2),
(N'Nước cam', 20000, 3),
(N'Trà Chanh', 15000, 4),
(N'Sinh tố dâu', 25000, 5),
(N'Sinh tố bơ', 30000, 5),
(N'Bánh tiramisu', 25000, 6);

INSERT INTO NhanVien (HoTen, NgaySinh, DienThoai, DiaChi) VALUES
(N'Nguyễn Văn A', '2000-05-10', '0909123456', N'443/6, Đỗ Xuân Hợp, Thủ Đức'),
(N'Lê Thị B', '2001-07-15', '0909345678', N'b2/3, đường 385, tăng nhơn phú, thủ đức');

INSERT INTO Ban (TenBan) VALUES
(N'Bàn 1'), (N'Bàn 2'), (N'Bàn 3'), (N'Bàn 4'), (N'Bàn 5'), (N'Bàn 6'),
(N'Bàn 7'), (N'Bàn 8'), (N'Bàn 9'), (N'Bàn 10'), (N'Bàn 11'), (N'Bàn 12');
GO

