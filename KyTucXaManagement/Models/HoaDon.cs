using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền phòng")]
        public decimal TienPhong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền điện")]
        public decimal TienDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền nước")]
        public decimal TienNuoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Phí khác")]
        public decimal PhiKhac { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien => TienPhong + TienDien + TienNuoc + PhiKhac;

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
