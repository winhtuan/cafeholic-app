using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.service
{
    internal class RoleService
    {
        private readonly CafeholicContext _context;

        public RoleService()
        {
            _context = new CafeholicContext();
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
