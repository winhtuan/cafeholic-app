using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils; 
using Microsoft.Data.SqlClient;

namespace CAFEHOLIC.DAO
{
    public class OrderDAO
    {
        private readonly DBContext context;

        public OrderDAO(DBContext dBContext)
        {
            this.context = dBContext;
        }

        public void updateStatus(String Status, int orderId)
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    string query = "UPDATE [Order] SET Status = @Status WHERE OrderId = @OrderId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(OrderDAO), "Lỗi khi cập nhật trạng thái đơn hàng.", ex);
                throw;
            }
        }

        public async Task<Order> CreateOrderFromCart(List<CartItem> cartItems, int? userId = null, int? voucherId = null)
        {
            if (cartItems == null || !cartItems.Any())
            {
                Logger.Error(nameof(OrderDAO), "Tạo đơn hàng thất bại: Giỏ hàng đang trống.");
                throw new ArgumentException("Giỏ hàng đang trống.");
            }

            Order order = null;

            try
            {
                using (var conn = context.GetConnection())
                {
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Tính tổng tiền
                            decimal total = cartItems.Sum(item => item.TotalPrice);

                            // 2. Thêm đơn hàng
                            string insertOrderSql = @"
                                INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId)
                                OUTPUT INSERTED.OrderId
                                VALUES (@UserId, @OrderDate, @TotalAmount, @Status, @VoucherId)";

                            int orderId;
                            using (var cmd = new SqlCommand(insertOrderSql, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@UserId", (object?)userId);
                                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@TotalAmount", total);
                                cmd.Parameters.AddWithValue("@Status", "Chờ Thanh Toán");
                                cmd.Parameters.AddWithValue("@VoucherId", (object?)voucherId ?? DBNull.Value);

                                orderId = (int)cmd.ExecuteScalar();
                            }

                            // 3. Thêm các OrderItem
                            foreach (var item in cartItems)
                            {
                                string insertItemSql = @"
                                    INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price)
                                    VALUES (@OrderId, @DrinkId, @Quantity, @Price)";
                                using (var cmd = new SqlCommand(insertItemSql, conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                                    cmd.Parameters.AddWithValue("@DrinkId", item.Drink.DrinkId);
                                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    cmd.Parameters.AddWithValue("@Price", item.Drink.Price);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 4. Commit transaction
                            tran.Commit();

                            // 5. Trả về Order vừa tạo
                            order = new Order
                            {
                                OrderId = orderId,
                                UserId = userId,
                                VoucherId = voucherId,
                                OrderDate = DateTime.Now,
                                TotalAmount = total,
                                Status = "Pending",
                                OrderItems = cartItems.Select(ci => new OrderItem
                                {
                                    DrinkId = ci.Drink.DrinkId,
                                    Quantity = ci.Quantity,
                                    Price = ci.Drink.Price
                                }).ToList()
                            };
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            Logger.Error(nameof(OrderDAO), "Lỗi khi tạo đơn hàng trong transaction.", ex);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(OrderDAO), "Lỗi kết nối hoặc xử lý đơn hàng.", ex);
                throw;
            }

            return order;
        }

        public List<Order> GetOrderByUserID(int userId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                using (var conn = context.GetConnection())
                {
                    // 1. Lấy các đơn hàng
                    string query = "SELECT * FROM [Order] WHERE UserId = @UserId ORDER BY OrderDate DESC";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order
                                {
                                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetInt32(reader.GetOrdinal("UserId")),
                                    OrderDate = reader.IsDBNull(reader.GetOrdinal("OrderDate")) ? null : reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                                    VoucherId = reader.IsDBNull(reader.GetOrdinal("VoucherId")) ? null : reader.GetInt32(reader.GetOrdinal("VoucherId")),
                                    OrderItems = new List<OrderItem>() // sẽ load tiếp bên dưới
                                };
                                orders.Add(order);
                            }
                        }
                    }

                    // 2. Lấy các OrderItem cho từng đơn hàng
                    foreach (var order in orders)
                    {
                        string itemQuery = "SELECT * FROM OrderItem WHERE OrderId = @OrderId";
                        using (var itemCmd = new SqlCommand(itemQuery, conn))
                        {
                            itemCmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                            using (var itemReader = itemCmd.ExecuteReader())
                            {
                                while (itemReader.Read())
                                {
                                    OrderItem item = new OrderItem
                                    {
                                        OrderItemId = itemReader.GetInt32(itemReader.GetOrdinal("OrderItemId")),
                                        OrderId = itemReader.IsDBNull(itemReader.GetOrdinal("OrderId")) ? null : itemReader.GetInt32(itemReader.GetOrdinal("OrderId")),
                                        DrinkId = itemReader.IsDBNull(itemReader.GetOrdinal("DrinkId")) ? null : itemReader.GetInt32(itemReader.GetOrdinal("DrinkId")),
                                        Quantity = itemReader.IsDBNull(itemReader.GetOrdinal("Quantity")) ? null : itemReader.GetInt32(itemReader.GetOrdinal("Quantity")),
                                        Price = itemReader.IsDBNull(itemReader.GetOrdinal("Price")) ? null : itemReader.GetDecimal(itemReader.GetOrdinal("Price"))
                                    };
                                    order.OrderItems.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(OrderDAO), "Lỗi khi lấy đơn hàng theo user.", ex);
            }
            return orders;
        }
    }
}
