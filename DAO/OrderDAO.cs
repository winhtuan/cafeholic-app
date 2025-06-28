using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.dao;
using CAFEHOLIC.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO
{
    public class OrderDAO
    {
        private readonly ILogger<OrderDAO> logger;
        private readonly DBContext context;
        public OrderDAO(DBContext dBContext, ILogger<OrderDAO> logger)
        {
            this.context = dBContext;
            this.logger = logger;
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
                logger.LogError(ex, "Lỗi khi lấy đơn hàng theo user.");
            }
            return orders;
        }

    }
}
