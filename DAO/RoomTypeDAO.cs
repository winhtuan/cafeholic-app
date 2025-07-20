using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.DAO
{
    public class RoomTypeDAO
    {
        private readonly CafeholicContext _context;
        private readonly ILogger<RoomTypeDAO> _logger;

        public RoomTypeDAO(ILogger<RoomTypeDAO> logger)
        {
            _context = new CafeholicContext();
            _logger = logger;
        }

        public List<RoomType> GetAllRoomTypes()
        {
            try
            {
                return _context.RoomTypes
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving list of room types.");
                return new List<RoomType>();
            }
        }

        public bool AddRoomType(RoomType roomType)
        {
            try
            {
                _context.RoomTypes.Add(roomType);
                int rowsAffected = _context.SaveChanges();
                _logger.LogInformation($"[AddRoomType] Added room type: {roomType.Name}, Rows affected: {rowsAffected}");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding room type: {roomType.Name}.");
                return false;
            }
        }
    }
}