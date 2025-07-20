using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.service
{
    internal class UserService
    {
        private readonly UserDAO dao;

        public UserService()
        {
            dao = new UserDAO(new DBContext(), new DBContext().GetLogger<UserDAO>());
        }

        public List<User> GetUserList()
        {
            return dao.GetListUser();
        }

        public List<User> GetStaffList()
        {
            return dao.GetListStaff();
        }

        public User? GetUser(int id)
        {
            return dao.GetUser(id);
        }

        public User? GetStaff(int id)
        {
            return dao.GetStaff(id);
        }
        public bool CreateUserWithAccount(User user, Account account)
        {
            return dao.AddUserWithAccount(user, account);
        }

        public bool UpdateUserWithAccount(User user, Account account)
        {
            return dao.UpdateUserWithAccount(user, account);
        }

        public bool DeleteUser(int id)
        {
            return dao.DeleteUser(id);
        }
    }
}
