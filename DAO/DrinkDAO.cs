using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using CAFEHOLIC.Model;
using CAFEHOLIC.dao;

namespace CAFEHOLIC.DAO
{
    public class DrinkDAO
    {
        private readonly ILogger<DrinkDAO> logger;
        private readonly DBContext context;

        public DrinkDAO(DBContext dBContext, ILogger<DrinkDAO> logger)
        {
            this.context = dBContext ?? throw new ArgumentNullException(nameof(dBContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<Drink> GetAllDrinks()
        {
            List<Drink> drinks = new List<Drink>();
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[GetAllDrinks] Connection opened, State: {0}", conn.State);
                    string query = "SELECT * FROM Drink WHERE IsAvailable = 1";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Drink drink = new Drink
                                {
                                    DrinkId = reader.GetInt32(reader.GetOrdinal("DrinkId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                    IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? null : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                    img = reader.IsDBNull(reader.GetOrdinal("img")) ? string.Empty : reader.GetString(reader.GetOrdinal("img"))
                                };
                                drinks.Add(drink);
                            }
                        }
                    }
                    logger.LogInformation($"[GetAllDrinks] Loaded {drinks.Count} drinks.");
                    return drinks;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[GetAllDrinks] Error loading drinks. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public int GetTotalDrinks()
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[GetTotalDrinks] Connection opened, State: {0}", conn.State);
                    string query = "SELECT COUNT(*) FROM Drink WHERE IsAvailable = 1";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        int total = result != null ? Convert.ToInt32(result) : 0;
                        logger.LogInformation($"[GetTotalDrinks] Query: {query}, Total drinks with IsAvailable = 1: {total}");
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[GetTotalDrinks] Error counting total drinks. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public int GetTotalDrinks(string searchQuery)
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[GetTotalDrinks] Connection opened, State: {0}, SearchQuery: {1}", conn.State, searchQuery);
                    string query = "SELECT COUNT(*) FROM Drink WHERE IsAvailable = 1";
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        query += " AND (LOWER(Name) LIKE @SearchQuery OR LOWER(Description) LIKE @SearchQuery)";
                    }
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery.ToLower()}%");
                        }
                        var result = cmd.ExecuteScalar();
                        int total = result != null ? Convert.ToInt32(result) : 0;
                        logger.LogInformation($"[GetTotalDrinks] Query: {query}, Total drinks: {total}");
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[GetTotalDrinks] Error counting total drinks with search query '{searchQuery}'. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public List<Drink> GetPagedDrinks(int page, int pageSize)
        {
            try
            {
                List<Drink> drinks = new List<Drink>();
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[GetPagedDrinks] Connection opened, State: {0}, Page: {1}, PageSize: {2}", conn.State, page, pageSize);
                    string query = @"
                    SELECT * FROM Drink 
                    WHERE IsAvailable = 1 
                    ORDER BY DrinkId 
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Drink drink = new Drink
                                {
                                    DrinkId = reader.GetInt32(reader.GetOrdinal("DrinkId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                    IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? null : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                    img = reader.IsDBNull(reader.GetOrdinal("img")) ? string.Empty : reader.GetString(reader.GetOrdinal("img"))
                                };
                                drinks.Add(drink);
                            }
                        }
                    }
                    logger.LogInformation("[GetPagedDrinks] Loaded {Count} drinks for page {Page}, pageSize {PageSize}.", drinks.Count, page, pageSize);
                    return drinks;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[GetPagedDrinks] Error loading drinks for page {page}, pageSize {pageSize}. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public List<Drink> GetPagedDrinks(int page, int pageSize, string searchQuery)
        {
            try
            {
                List<Drink> drinks = new List<Drink>();
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[GetPagedDrinks] Connection opened, State: {0}, Page: {1}, PageSize: {2}, SearchQuery: {3}", conn.State, page, pageSize, searchQuery);
                    string query = @"
                    SELECT * FROM Drink 
                    WHERE IsAvailable = 1";
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        query += " AND (LOWER(Name) LIKE @SearchQuery OR LOWER(Description) LIKE @SearchQuery)";
                    }
                    query += " ORDER BY DrinkId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);
                        if (!string.IsNullOrWhiteSpace(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery.ToLower()}%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Drink drink = new Drink
                                {
                                    DrinkId = reader.GetInt32(reader.GetOrdinal("DrinkId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                    IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? null : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                    img = reader.IsDBNull(reader.GetOrdinal("img")) ? string.Empty : reader.GetString(reader.GetOrdinal("img"))
                                };
                                drinks.Add(drink);
                            }
                        }
                    }
                    logger.LogInformation($"[GetPagedDrinks] Loaded {drinks.Count} drinks for page {page}, pageSize {pageSize}, searchQuery '{searchQuery}'.");
                    return drinks;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[GetPagedDrinks] Error loading drinks for page {page}, pageSize {pageSize}, searchQuery '{searchQuery}'. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public bool AddDrink(Drink drink)
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[AddDrink] Connection opened, State: {0}, Drink: {1}", conn.State, drink.Name);
                    string query = @"INSERT INTO Drink (Name, Description, Price, IsAvailable, img) 
                                    VALUES (@Name, @Description, @Price, @IsAvailable, @img)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", drink.Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", drink.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", drink.Price ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsAvailable", drink.IsAvailable ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@img", drink.img ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        logger.LogInformation($"[AddDrink] Rows affected: {rowsAffected}, Drink: {drink.Name}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[AddDrink] Error adding drink: {drink.Name}. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public bool UpdateDrink(Drink drink)
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[UpdateDrink] Connection opened, State: {0}, DrinkId: {1}", conn.State, drink.DrinkId);
                    string query = @"UPDATE Drink 
                                    SET Name = @Name, Description = @Description, Price = @Price, 
                                        IsAvailable = @IsAvailable, img = @img 
                                    WHERE DrinkId = @DrinkId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DrinkId", drink.DrinkId);
                        cmd.Parameters.AddWithValue("@Name", drink.Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", drink.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", drink.Price ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsAvailable", drink.IsAvailable ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@img", drink.img ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        logger.LogInformation($"[UpdateDrink] Rows affected: {rowsAffected}, DrinkId: {drink.DrinkId}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[UpdateDrink] Error updating drink: {drink.DrinkId}. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }

        public bool DeleteDrink(int drinkId)
        {
            try
            {
                using (var conn = context.GetConnection())
                {
                    logger.LogInformation("[DeleteDrink] Connection opened, State: {0}, DrinkId: {1}", conn.State, drinkId);
                    string query = "UPDATE Drink SET IsAvailable = 0 WHERE DrinkId = @DrinkId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DrinkId", drinkId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        logger.LogInformation($"[DeleteDrink] Rows affected: {rowsAffected}, DrinkId: {drinkId}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[DeleteDrink] Error deleting drink: {drinkId}. InnerException: {0}", ex.InnerException?.Message);
                throw;
            }
        }
    }
}