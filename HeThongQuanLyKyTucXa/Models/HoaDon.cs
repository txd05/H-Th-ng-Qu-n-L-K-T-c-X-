using System.ComponentModel.DataAnnotations;

namespace KyTucXaManagement.Models
{
    public class HoaDon
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Display(Name = "Mã hóa đơn")]
        public string MaHoaDon { get; set; } = string.Empty;

        [Display(Name = "Tháng")]
        public int Thang { get; set; }

        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Display(Name = "Tiền phòng")]
        public double TienPhong { get; set; }

        [Display(Name = "Tiền điện")]
        public double TienDien { get; set; }

        [Display(Name = "Tiền nước")]
        public double TienNuoc { get; set; }

        [Display(Name = "Phí khác")]
        public double PhiKhac { get; set; }

        // Tổng tiền — computed, không lưu DB (được ignore trong DbContext)
        [Display(Name = "Tổng tiền")]
        public double TongTien => TienPhong + TienDien + TienNuoc + PhiKhac;

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chưa thanh toán";

        [DataType(DataType.Date)]
        [Display(Name = "Ngày lập")]
        public DateTime NgayLap { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Hạn thanh toán")]
        public DateTime? HanThanhToan { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime? NgayThanhToan { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Khóa ngoại
        public int SinhVienId { get; set; }
        public SinhVien? SinhVien { get; set; }

        public int? PhongId { get; set; }
        public Phong? Phong { get; set; }
    }
}
