using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class Role
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
