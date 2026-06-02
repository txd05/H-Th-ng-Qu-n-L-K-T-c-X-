using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KyTucXaManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaPhong = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LoaiPhong = table.Column<string>(type: "TEXT", nullable: false),
                    SucChua = table.Column<int>(type: "INTEGER", nullable: false),
                    SoNguoiHienTai = table.Column<int>(type: "INTEGER", nullable: false),
                    Tang = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ToaNha = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    GioiTinh = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    GiaPhong = table.Column<double>(type: "REAL", nullable: false),
                    MoTa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoDiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Thang = table.Column<int>(type: "INTEGER", nullable: false),
                    Nam = table.Column<int>(type: "INTEGER", nullable: false),
                    ChiSoDau = table.Column<double>(type: "REAL", nullable: false),
                    ChiSoCuoi = table.Column<double>(type: "REAL", nullable: false),
                    DonGia = table.Column<double>(type: "REAL", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoDiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiSoDiens_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoNuocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Thang = table.Column<int>(type: "INTEGER", nullable: false),
                    Nam = table.Column<int>(type: "INTEGER", nullable: false),
                    ChiSoDau = table.Column<double>(type: "REAL", nullable: false),
                    ChiSoCuoi = table.Column<double>(type: "REAL", nullable: false),
                    DonGia = table.Column<double>(type: "REAL", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoNuocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiSoNuocs_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaSinhVien = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GioiTinh = table.Column<string>(type: "TEXT", nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DiaChi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    QueQuan = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DienUuTien = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NamHoc = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    NgayVaoKTX = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinhViens_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaiSans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaTaiSan = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TenTaiSan = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LoaiTaiSan = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SoLuong = table.Column<int>(type: "INTEGER", nullable: false),
                    TinhTrang = table.Column<string>(type: "TEXT", nullable: false),
                    GiaTri = table.Column<double>(type: "REAL", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MoTa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiSans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiSans_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonYeuCaus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaDon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LoaiDon = table.Column<string>(type: "TEXT", nullable: false),
                    NgayNop = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoiDung = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    LyDoTuChoi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NguoiDuyet = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SinhVienId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonYeuCaus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonYeuCaus_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DonYeuCaus_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaHoaDon = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Thang = table.Column<int>(type: "INTEGER", nullable: false),
                    Nam = table.Column<int>(type: "INTEGER", nullable: false),
                    TienPhong = table.Column<double>(type: "REAL", nullable: false),
                    TienDien = table.Column<double>(type: "REAL", nullable: false),
                    TienNuoc = table.Column<double>(type: "REAL", nullable: false),
                    PhiKhac = table.Column<double>(type: "REAL", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HanThanhToan = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SinhVienId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhongId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDons_Phongs_PhongId",
                        column: x => x.PhongId,
                        principalTable: "Phongs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoaDons_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TheKTXs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaThe = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NgayCap = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    SinhVienId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheKTXs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheKTXs_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuCoTaiSans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MoTaSuCo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MucDo = table.Column<string>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    NgayBao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgaySua = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChiPhiSua = table.Column<double>(type: "REAL", nullable: true),
                    NguoiSua = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TaiSanId = table.Column<int>(type: "INTEGER", nullable: false),
                    SinhVienBaoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuCoTaiSans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuCoTaiSans_SinhViens_SinhVienBaoId",
                        column: x => x.SinhVienBaoId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SuCoTaiSans_TaiSans_TaiSanId",
                        column: x => x.TaiSanId,
                        principalTable: "TaiSans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoDiens_PhongId",
                table: "ChiSoDiens",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoNuocs_PhongId",
                table: "ChiSoNuocs",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_DonYeuCaus_PhongId",
                table: "DonYeuCaus",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_DonYeuCaus_SinhVienId",
                table: "DonYeuCaus",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_PhongId",
                table: "HoaDons",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_SinhVienId",
                table: "HoaDons",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_MaPhong",
                table: "Phongs",
                column: "MaPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaSinhVien",
                table: "SinhViens",
                column: "MaSinhVien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_PhongId",
                table: "SinhViens",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_SuCoTaiSans_SinhVienBaoId",
                table: "SuCoTaiSans",
                column: "SinhVienBaoId");

            migrationBuilder.CreateIndex(
                name: "IX_SuCoTaiSans_TaiSanId",
                table: "SuCoTaiSans",
                column: "TaiSanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiSans_PhongId",
                table: "TaiSans",
                column: "PhongId");

            migrationBuilder.CreateIndex(
                name: "IX_TheKTXs_SinhVienId",
                table: "TheKTXs",
                column: "SinhVienId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiSoDiens");

            migrationBuilder.DropTable(
                name: "ChiSoNuocs");

            migrationBuilder.DropTable(
                name: "DonYeuCaus");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "SuCoTaiSans");

            migrationBuilder.DropTable(
                name: "TheKTXs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TaiSans");

            migrationBuilder.DropTable(
                name: "SinhViens");

            migrationBuilder.DropTable(
                name: "Phongs");
        }
    }
}
