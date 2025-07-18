using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAFEHOLIC.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO
{
    public class DrinkDAO
    {
        private readonly ILogger<DrinkDAO> logger;
        private readonly DBContext context;
        public DrinkDAO(DBContext dBContext, ILogger<DrinkDAO> logger)
        {
            this.context = dBContext;
            this.logger = logger;
        }
        public List<Drink> GetAllDrinks()
        {
            List<Drink> drinks = new List<Drink>();
            try
            {
                using (var conn = context.GetConnection())
                {
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
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi lấy danh sách đồ uống.");
            }
            return drinks;
        }
        public void ApplySearch(String SearchKeyword, List<Drink> allDrinks, ObservableCollection<Drink> Drinks)
        {
            var filtered = string.IsNullOrWhiteSpace(SearchKeyword)
                ? allDrinks
                : allDrinks.Where(d => d.Name != null && d.Name.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)).ToList();
            Drinks.Clear();
            foreach (var drink in filtered)
            {
                Drinks.Add(drink);
            }
        }
    }
}
