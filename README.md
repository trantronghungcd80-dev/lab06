# LAB 06: WINDOWS FORMS NÂNG CAO VÀ DATAGRIDVIEW
## Đề tài: Quản lý sản phẩm (ProductManager)
**Môn học:** COMP1019 – Lập trình trên Windows  
**Công nghệ:** C# (.NET Framework 4.7.2), Windows Forms, Visual Studio  

---

## 1. Giới thiệu tổng quan
Ứng dụng **ProductManager** được xây dựng nhằm quản lý danh sách sản phẩm của cửa hàng với dữ liệu lưu trữ trong bộ nhớ (*In-Memory*) thông qua kiến trúc liên kết dữ liệu 3 tầng (`BindingList<T>` $\rightarrow$ `BindingSource` $\rightarrow$ `DataGridView`).

Chương trình áp dụng đầy đủ các kỹ thuật UI/UX và bộ điều hướng nâng cao:
* **MenuStrip:** Hệ thống menu đa tầng (File, Quản lý, Trợ giúp) tích hợp phím tắt tiêu chuẩn (`Ctrl+N`, `Ctrl+S`, `Delete`, `F5`, `Ctrl+E`, `Alt+F4`).
* **ToolStrip:** Thanh công cụ trực quan hỗ trợ thao tác nhanh các chức năng CRUD và thanh tìm kiếm tức thời.
* **StatusStrip:** Thanh trạng thái chân trang phản hồi kết quả thao tác, hiển thị tổng số lượng sản phẩm, tổng tồn kho và tổng giá trị tồn kho.
* **ContextMenuStrip:** Menu ngữ cảnh chuột phải trên từng dòng của `DataGridView` (Sửa, Xóa, Xem chi tiết).
* **ErrorProvider:** Bắt lỗi dữ liệu đầu vào tại chỗ với icon cảnh báo màu đỏ trực quan.
* **OpenFileDialog:** Hộp thoại chọn ảnh sản phẩm an toàn và hiển thị chuẩn tỉ lệ `PictureBoxSizeMode.Zoom`.

---

## 2. Cấu trúc dữ liệu sản phẩm (`Product.cs`)

| Tên trường | Kiểu dữ liệu | Ràng buộc & Quy tắc nghiệp vụ |
| :--- | :--- | :--- |
| `MaSP` | `string` | Mã sản phẩm, không rỗng, không được trùng lặp khi thêm mới |
| `TenSP` | `string` | Tên sản phẩm, không được để trống |
| `LoaiSP` | `string` | Chọn từ danh mục: *Điện tử, Văn phòng, Gia dụng, Thời trang* |
| `DonGia` | `decimal` | Đơn giá sản phẩm, phải lớn hơn 0 VNĐ |
| `SoLuong` | `int` | Số lượng tồn kho, lớn hơn hoặc bằng 0 |
| `NgayNhap` | `DateTime` | Ngày nhập hàng, không được lớn hơn ngày hiện tại |
| `ConKinhDoanh`| `bool` | Trạng thái: `true` (Còn kinh doanh) / `false` (Ngừng kinh doanh) |
| `DuongDanAnh` | `string` | Đường dẫn tệp ảnh sản phẩm trên máy tính |
| `ThanhTien` | `decimal` | Thuộc tính chỉ đọc tự động tính toán: `DonGia * SoLuong` |

---

## 3. Kiến trúc mã nguồn và Clean Code

Mã nguồn tuân thủ nguyên lý **Clean Code** và **DRY (Don't Repeat Yourself)**:
1. **Bộ 4 hàm cơ sở độc lập:**
   * `ValidateInput(bool isAddingNew)`: Kiểm tra toàn bộ điều kiện dữ liệu với `ErrorProvider`.
   * `ClearInput()`: Làm sạch form nhập liệu, xóa lỗi và đưa con trỏ về ô Mã SP.
   * `LoadProductToForm(Product p)`: Đổ dữ liệu từ dòng được chọn lên form, khóa chỉnh sửa Mã SP khi xem/sửa và tải ảnh an toàn không khóa file.
   * `UpdateStatus(string thongBao)`: Cập nhật thông điệp và tính toán thống kê tồn kho theo thời gian thực.
2. **Bộ điều hướng đồng bộ (Delegation):**
   * Các sự kiện click từ Menu, Toolbar, ContextMenu và Button trên Form đều ủy quyền xử lý về các hàm nghiệp vụ chung: `ThemSanPham()`, `SuaSanPham()`, `XoaSanPham()`, `LamMoiToanBo()`.
3. **Quản lý trạng thái nút bấm tập trung (`CapNhatTrangThaiNut`):**
   * Tự động vô hiệu hóa (`Enabled = false`) nút Sửa, Xóa trên tất cả các thanh điều hướng khi chưa có dòng nào được chọn trên `DataGridView`.
4. **Chuột phải thông minh (`CellMouseDown`):**
   * Tự động chọn dòng và đồng bộ con trỏ `BindingSource` ngay khi nhấp chuột phải trước khi mở `ContextMenuStrip`.

---

## 4. Các tính năng mở rộng (Bonus) đã hoàn thành
- [x] **Sắp xếp linh hoạt:** Cho phép sắp xếp danh sách theo *Đơn giá tăng/giảm* hoặc *Số lượng tồn tăng/giảm* trực tiếp từ MenuStrip và ToolStrip.
- [x] **Xuất danh sách ra file Text (`SaveFileDialog`):** Xuất toàn bộ bảng danh sách sản phẩm được căn chỉnh cột đẹp mắt, kèm tổng hợp tồn kho và tổng thành tiền ra tệp `.txt`.
- [x] **Thống kê tổng giá trị tồn kho:** Tính toán và cập nhật liên tục tổng giá trị tồn kho (VNĐ) và tổng số lượng sản phẩm trên thanh `StatusStrip`.

---

## 5. Cấu trúc thư mục dự án

```text
Lab06/
├── Lab06_QuanLySanPham.sln             # Solution chính mở bằng Visual Studio
├── Lab06_YeuCau.docx                   # Tệp tài liệu yêu cầu đề bài
├── .gitignore                          # Cấu hình bỏ qua tệp rác build
├── README.md                           # Báo cáo và hướng dẫn dự án
└── Lab06_QuanLySanPham/                # Thư mục mã nguồn project
    ├── Lab06_QuanLySanPham.csproj      # File cấu hình project C#
    ├── Lab06_QuanLySanPham.slnx        # File cấu hình solution thế hệ mới
    ├── App.config                      # Cấu hình ứng dụng .NET
    ├── Product.cs                      # Lớp thực thể sản phẩm
    ├── Program.cs                      # Entry point khởi chạy Form
    ├── FrmProduct.cs                   # Xử lý logic sự kiện & nghiệp vụ
    ├── FrmProduct.Designer.cs          # Mã sinh giao diện tự động
    ├── FrmProduct.resx                 # Tài nguyên giao diện Form
    └── Properties/
        ├── AssemblyInfo.cs
        ├── Resources.Designer.cs
        ├── Resources.resx
        ├── Settings.Designer.cs
        └── Settings.settings
```

---

## 6. Hướng dẫn cài đặt và chạy ứng dụng

1. **Yêu cầu hệ thống:**
   * Hệ điều hành: Windows 10 / 11.
   * IDE: Visual Studio 2019, 2022 hoặc mới hơn với workload **.NET desktop development**.
   * Target Framework: .NET Framework 4.7.2 trở lên.

2. **Cách mở và chạy:**
   * Clone repository về máy:
     ```bash
     git clone https://github.com/trantronghungcd80-dev/lab06.git
     ```
   * Mở tệp `Lab06_QuanLySanPham.sln` bằng Visual Studio.
   * Nhấn phím `F5` hoặc chọn menu **Debug > Start Debugging** để biên dịch và chạy ứng dụng.