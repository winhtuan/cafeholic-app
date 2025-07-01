using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAFEHOLIC.DAO
{
    using global::CAFEHOLIC.dao;
    using global::CAFEHOLIC.Model;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;

    namespace CAFEHOLIC.DAO
    {
        public class RoomDAO
        {
            private readonly DBContext context;
            private readonly ILogger<RoomDAO> logger;

            public RoomDAO(DBContext context, ILogger<RoomDAO> logger)
            {
                this.context = context;
                this.logger = logger;
            }

            public List<StudyRoom> GetAllRooms()
            {
                List<StudyRoom> rooms = new();
                try
                {
                    using (var conn = context.GetConnection())
                    {
                        string query = @"SELECT sr.RoomID, sr.Name, sr.IsAvailable, rt.TypeID, rt.Name AS TypeName,
                                            rt.MinCapacity, rt.MaxCapacity, rt.Description
                                     FROM StudyRoom sr
                                     LEFT JOIN RoomType rt ON sr.RoomTypeID = rt.TypeID";

                        using (var cmd = new SqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var roomType = new RoomType
                                {
                                    TypeId = reader.GetInt32(reader.GetOrdinal("TypeID")),
                                    Name = reader["TypeName"]?.ToString(),
                                    MinCapacity = reader["MinCapacity"] as int?,
                                    MaxCapacity = reader["MaxCapacity"] as int?,
                                    Description = reader["Description"]?.ToString()
                                };

                                var room = new StudyRoom
                                {
                                    RoomId = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                    Name = reader["Name"]?.ToString(),
                                    IsAvailable = reader["IsAvailable"] as bool?,
                                    RoomTypeId = roomType.TypeId,
                                    RoomType = roomType
                                };

                                rooms.Add(room);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi lấy danh sách phòng học.");
                }

                return rooms;
            }

            public StudyRoom? GetRoomById(int roomId)
            {
                try
                {
                    using (var conn = context.GetConnection())
                    {
                        string query = @"SELECT sr.RoomID, sr.Name, sr.IsAvailable, rt.TypeID, rt.Name AS TypeName,
                                            rt.MinCapacity, rt.MaxCapacity, rt.Description
                                     FROM StudyRoom sr
                                     LEFT JOIN RoomType rt ON sr.RoomTypeID = rt.TypeID
                                     WHERE sr.RoomID = @RoomID";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@RoomID", roomId);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var roomType = new RoomType
                                    {
                                        TypeId = reader.GetInt32(reader.GetOrdinal("TypeID")),
                                        Name = reader["TypeName"]?.ToString(),
                                        MinCapacity = reader["MinCapacity"] as int?,
                                        MaxCapacity = reader["MaxCapacity"] as int?,
                                        Description = reader["Description"]?.ToString()
                                    };

                                    var room = new StudyRoom
                                    {
                                        RoomId = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                        Name = reader["Name"]?.ToString(),
                                        IsAvailable = reader["IsAvailable"] as bool?,
                                        RoomTypeId = roomType.TypeId,
                                        RoomType = roomType
                                    };

                                    return room;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi lấy thông tin phòng học.");
                }

                return null;
            }
            public bool UpdateRoomStatus(int roomId, bool isAvailable)
            {
                try
                {
                    using var conn = context.GetConnection();
                    string query = "UPDATE StudyRoom SET IsAvailable = @IsAvailable WHERE RoomId = @RoomId";

                    using var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                    cmd.Parameters.AddWithValue("@RoomId", roomId);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Lỗi khi cập nhật trạng thái phòng.");
                    return false;
                }
            }

        }
    }

}
