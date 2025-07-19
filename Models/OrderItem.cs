using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? DrinkId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Drink? Drink { get; set; }

    public virtual Order? Order { get; set; }
}
