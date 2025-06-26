using CAFEHOLIC.dao;
using CAFEHOLIC.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO
{
    public class AccountDAO
    {
        private readonly ILogger<AccountDAO> logger;
        private readonly DBContext context;

        public AccountDAO(DBContext dBContext, ILogger<AccountDAO> logger)
        {
            this.context = dBContext;
            this.logger = logger;
        }

        public Account CheckLogin(string username, string password)
        {
            logger.LogInformation("Kiểm tra đăng nhập cho tài khoản: {Username}", username);
            logger.LogInformation("Mật khẩu đã được cung cấp: {Password}", password);
            try
            {
                using (var conn = context.GetConnection())
                {

                    string query = "SELECT * FROM Account WHERE PhoneNumber = @Username AND PasswordHash = @Password " +
                                   "AND IsVerified = 1";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                logger.LogInformation("Đang thực hiện truy vấn đăng nhập...");

                                Account acc = new Account
                                {
                                    AccId = reader.GetInt32(reader.GetOrdinal("AccID")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                                };

                                return acc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi kiểm tra đăng nhập.");
            }

            return null; // Đăng nhập thất bại
        }

    }
}
