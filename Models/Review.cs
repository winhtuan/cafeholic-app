using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? UserId { get; set; }

    public int? DrinkId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Drink? Drink { get; set; }

    public virtual User? User { get; set; }
}
