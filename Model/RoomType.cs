using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class RoomType
{
    public int TypeId { get; set; }

    public string? Name { get; set; }

    public int? MinCapacity { get; set; }

    public int? MaxCapacity { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<StudyRoom> StudyRooms { get; set; } = new List<StudyRoom>();
}
