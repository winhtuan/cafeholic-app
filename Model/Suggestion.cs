using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class Suggestion
{
    public int SuggestionId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? SuggestionDate { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Drink> Drinks { get; set; } = new List<Drink>();
}
