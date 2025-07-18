using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public virtual Account? Account { get; set; }
}
