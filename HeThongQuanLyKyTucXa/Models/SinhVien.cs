using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KyTucXaManagement.Models
{
    public class SinhVien
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã sinh viên")]
        public string MaSinhVien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(15)]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(50)]
        [Display(Name = "Quê quán")]
        public string? QueQuan { get; set; }

        [StringLength(50)]
        [Display(Name = "Diện ưu tiên")]
        public string? DienUuTien { get; set; }

        [StringLength(20)]
        [Display(Name = "Năm học")]
        public string? NamHoc { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang nội trú";

        [Display(Name = "Ngày vào KTX")]
        [DataType(DataType.Date)]
        public DateTime? NgayVaoKTX { get; set; }

        // Liên kết với tài khoản Identity
        public string? UserId { get; set; }

        // Khóa ngoại phòng
        public int? PhongId { get; set; }
        public Phong? Phong { get; set; }

        // Navigation properties
        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public ICollection<DonYeuCau> DonYeuCaus { get; set; } = new List<DonYeuCau>();
        public TheKTX? TheKTX { get; set; }
    }
}
