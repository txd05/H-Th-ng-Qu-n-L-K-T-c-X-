using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KyTucXaManagement.Models
{
    public class ChiSoNuoc
    {
        public int Id { get; set; }

        [Display(Name = "Tháng")]
        public int Thang { get; set; }

        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Chỉ số đầu")]
        public decimal ChiSoDau { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Chỉ số cuối")]
        public decimal ChiSoCuoi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiêu thụ (m³)")]
        public decimal TieuThu => ChiSoCuoi - ChiSoDau;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá (đ/m³)")]
        public decimal DonGia { get; set; } = 15000;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien => TieuThu * DonGia;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày ghi")]
        public DateTime NgayGhi { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Khóa ngoại
        public int PhongId { get; set; }
        public Phong? Phong { get; set; }
    }
}
