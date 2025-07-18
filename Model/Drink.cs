using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class Drink
{
    public int DrinkId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool? IsAvailable { get; set; }

    public String img { get; set; } = string.Empty;

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Suggestion> Suggestions { get; set; } = new List<Suggestion>();
}
