using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lab06_QuanLySanPham
{
    public partial class FrmProduct : Form
    {
        // Quản lý dữ liệu trong bộ nhớ (In-Memory)
        private readonly BindingList<Product> products = new BindingList<Product>();
        private readonly BindingSource productSource = new BindingSource();

        public FrmProduct()
        {
            InitializeComponent();
        }

        private void FrmProduct_Load(object sender, EventArgs e)
        {
            KhoiTaoDuLieu();
            DinhDangCotDataGridView();
            CapNhatTrangThaiNut();
            UpdateStatus("Sẵn sàng làm việc");
        }

        #region KHỞI TẠO DỮ LIỆU BAN ĐẦU

        private void KhoiTaoDuLieu()
        {
            // Danh mục loại sản phẩm cố định theo đề bài
            cboLoaiSP.Items.Clear();
            cboLoaiSP.Items.AddRange(new object[] { "Điện tử", "Văn phòng", "Gia dụng", "Thời trang" });
            cboLoaiSP.SelectedIndex = 0;

            // Dữ liệu mẫu ban đầu (tối thiểu 3 sản phẩm theo yêu cầu)
            products.Add(new Product { MaSP = "SP01", TenSP = "Bàn phím cơ AKKO 3087", LoaiSP = "Điện tử", DonGia = 1250000m, SoLuong = 15, NgayNhap = DateTime.Now.AddDays(-10), ConKinhDoanh = true });
            products.Add(new Product { MaSP = "SP02", TenSP = "Ghế công thái học Sihoo M57", LoaiSP = "Văn phòng", DonGia = 3450000m, SoLuong = 5, NgayNhap = DateTime.Now.AddDays(-5), ConKinhDoanh = true });
            products.Add(new Product { MaSP = "SP03", TenSP = "Nồi chiên không dầu Philips HD9650", LoaiSP = "Gia dụng", DonGia = 2190000m, SoLuong = 20, NgayNhap = DateTime.Now.AddDays(-2), ConKinhDoanh = true });
            products.Add(new Product { MaSP = "SP04", TenSP = "Áo Polo thể thao nam", LoaiSP = "Thời trang", DonGia = 350000m, SoLuong = 40, NgayNhap = DateTime.Now.AddDays(-1), ConKinhDoanh = true });

            // Liên kết Binding 3 lớp: BindingList -> BindingSource -> DataGridView
            productSource.DataSource = products;
            dgvProducts.DataSource = productSource;

            // Lắng nghe thay đổi con trỏ để cập nhật trạng thái nút
            productSource.CurrentChanged += (s, e) => CapNhatTrangThaiNut();
        }

        private void DinhDangCotDataGridView()
        {
            if (dgvProducts.Columns["MaSP"] != null)
            {
                dgvProducts.Columns["MaSP"].HeaderText = "Mã SP";
                dgvProducts.Columns["MaSP"].Width = 80;
                dgvProducts.Columns["MaSP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dgvProducts.Columns["TenSP"] != null)
            {
                dgvProducts.Columns["TenSP"].HeaderText = "Tên sản phẩm";
            }
            if (dgvProducts.Columns["LoaiSP"] != null)
            {
                dgvProducts.Columns["LoaiSP"].HeaderText = "Loại sản phẩm";
                dgvProducts.Columns["LoaiSP"].Width = 110;
            }
            if (dgvProducts.Columns["DonGia"] != null)
            {
                dgvProducts.Columns["DonGia"].HeaderText = "Đơn giá (VNĐ)";
                dgvProducts.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvProducts.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProducts.Columns["DonGia"].Width = 110;
            }
            if (dgvProducts.Columns["SoLuong"] != null)
            {
                dgvProducts.Columns["SoLuong"].HeaderText = "Số lượng";
                dgvProducts.Columns["SoLuong"].DefaultCellStyle.Format = "N0";
                dgvProducts.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProducts.Columns["SoLuong"].Width = 80;
            }
            if (dgvProducts.Columns["ThanhTien"] != null)
            {
                dgvProducts.Columns["ThanhTien"].HeaderText = "Thành tiền (VNĐ)";
                dgvProducts.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                dgvProducts.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProducts.Columns["ThanhTien"].Width = 120;
            }
            if (dgvProducts.Columns["NgayNhap"] != null)
            {
                dgvProducts.Columns["NgayNhap"].HeaderText = "Ngày nhập";
                dgvProducts.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvProducts.Columns["NgayNhap"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvProducts.Columns["NgayNhap"].Width = 95;
            }
            if (dgvProducts.Columns["ConKinhDoanh"] != null)
            {
                dgvProducts.Columns["ConKinhDoanh"].HeaderText = "Còn KD";
                dgvProducts.Columns["ConKinhDoanh"].Width = 70;
            }
            if (dgvProducts.Columns["DuongDanAnh"] != null)
            {
                dgvProducts.Columns["DuongDanAnh"].Visible = false;
            }
        }

        #endregion

        #region BỐN HÀM CLEAN CODE BẮT BUỘC THEO ĐỀ BÀI

        // 1. Kiểm tra tính hợp lệ dữ liệu đầu vào sử dụng ErrorProvider
        private bool ValidateInput(bool isAddingNew = true)
        {
            errorProvider1.Clear();
            bool isValid = true;

            // Kiểm tra Mã SP: không rỗng, không trùng khi thêm mới
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                errorProvider1.SetError(txtMaSP, "Mã sản phẩm không được để trống!");
                isValid = false;
            }
            else if (isAddingNew && products.Any(p => p.MaSP.Trim().Equals(txtMaSP.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                errorProvider1.SetError(txtMaSP, "Mã sản phẩm đã tồn tại!");
                isValid = false;
            }

            // Kiểm tra Tên SP: không rỗng
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                errorProvider1.SetError(txtTenSP, "Tên sản phẩm không được để trống!");
                isValid = false;
            }

            // Kiểm tra Đơn giá: lớn hơn 0
            if (numDonGia.Value <= 0)
            {
                errorProvider1.SetError(numDonGia, "Đơn giá phải lớn hơn 0 VNĐ!");
                isValid = false;
            }

            // Kiểm tra Số lượng: không được âm (>= 0)
            if (numSoLuong.Value < 0)
            {
                errorProvider1.SetError(numSoLuong, "Số lượng không được âm!");
                isValid = false;
            }

            // Kiểm tra Ngày nhập: không được lớn hơn ngày hiện tại
            if (dtpNgayNhap.Value.Date > DateTime.Now.Date)
            {
                errorProvider1.SetError(dtpNgayNhap, "Ngày nhập không được lớn hơn ngày hiện tại!");
                isValid = false;
            }

            return isValid;
        }

        // 2. Làm sạch các ô nhập trên Form
        private void ClearInput()
        {
            errorProvider1.Clear();
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtMaSP.Enabled = true;
            if (cboLoaiSP.Items.Count > 0) cboLoaiSP.SelectedIndex = 0;
            numDonGia.Value = 0;
            numSoLuong.Value = 0;
            dtpNgayNhap.Value = DateTime.Now;
            chkConKinhDoanh.Checked = true;

            if (picAnh.Image != null)
            {
                picAnh.Image.Dispose();
                picAnh.Image = null;
            }
            picAnh.Tag = null;

            dgvProducts.ClearSelection();
            CapNhatTrangThaiNut();
            txtMaSP.Focus();
        }

        // 3. Đổ dữ liệu từ đối tượng lên Form để xem / sửa
        private void LoadProductToForm(Product p)
        {
            if (p == null) return;

            errorProvider1.Clear();
            txtMaSP.Text = p.MaSP;
            txtMaSP.Enabled = false; // Khóa không cho đổi mã SP khi xem/sửa
            txtTenSP.Text = p.TenSP;
            cboLoaiSP.Text = p.LoaiSP;
            numDonGia.Value = p.DonGia;
            numSoLuong.Value = p.SoLuong;
            dtpNgayNhap.Value = p.NgayNhap;
            chkConKinhDoanh.Checked = p.ConKinhDoanh;

            // Xử lý nạp ảnh an toàn (tránh lock file)
            if (picAnh.Image != null)
            {
                picAnh.Image.Dispose();
                picAnh.Image = null;
            }

            if (!string.IsNullOrEmpty(p.DuongDanAnh) && File.Exists(p.DuongDanAnh))
            {
                try
                {
                    picAnh.SizeMode = PictureBoxSizeMode.Zoom;
                    using (var bmpTemp = new Bitmap(p.DuongDanAnh))
                    {
                        picAnh.Image = new Bitmap(bmpTemp);
                    }
                    picAnh.Tag = p.DuongDanAnh;
                }
                catch
                {
                    picAnh.Image = null;
                    picAnh.Tag = null;
                }
            }
            else
            {
                picAnh.Image = null;
                picAnh.Tag = null;
            }
        }

        // 4. Cập nhật StatusStrip: Trạng thái thao tác, Số lượng sản phẩm & Tổng giá trị tồn kho (Bonus)
        private void UpdateStatus(string thongBao)
        {
            lblStatus.Text = thongBao;
            decimal tongGiaTriTonKho = products.Sum(p => p.ThanhTien);
            int tongSoLuong = products.Sum(p => p.SoLuong);
            lblTongSo.Text = $"Tổng số: {products.Count} SP | Tồn kho: {tongSoLuong:N0} | Tổng giá trị: {tongGiaTriTonKho:N0} VNĐ";
        }

        #endregion

        #region NGHIỆP VỤ CRUD CHÍNH (HÀM XỬ LÝ CHUNG DRY)

        private void ThemSanPham()
        {
            if (!ValidateInput(isAddingNew: true)) return;

            try
            {
                var p = new Product
                {
                    MaSP = txtMaSP.Text.Trim(),
                    TenSP = txtTenSP.Text.Trim(),
                    LoaiSP = cboLoaiSP.Text,
                    DonGia = numDonGia.Value,
                    SoLuong = (int)numSoLuong.Value,
                    NgayNhap = dtpNgayNhap.Value,
                    ConKinhDoanh = chkConKinhDoanh.Checked,
                    DuongDanAnh = picAnh.Tag?.ToString() ?? string.Empty
                };

                products.Add(p);
                ClearInput();
                UpdateStatus($"Đã thêm thành công sản phẩm: {p.TenSP}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SuaSanPham()
        {
            Product selected = LaySanPhamDangChon();
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa trên danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInput(isAddingNew: false)) return;

            try
            {
                selected.TenSP = txtTenSP.Text.Trim();
                selected.LoaiSP = cboLoaiSP.Text;
                selected.DonGia = numDonGia.Value;
                selected.SoLuong = (int)numSoLuong.Value;
                selected.NgayNhap = dtpNgayNhap.Value;
                selected.ConKinhDoanh = chkConKinhDoanh.Checked;
                selected.DuongDanAnh = picAnh.Tag?.ToString() ?? string.Empty;

                productSource.ResetBindings(false);
                dgvProducts.Refresh();
                UpdateStatus($"Đã cập nhật sản phẩm [{selected.MaSP}]");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XoaSanPham()
        {
            Product selected = LaySanPhamDangChon();
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa trên danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dr = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sản phẩm [{selected.MaSP}] - {selected.TenSP}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                products.Remove(selected);
                ClearInput();
                UpdateStatus("Đã xóa sản phẩm thành công");
            }
        }

        private void TimKiemSanPham()
        {
            string keyword = tstxtTimKiem.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                dgvProducts.DataSource = productSource;
                DinhDangCotDataGridView();
                CapNhatTrangThaiNut();
                UpdateStatus("Đã hiển thị toàn bộ danh sách sản phẩm");
                return;
            }

            var ketQua = products.Where(p =>
                p.MaSP.ToLower().Contains(keyword) ||
                p.TenSP.ToLower().Contains(keyword) ||
                p.LoaiSP.ToLower().Contains(keyword)).ToList();

            dgvProducts.DataSource = ketQua;
            DinhDangCotDataGridView();
            CapNhatTrangThaiNut();
            UpdateStatus($"Tìm thấy {ketQua.Count} sản phẩm khớp với từ khóa '{keyword}'");
        }

        private void LamMoiToanBo()
        {
            tstxtTimKiem.Clear();
            dgvProducts.DataSource = productSource;
            DinhDangCotDataGridView();
            ClearInput();
            UpdateStatus("Đã làm mới dữ liệu và hiển thị danh sách đầy đủ");
        }

        private void XemChiTietSanPham()
        {
            Product p = LaySanPhamDangChon();
            if (p == null) return;

            string info = $"Mã sản phẩm: {p.MaSP}\n" +
                          $"Tên sản phẩm: {p.TenSP}\n" +
                          $"Loại sản phẩm: {p.LoaiSP}\n" +
                          $"Đơn giá: {p.DonGia:N0} VNĐ\n" +
                          $"Số lượng: {p.SoLuong:N0}\n" +
                          $"Thành tiền: {p.ThanhTien:N0} VNĐ\n" +
                          $"Ngày nhập: {p.NgayNhap:dd/MM/yyyy}\n" +
                          $"Trạng thái: {(p.ConKinhDoanh ? "Đang kinh doanh" : "Ngừng kinh doanh")}\n" +
                          $"Đường dẫn ảnh: {(string.IsNullOrEmpty(p.DuongDanAnh) ? "(Chưa có ảnh)" : p.DuongDanAnh)}";

            MessageBox.Show(info, "Chi tiết sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Product LaySanPhamDangChon()
        {
            if (dgvProducts.CurrentRow != null)
            {
                return dgvProducts.CurrentRow.DataBoundItem as Product;
            }
            return productSource.Current as Product;
        }

        #endregion

        #region CHỨC NĂNG MỞ RỘNG (BONUS): SẮP XẾP & XUẤT FILE TEXT

        // Sắp xếp theo đơn giá (Tăng / Giảm)
        private void SapXepDonGia(bool tangDan)
        {
            var sorted = tangDan
                ? products.OrderBy(p => p.DonGia).ToList()
                : products.OrderByDescending(p => p.DonGia).ToList();

            products.Clear();
            foreach (var item in sorted) products.Add(item);

            UpdateStatus(tangDan ? "Đã sắp xếp danh sách theo đơn giá tăng dần" : "Đã sắp xếp danh sách theo đơn giá giảm dần");
        }

        // Sắp xếp theo số lượng (Tăng / Giảm)
        private void SapXepSoLuong(bool tangDan)
        {
            var sorted = tangDan
                ? products.OrderBy(p => p.SoLuong).ToList()
                : products.OrderByDescending(p => p.SoLuong).ToList();

            products.Clear();
            foreach (var item in sorted) products.Add(item);

            UpdateStatus(tangDan ? "Đã sắp xếp danh sách theo số lượng tăng dần" : "Đã sắp xếp danh sách theo số lượng giảm dần");
        }

        // Xuất danh sách sản phẩm ra file .txt bằng SaveFileDialog
        private void XuatFileText()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Xuất danh sách sản phẩm ra file Text";
                sfd.Filter = "Text file (*.txt)|*.txt";
                sfd.FileName = $"DanhSachSanPham_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine("==========================================================================================");
                            sw.WriteLine("                            DANH SÁCH SẢN PHẨM (PRODUCT MANAGER)");
                            sw.WriteLine($" Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                            sw.WriteLine("==========================================================================================");
                            sw.WriteLine(string.Format("{0,-8} | {1,-32} | {2,-12} | {3,12} | {4,8} | {5,14} | {6,-10}",
                                "Mã SP", "Tên sản phẩm", "Loại SP", "Đơn giá", "Số lượng", "Thành tiền", "Trạng thái"));
                            sw.WriteLine(new string('-', 106));

                            foreach (var p in products)
                            {
                                sw.WriteLine(string.Format("{0,-8} | {1,-32} | {2,-12} | {3,12:N0} | {4,8:N0} | {5,14:N0} | {6,-10}",
                                    p.MaSP,
                                    p.TenSP.Length > 32 ? p.TenSP.Substring(0, 29) + "..." : p.TenSP,
                                    p.LoaiSP,
                                    p.DonGia,
                                    p.SoLuong,
                                    p.ThanhTien,
                                    p.ConKinhDoanh ? "Còn KD" : "Ngừng KD"));
                            }

                            sw.WriteLine("==========================================================================================");
                            decimal tongGiaTri = products.Sum(p => p.ThanhTien);
                            int tongSL = products.Sum(p => p.SoLuong);
                            sw.WriteLine($"Tổng số sản phẩm: {products.Count} | Tổng tồn kho: {tongSL:N0} cái | Tổng giá trị: {tongGiaTri:N0} VNĐ");
                            sw.WriteLine("==========================================================================================");
                        }

                        MessageBox.Show($"Đã xuất danh sách thành công ra file:\n{sfd.FileName}",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStatus($"Đã xuất file text thành công: {Path.GetFileName(sfd.FileName)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region QUẢN LÝ TRẠNG THÁI NÚT BẤM & TƯƠNG TÁC LƯỚI

        // 1. Tự động bật/tắt nút Sửa, Xóa trên tất cả các thanh điều hướng
        private void CapNhatTrangThaiNut()
        {
            bool coChonDong = dgvProducts.CurrentRow != null && dgvProducts.SelectedRows.Count > 0;
            mnuSua.Enabled = tsbSua.Enabled = ctxSua.Enabled = btnSua.Enabled = coChonDong;
            mnuXoa.Enabled = tsbXoa.Enabled = ctxXoa.Enabled = btnXoa.Enabled = coChonDong;
            ctxChiTiet.Enabled = coChonDong;
        }

        // 2. Nhấp chuột trái để đổ dữ liệu lên Form
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Product p = LaySanPhamDangChon();
                if (p != null)
                {
                    LoadProductToForm(p);
                    CapNhatTrangThaiNut();
                }
            }
        }

        // 3. Nhấp chuột phải tự động bôi xanh chọn dòng trước khi mở ContextMenuStrip
        private void dgvProducts_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvProducts.ClearSelection();
                dgvProducts.Rows[e.RowIndex].Selected = true;
                dgvProducts.CurrentCell = dgvProducts.Rows[e.RowIndex].Cells[0];
                productSource.Position = e.RowIndex;
                CapNhatTrangThaiNut();
            }
        }

        // 4. Chọn ảnh an toàn với OpenFileDialog và PictureBoxSizeMode.Zoom
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh sản phẩm";
                dialog.Filter = "Ảnh sản phẩm (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (picAnh.Image != null)
                    {
                        picAnh.Image.Dispose();
                        picAnh.Image = null;
                    }

                    picAnh.SizeMode = PictureBoxSizeMode.Zoom;
                    using (var bmpTemp = new Bitmap(dialog.FileName))
                    {
                        picAnh.Image = new Bitmap(bmpTemp);
                    }
                    picAnh.Tag = dialog.FileName;
                    UpdateStatus($"Đã chọn ảnh: {Path.GetFileName(dialog.FileName)}");
                }
            }
        }

        #endregion

        #region ĐỒNG BỘ SỰ KIỆN GỌI CHUNG (DRY DELEGATION)

        // Nhóm Thêm
        private void btnThem_Click(object sender, EventArgs e) => ThemSanPham();
        private void mnuThem_Click(object sender, EventArgs e) => ThemSanPham();
        private void tsbThem_Click(object sender, EventArgs e) => ThemSanPham();

        // Nhóm Sửa
        private void btnSua_Click(object sender, EventArgs e) => SuaSanPham();
        private void mnuSua_Click(object sender, EventArgs e) => SuaSanPham();
        private void tsbSua_Click(object sender, EventArgs e) => SuaSanPham();
        private void ctxSua_Click(object sender, EventArgs e) => SuaSanPham();

        // Nhóm Xóa
        private void btnXoa_Click(object sender, EventArgs e) => XoaSanPham();
        private void mnuXoa_Click(object sender, EventArgs e) => XoaSanPham();
        private void tsbXoa_Click(object sender, EventArgs e) => XoaSanPham();
        private void ctxXoa_Click(object sender, EventArgs e) => XoaSanPham();

        // Nhóm Làm mới
        private void btnLamMoi_Click(object sender, EventArgs e) => LamMoiToanBo();
        private void mnuFileLamMoi_Click(object sender, EventArgs e) => LamMoiToanBo();
        private void tsbLamMoi_Click(object sender, EventArgs e) => LamMoiToanBo();

        // Nhóm Xuất file Text (Bonus)
        private void mnuFileXuatText_Click(object sender, EventArgs e) => XuatFileText();
        private void tsbXuatText_Click(object sender, EventArgs e) => XuatFileText();

        // Nhóm Sắp xếp (Bonus)
        private void mnuSapXepDonGiaTang_Click(object sender, EventArgs e) => SapXepDonGia(true);
        private void tsmSapXepDonGiaTang_Click(object sender, EventArgs e) => SapXepDonGia(true);

        private void mnuSapXepDonGiaGiam_Click(object sender, EventArgs e) => SapXepDonGia(false);
        private void tsmSapXepDonGiaGiam_Click(object sender, EventArgs e) => SapXepDonGia(false);

        private void mnuSapXepSoLuongTang_Click(object sender, EventArgs e) => SapXepSoLuong(true);
        private void tsmSapXepSoLuongTang_Click(object sender, EventArgs e) => SapXepSoLuong(true);

        private void mnuSapXepSoLuongGiam_Click(object sender, EventArgs e) => SapXepSoLuong(false);
        private void tsmSapXepSoLuongGiam_Click(object sender, EventArgs e) => SapXepSoLuong(false);

        // Nhóm Tìm kiếm
        private void tsbTimKiem_Click(object sender, EventArgs e) => TimKiemSanPham();
        private void tstxtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimKiemSanPham();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Chi tiết, Giới thiệu & Thoát
        private void ctxChiTiet_Click(object sender, EventArgs e) => XemChiTietSanPham();
        private void mnuFileThoat_Click(object sender, EventArgs e) => Application.Exit();
        private void mnuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Phần mềm Quản lý sản phẩm ProductManager v1.0\nChuyên đề: UI/UX & Bộ điều hướng\nCOMP1019 - Lập trình trên Windows",
                            "Thông tin ứng dụng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}