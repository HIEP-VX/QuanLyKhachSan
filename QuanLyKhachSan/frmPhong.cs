using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyKhachSan
{
    public partial class frmPhong : Form
    {
        private int index = -1;
        public frmPhong()
        {
            InitializeComponent();
        }

        private void reload()
        {
            string sql = "select maPhong, hangPhong, donGiaThue, viTri,\n"+
                         "CASE\n" +
                         "WHEN trangThai = 1 THEN N'Trống'\n" +
                         "WHEN trangThai = 2 THEN N'Đang sử dụng'\n" +
                         "END AS trangThai\n"+
                         "from phong";
            dgvPhong.DataSource = accessData.getData(sql);

            txtMa.Text = "";
            txtViTri.Text = "";
            txtGia.Text = "";
            txtHangPhong.Text = "";
            cbTrangThai.Text = "";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtHangPhong.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập hạng phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtHangPhong.Focus();
                return;
            }

            if (txtGia.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập giá thuê phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtGia.Focus();
                return;
            }

            if (txtViTri.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập vị trí phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtViTri.Focus();
                return;
            }

            if (cbTrangThai.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập trạng thái phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbTrangThai.Focus();
                return;
            }

            // Tách giá trị của ô trạng thái
            string selectedItem = cbTrangThai.SelectedItem as string;
            string selectedValue = "";

            // Tách giá trị (trong trường hợp này, "1")
            string[] parts = selectedItem.Split('-');
            if (parts.Length == 2)
                // Sử dụng giá trị (trong trường hợp này, "1")
                selectedValue = parts[0].Trim();

            string sql = "insert into phong (hangPhong, donGiaThue, viTri, trangThai) values (@hang, @gia, @vitri, @trangthai)";
            using (SqlConnection conn = SqlConnectionData.connect())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@hang", txtHangPhong.Text);
                cmd.Parameters.AddWithValue("@gia", txtGia.Text);
                cmd.Parameters.AddWithValue("@vitri", txtViTri.Text);
                cmd.Parameters.AddWithValue("@trangthai", selectedValue);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    reload();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void frmPhong_Load(object sender, EventArgs e)
        {
            reload();
            txtMa.ReadOnly= true;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Vui lòng chọn một phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Tách giá trị của ô trạng thái
                string selectedItem = cbTrangThai.SelectedItem as string;
                string selectedValue = "";

                // Tách giá trị (trong trường hợp này, "1")
                string[] parts = selectedItem.Split('-');
                if (parts.Length == 2)
                    // Sử dụng giá trị (trong trường hợp này, "1")
                    selectedValue = parts[0].Trim();

                string query = "UPDATE Phong SET hangPhong = @hang, donGiaThue = @gia, viTri = @vitri, trangThai = @trangThai WHERE maPhong = @maPhong";
                using (SqlConnection conn = SqlConnectionData.connect())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@maPhong", txtMa.Text);
                    cmd.Parameters.AddWithValue("@hang", txtHangPhong.Text);
                    cmd.Parameters.AddWithValue("@gia", txtGia.Text);
                    cmd.Parameters.AddWithValue("@vitri", txtViTri.Text);
                    cmd.Parameters.AddWithValue("@trangThai", selectedValue);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        reload();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            if (index < 0)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtMa.Text = dgvPhong.Rows[index].Cells[0].Value.ToString();
            txtHangPhong.Text = dgvPhong.Rows[index].Cells[1].Value.ToString();
            txtGia.Text = dgvPhong.Rows[index].Cells[2].Value.ToString();
            txtViTri.Text = dgvPhong.Rows[index].Cells[3].Value.ToString();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Vui lòng chọn một phòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string sql = "Delete from phong where maPhong = '" + txtMa.Text + "'";
                    try
                    {
                        accessData.execQuery(sql);
                        MessageBox.Show("Xóa thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        reload();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
