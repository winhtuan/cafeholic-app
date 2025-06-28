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
        public string GenerateOTP(string phoneNumber)
        {
            var random = new Random();
            string otp = string.Concat(Enumerable.Range(0, 6).Select(_ => random.Next(10).ToString()));

            // (Tuỳ chọn) Lưu OTP vào cache/bộ nhớ hoặc DB nếu cần xác minh sau
            // Example: SaveOtpToDatabase(phoneNumber, otp);

            return otp;
        }

        public Account CheckLogin(string phone, string password)
        {
            logger.LogInformation("Kiểm tra đăng nhập cho tài khoản: {Username}", phone);
            logger.LogInformation("Mật khẩu đã được cung cấp: {Password}", password);
            try
            {
                using (var conn = context.GetConnection())
                {

                    string query = "SELECT * FROM Account WHERE PhoneNumber = @phone AND PasswordHash = @Password " +
                                   "AND IsVerified = 1";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@phone", phone);
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

        public Account GetAccountById(int accId)
        {
            logger.LogInformation("Lấy thông tin tài khoản với ID: {AccId}", accId);
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "SELECT * FROM Account WHERE AccID = @AccId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", accId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Account acc = new Account
                                {
                                    AccId = reader.GetInt32(reader.GetOrdinal("AccID")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    RegistDate = reader.IsDBNull(reader.GetOrdinal("RegistDate")) ? null : reader.GetDateTime(reader.GetOrdinal("RegistDate")),
                                    VerificationToken = reader.IsDBNull(reader.GetOrdinal("VerificationToken")) ? null : reader.GetString(reader.GetOrdinal("VerificationToken")),
                                    IsVerified = reader.IsDBNull(reader.GetOrdinal("IsVerified")) ? null : reader.GetBoolean(reader.GetOrdinal("IsVerified")),
                                    RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetInt32(reader.GetOrdinal("RoleID")),
                                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetInt32(reader.GetOrdinal("UserID"))
                                };
                                return acc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi lấy thông tin tài khoản.");
            }
            return null; // Không tìm thấy tài khoản
        }

        public Account CreateAccount(String Phone, String Password,int userID)
        {
            logger.LogInformation("Tạo tài khoản mới với số điện thoại: {Phone}", Phone);
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, IsVerified,UserId,RoleId) " +
                                   "VALUES (@Phone, @Password, GETDATE(), 1,@userid,1);SELECT SCOPE_IDENTITY();";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", Phone);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@userid", userID);
                        int newAccId = Convert.ToInt32(cmd.ExecuteScalar());
                        logger.LogInformation("Tài khoản mới đã được tạo với ID: {NewAccId}", newAccId);
                        return GetAccountById(newAccId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi tạo tài khoản mới.");
            }
            return null;
        }

        public bool IsPhoneNumberExists(string phoneNumber)
        {
            logger.LogInformation("Kiểm tra sự tồn tại của số điện thoại: {Phone}", phoneNumber);
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "SELECT 1 FROM Account WHERE PhoneNumber = @Phone";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);

                        var result = cmd.ExecuteScalar();
                        return result != null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi kiểm tra số điện thoại.");
                return false; // hoặc tùy bạn, có thể throw ra
            }
        }

    }
}
