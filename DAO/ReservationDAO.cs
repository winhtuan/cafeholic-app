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
        public class ReservationDAO
        {
            private readonly DBContext context;
            private readonly ILogger<ReservationDAO> logger;

            public ReservationDAO(DBContext context, ILogger<ReservationDAO> logger)
            {
                this.context = context;
                this.logger = logger;
            }

            public bool EndCurrentReservation(int roomId, DateTime now)
            {
                try
                {
                    using var conn = context.GetConnection();
                    string query = @"
                    UPDATE Reservation
                    SET EndTime = @Now, Status = 'Completed'
                    WHERE RoomId = @RoomId AND Status = 'Pending'";
                    using var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoomId", roomId);
                    cmd.Parameters.AddWithValue("@Now", now);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi kết thúc reservation.");
                    return false;
                }
            }


            // ✅ Thêm đặt phòng mới
            public bool InsertReservation(Reservation reservation)
            {
                try
                {
                    using var conn = context.GetConnection();
                    string query = @"
                    INSERT INTO Reservation (UserId, RoomId, StartTime, Status)
                    VALUES (@UserId, @RoomId, @StartTime, @Status)";
                    using var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", reservation.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoomId", reservation.RoomId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartTime", reservation.StartTime ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status ?? "Pending");

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Lỗi khi thêm Reservation");
                    return false;
                }
            }

            // ✅ Kiểm tra trùng giờ phòng
            public bool IsRoomAvailable(int roomId, DateTime startTime, DateTime endTime)
            {
                try
                {
                    using var conn = context.GetConnection();
                    string query = @"
                    SELECT COUNT(*) FROM Reservation
                    WHERE RoomId = @RoomId AND Status != 'Cancelled'
                          AND ((StartTime < @EndTime AND EndTime > @StartTime))";
                    using var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoomId", roomId);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0; // ✅ Không trùng thì phòng còn trống
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Lỗi khi kiểm tra lịch phòng.");
                    return false;
                }
            }

            // ✅ (Tùy chọn) Lấy danh sách đặt phòng
            public List<Reservation> GetReservations()
            {
                var list = new List<Reservation>();
                try
                {
                    using var conn = context.GetConnection();
                    string query = "SELECT * FROM Reservation";
                    using var cmd = new SqlCommand(query, conn);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Reservation
                        {
                            ReservationId = reader.GetInt32(reader.GetOrdinal("ReservationId")),
                            UserId = reader["UserId"] as int?,
                            RoomId = reader["RoomId"] as int?,
                            StartTime = reader["StartTime"] as DateTime?,
                            EndTime = reader["EndTime"] as DateTime?,
                            Status = reader["Status"]?.ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Lỗi khi lấy danh sách Reservation.");
                }

                return list;
            }
        }
    }

}
