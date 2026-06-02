using System.ComponentModel.DataAnnotations;

namespace KyTucXaManagement.Models
{
    public class ChiSoNuoc
    {
        public int Id { get; set; }

        [Display(Name = "Tháng")]
        public int Thang { get; set; }

        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Display(Name = "Chỉ số đầu")]
        public double ChiSoDau { get; set; }

        [Display(Name = "Chỉ số cuối")]
        public double ChiSoCuoi { get; set; }

        // Computed — ignored in DB
        [Display(Name = "Tiêu thụ (m³)")]
        public double TieuThu => ChiSoCuoi - ChiSoDau;

        [Display(Name = "Đơn giá (đ/m³)")]
        public double DonGia { get; set; } = 15000;

        // Computed — ignored in DB
        [Display(Name = "Thành tiền")]
        public double ThanhTien => TieuThu * DonGia;

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
