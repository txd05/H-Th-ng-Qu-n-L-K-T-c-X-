using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KyTucXaManagement.Models
{
    public class Phong
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã phòng không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã phòng")]
        public string MaPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại phòng không được để trống")]
        [Display(Name = "Loại phòng")]
        public string LoaiPhong { get; set; } = "4 người";

        [Required]
        [Range(1, 20)]
        [Display(Name = "Sức chứa")]
        public int SucChua { get; set; }

        [Display(Name = "Số người hiện tại")]
        public int SoNguoiHienTai { get; set; } = 0;

        [StringLength(10)]
        [Display(Name = "Tầng")]
        public string? Tang { get; set; }

        [StringLength(50)]
        [Display(Name = "Tòa nhà")]
        public string? ToaNha { get; set; }

        [StringLength(20)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Còn chỗ";

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá phòng/tháng")]
        public decimal GiaPhong { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        // Navigation properties
        public ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
        public ICollection<ChiSoDien> ChiSoDiens { get; set; } = new List<ChiSoDien>();
        public ICollection<ChiSoNuoc> ChiSoNuocs { get; set; } = new List<ChiSoNuoc>();
        public ICollection<TaiSan> TaiSans { get; set; } = new List<TaiSan>();
    }
}
