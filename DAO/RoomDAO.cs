using CAFEHOLIC.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO;

public class RoomDAO
{
    private readonly CafeholicContext _context;
    private readonly ILogger<RoomDAO> _logger;

    public RoomDAO(ILogger<RoomDAO> logger)
    {
        _context = new CafeholicContext();
        _logger = logger;
    }

    public List<StudyRoom> GetAllRooms()
    {
        try
        {
            return _context.StudyRooms.Include(r => r.RoomType).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving list of study rooms.");
            return new List<StudyRoom>();
        }
    }

    public StudyRoom? GetRoomById(int roomId)
    {
        try
        {
            return _context.StudyRooms.Include(r => r.RoomType).FirstOrDefault(r => r.RoomId == roomId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving study room information.");
            return null;
        }
    }

    public bool UpdateRoomStatus(int roomId, bool isAvailable)
    {
        try
        {
            var room = _context.StudyRooms.Find(roomId);
            if (room != null)
            {
                room.IsAvailable = isAvailable;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room status.");
            return false;
        }
    }
}