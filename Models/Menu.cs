using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public int? DrinkId { get; set; }

    public string? Name { get; set; }

    public virtual Drink? Drink { get; set; }
}
