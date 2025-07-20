using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CAFEHOLIC.Model;
using CAFEHOLIC.dao;

namespace CAFEHOLIC.DAO
{
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
                return _context.StudyRooms
                    .AsNoTracking()
                    .Include(r => r.RoomType)
                    .Include(r => r.Reservations)
                    .ToList();
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
                return _context.StudyRooms
                    .Include(r => r.RoomType)
                    .FirstOrDefault(r => r.RoomId == roomId);
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

        public List<StudyRoom> GetPagedRooms(int page, int pageSize)
        {
            try
            {
                return _context.StudyRooms
                    .AsNoTracking()
                    .Include(r => r.RoomType)
                    .OrderBy(r => r.RoomId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving paged rooms for page {page}, pageSize {pageSize}.");
                return new List<StudyRoom>();
            }
        }

        public List<StudyRoom> GetPagedRooms(int page, int pageSize, string searchQuery)
        {
            try
            {
                IQueryable<StudyRoom> query = _context.StudyRooms
                    .AsNoTracking()
                    .Include(r => r.RoomType)
                    .OrderBy(r => r.RoomId);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.ToLower();
                    query = query.Where(r => r.Name.ToLower().Contains(searchQuery) || r.RoomType.Name.ToLower().Contains(searchQuery));
                }

                return query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving paged rooms for page {page}, pageSize {pageSize}, searchQuery '{searchQuery}'.");
                return new List<StudyRoom>();
            }
        }

        public int GetTotalRooms()
        {
            try
            {
                return _context.StudyRooms.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting total rooms.");
                return 0;
            }
        }

        public int GetTotalRooms(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return _context.StudyRooms.Count();
                }

                searchQuery = searchQuery.ToLower();
                return _context.StudyRooms
                    .Include(r => r.RoomType)
                    .Count(r => r.Name.ToLower().Contains(searchQuery) ||
                                r.RoomType.Name.ToLower().Contains(searchQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error counting total rooms with search query '{searchQuery}'.");
                return 0;
            }
        }

        public bool AddRoom(StudyRoom room)
        {
            try
            {
                _context.StudyRooms.Add(room);
                int rowsAffected = _context.SaveChanges();
                _logger.LogInformation($"[AddRoom] Added room: {room.Name}, Rows affected: {rowsAffected}");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding room: {room.Name}.");
                return false;
            }
        }

        public bool UpdateRoom(StudyRoom room)
        {
            try
            {
                var existingRoom = _context.StudyRooms.Find(room.RoomId);
                if (existingRoom != null)
                {
                    existingRoom.Name = room.Name;
                    existingRoom.IsAvailable = room.IsAvailable;
                    existingRoom.RoomTypeId = room.RoomTypeId;
                    int rowsAffected = _context.SaveChanges();
                    _logger.LogInformation($"[UpdateRoom] Updated room: {room.Name}, Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating room: {room.RoomId}.");
                return false;
            }
        }

        public bool DeleteRoom(int roomId)
        {
            try
            {
                var room = _context.StudyRooms.Find(roomId);
                if (room != null)
                {
                    var reservations = _context.Reservations.Any(r => r.RoomId == roomId);
                    if (reservations)
                    {
                        _logger.LogWarning($"[DeleteRoom] Cannot delete room {roomId} due to existing reservations.");
                        return false;
                    }
                    _context.StudyRooms.Remove(room);
                    int rowsAffected = _context.SaveChanges();
                    _logger.LogInformation($"[DeleteRoom] Deleted room: {roomId}, Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room: {roomId}.");
                return false;
            }
        }
    }
}