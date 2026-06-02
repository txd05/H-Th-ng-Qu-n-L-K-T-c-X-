using System.ComponentModel.DataAnnotations;

namespace KyTucXaManagement.Models
{
    public class TheKTX
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã thẻ")]
        public string MaThe { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày cấp")]
        public DateTime NgayCap { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn")]
        public DateTime NgayHetHan { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Còn hiệu lực";

        // Khóa ngoại
        public int SinhVienId { get; set; }
        public SinhVien? SinhVien { get; set; }
    }
}
