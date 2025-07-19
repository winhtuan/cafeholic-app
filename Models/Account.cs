using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class Account
{
    public int AccId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PasswordHash { get; set; }

    public DateTime? RegistDate { get; set; }

    public string? VerificationToken { get; set; }

    public bool? IsVerified { get; set; }

    public int? RoleId { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
