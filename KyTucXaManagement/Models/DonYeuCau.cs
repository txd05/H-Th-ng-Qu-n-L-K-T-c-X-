using System.ComponentModel.DataAnnotations;

namespace KyTucXaManagement.Models
{
    public class DonYeuCau
    {
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name = "Mã đơn")]
        public string MaDon { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại đơn")]
        public string LoaiDon { get; set; } = "Đăng ký";

        [DataType(DataType.Date)]
        [Display(Name = "Ngày nộp")]
        public DateTime NgayNop { get; set; } = DateTime.Now;

        [StringLength(1000)]
        [Display(Name = "Nội dung")]
        public string? NoiDung { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ duyệt";

        [StringLength(500)]
        [Display(Name = "Lý do từ chối")]
        public string? LyDoTuChoi { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày duyệt")]
        public DateTime? NgayDuyet { get; set; }

        [StringLength(100)]
        [Display(Name = "Người duyệt")]
        public string? NguoiDuyet { get; set; }

        // Khóa ngoại
        public int SinhVienId { get; set; }
        public SinhVien? SinhVien { get; set; }

        public int? PhongId { get; set; }
        public Phong? Phong { get; set; }
    }
}
