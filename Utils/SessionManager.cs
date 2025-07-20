using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.Utils
{
    public class SessionManager
    {
        private static SessionManager instance;
        public static SessionManager Instance => instance ??= new SessionManager();

        public User CurrentUser { get; private set; }

        public void SetUser(User user)
        {
            CurrentUser = user;
        }
    }

}
