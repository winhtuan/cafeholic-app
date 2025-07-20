using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? TransactionId { get; set; }

    public string? PaymentType { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
