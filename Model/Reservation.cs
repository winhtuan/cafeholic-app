using System;
using System.Collections.Generic;

namespace CAFEHOLIC.Model;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Status { get; set; }

    public virtual StudyRoom? Room { get; set; }

    public virtual User? User { get; set; }
}
