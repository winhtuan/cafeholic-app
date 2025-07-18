using CAFEHOLIC.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO;

public class ReservationDAO
{
    private readonly CafeholicContext _context;
    private readonly ILogger<ReservationDAO> _logger;

    public ReservationDAO(ILogger<ReservationDAO> logger)
    {
        _context = new CafeholicContext();
        _logger = logger;
    }

    public bool InsertReservation(Reservation reservation)
    {
        try
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding Reservation.");
            return false;
        }
    }

    public bool EndCurrentReservation(int roomId, DateTime now)
    {
        try
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.RoomId == roomId && r.Status == "Pending");
            if (reservation != null)
            {
                reservation.EndTime = now;
                reservation.Status = "Completed";
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending reservation.");
            return false;
        }
    }

    public bool IsRoomAvailable(int roomId, DateTime startTime, DateTime endTime)
    {
        try
        {
            return !_context.Reservations.Any(r =>
                r.RoomId == roomId &&
                r.Status != "Cancelled" &&
                r.StartTime < endTime &&
                r.EndTime > startTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking room availability.");
            return false;
        }
    }

    public List<Reservation> GetReservations()
    {
        try
        {
            return _context.Reservations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Reservation list.");
            return new List<Reservation>();
        }
    }
}