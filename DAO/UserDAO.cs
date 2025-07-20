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

namespace CAFEHOLIC.DAO
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

<<<<<<< HEAD
        public List<User> GetListStaff()
        {
            var listStaff = new List<User>();
            try
            {
                using var context = new CafeholicContext();
                listStaff = context.Users
                    .Where(u => u.Accounts.Any(a => a.Role != null && a.Role.RoleName.Equals("Staff")))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listStaff;
        }

        public List<User> GetListUser()
        {
            var listUser = new List<User>();
            try
            {
                using var context = new CafeholicContext();
                listUser = context.Users
                    .Where(u => u.Accounts.Any(a => a.Role != null && a.Role.RoleName.Equals("User")))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listUser;
        }

        public User? GetUser(int id)
        {
            try
            {
                using var context = new CafeholicContext();

                var user = context.Users
                    .FirstOrDefault(u => u.Id == id &&
                                         u.Accounts.Any(a => a.Role != null && a.Role.RoleName.Equals("User")));

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi truy vấn user: " + ex.Message);
            }
        }

        public User? GetStaff(int id)
        {
            try
            {
                using var context = new CafeholicContext();

                var user = context.Users
                    .FirstOrDefault(u => u.Id == id &&
                                         u.Accounts.Any(a => a.Role != null && a.Role.RoleName.Equals("Staff")));

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi truy vấn user: " + ex.Message);
            }
        }

        public bool AddUserWithAccount(User user, Account account)
        {
            try
            {
                using var context = new CafeholicContext();

                // Thêm user trước
                context.Users.Add(user);
                context.SaveChanges();

                // Sau khi có user.Id → thêm Account
                account.UserId = user.Id;
                account.RegistDate = DateTime.Now;
                account.IsVerified = false;

                context.Accounts.Add(account);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi thêm User + Account");
                return false;
            }
        }

        public bool UpdateUserWithAccount(User user, Account account)
        {
            try
            {
                using var context = new CafeholicContext();

                // Update User
                var existingUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                }

                // Update Account
                var existingAccount = context.Accounts.FirstOrDefault(a => a.UserId == user.Id);
                if (existingAccount != null)
                {
                    existingAccount.PhoneNumber = account.PhoneNumber;
                    existingAccount.PasswordHash = account.PasswordHash;
                    existingAccount.RoleId = account.RoleId;
                }

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi cập nhật User + Account");
                return false;
            }
        }


        public bool DeleteUser(int userId)
        {
            try
            {
                using var context = new CafeholicContext();

                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                    return false;

                // Xóa Account liên quan trước
                var accounts = context.Accounts.Where(a => a.UserId == userId).ToList();
                context.Accounts.RemoveRange(accounts);

                // Sau đó xóa User
                context.Users.Remove(user);

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xóa User: " + ex.Message);
                return false;
            }
        }


=======
        public User? GetUserByPhone(string phoneNumber)
        {
            logger.LogInformation("Lấy thông tin người dùng với số điện thoại: {PhoneNumber}", phoneNumber);
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "SELECT Id, PhoneNumber, FullName FROM [User] WHERE PhoneNumber = @PhoneNumber";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(0),
                                    PhoneNumber = reader.GetString(1),
                                    FullName = reader.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi lấy thông tin người dùng.");
            }
            return null;
        }
>>>>>>> origin/develop
    }
}