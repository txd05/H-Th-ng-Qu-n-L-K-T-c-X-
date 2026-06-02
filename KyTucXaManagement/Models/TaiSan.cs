using System.ComponentModel.DataAnnotations;

namespace KyTucXaManagement.Models
{
    public class TaiSan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã tài sản")]
        public string MaTaiSan { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên tài sản")]
        public string TenTaiSan { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Loại tài sản")]
        public string? LoaiTaiSan { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; } = 1;

        [Display(Name = "Tình trạng")]
        public string TinhTrang { get; set; } = "Bình thường";

        [Display(Name = "Giá trị")]
        public double GiaTri { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhập")]
        public DateTime? NgayNhap { get; set; }

        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        // Khóa ngoại
        public int PhongId { get; set; }
        public Phong? Phong { get; set; }

        // Navigation
        public ICollection<SuCoTaiSan> SuCoTaiSans { get; set; } = new List<SuCoTaiSan>();
    }

    public class SuCoTaiSan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Mô tả sự cố")]
        public string MoTaSuCo { get; set; } = string.Empty;

        [Display(Name = "Mức độ")]
        public string MucDo { get; set; } = "Nhẹ";

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ xử lý";

        [DataType(DataType.Date)]
        [Display(Name = "Ngày báo")]
        public DateTime NgayBao { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sửa")]
        public DateTime? NgaySua { get; set; }

        [Display(Name = "Chi phí sửa")]
        public double? ChiPhiSua { get; set; }

        [StringLength(200)]
        [Display(Name = "Người sửa")]
        public string? NguoiSua { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Khóa ngoại
        public int TaiSanId { get; set; }
        public TaiSan? TaiSan { get; set; }

        public int? SinhVienBaoId { get; set; }
        public SinhVien? SinhVienBao { get; set; }
    }
}
