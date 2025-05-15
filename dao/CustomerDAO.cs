using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace CAFEHOLIC.dao
{
    class CustomerDAO
    {
        private readonly DBContext context;

        public CustomerDAO(DBContext context)
        {
            this.context = context;
        }

        public void TestConnectionWithMessage()
        {
            try
            {
                using (SqlConnection conn = context.GetConnection())
                {
                    // Nếu tới đây không lỗi thì kết nối OK
                    MessageBox.Show("Kết nối thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối thất bại: " + ex.Message, "Lỗi");
            }
        }
    }
}