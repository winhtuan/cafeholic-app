using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Models;

public partial class StudyRoom
{
    public int RoomId { get; set; }

    public string? Name { get; set; }

    public bool? IsAvailable { get; set; }

    public int? RoomTypeId { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual RoomType? RoomType { get; set; }
}
