using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Windows;
using CAFEHOLIC.DAO;
using Microsoft.Extensions.Logging;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.dao
{
    class UserDAO
    {
        private readonly ILogger<UserDAO> logger;
        private readonly DBContext context;

        public UserDAO(DBContext context, ILogger<UserDAO> logg)
        {
            this.context = context;
            this.logger = logg;
        }

        public User CreateUser(string phoneNumber, string fullName, string password)
        {
            logger.LogInformation("Tạo người dùng mới với số điện thoại: {PhoneNumber}", phoneNumber);
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "INSERT INTO [User] (PhoneNumber, FullName) OUTPUT INSERTED.Id VALUES (@PhoneNumber, @FullName)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        int userId = (int)cmd.ExecuteScalar();
                        logger.LogInformation("Người dùng mới đã được tạo với ID: {UserId}", userId);
                        return new User { Id = userId, PhoneNumber = phoneNumber, FullName = fullName };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi tạo người dùng mới.");
                throw;
            }
        }
    }
}