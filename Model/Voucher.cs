using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherCode { get; set; }

    public string? Description { get; set; }

    public decimal? DiscountPercent { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
