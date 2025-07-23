using System;
using System.Collections.Generic;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.Service
{
    public class DrinkService
    {
        private readonly DrinkDAO drinkDAO;
        private readonly ILogger<DrinkService> logger;

        public DrinkService(DrinkDAO drinkDAO, ILogger<DrinkService> logger)
        {
            this.drinkDAO = drinkDAO;
            this.logger = logger;
        }

        public List<Drink> GetAllDrinks()
        {
            try
            {
                return drinkDAO.GetAllDrinks();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi lấy danh sách đồ uống từ service.");
                throw;
            }
        }

        public List<Drink> GetPagedDrinks(int page, int pageSize)
        {
            try
            {
                return drinkDAO.GetPagedDrinks(page, pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi lấy danh sách đồ uống theo trang từ service.");
                throw;
            }
        }

        public List<Drink> GetPagedDrinks(int page, int pageSize, string searchQuery)
        {
            try
            {
                return drinkDAO.GetPagedDrinks(page, pageSize, searchQuery);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Lỗi khi lấy danh sách đồ uống theo trang với tìm kiếm '{searchQuery}' từ service.");
                throw;
            }
        }

        public int GetTotalDrinks()
        {
            try
            {
                return drinkDAO.GetTotalDrinks();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi đếm tổng số đồ uống từ service.");
                throw;
            }
        }

        public int GetTotalDrinks(string searchQuery)
        {
            try
            {
                return drinkDAO.GetTotalDrinks(searchQuery);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Lỗi khi đếm tổng số đồ uống với tìm kiếm '{searchQuery}' từ service.");
                throw;
            }
        }

        public bool AddDrink(Drink drink)
        {
            try
            {
                if (string.IsNullOrEmpty(drink.Name) || drink.Price <= 0)
                {
                    logger.LogWarning("Tên đồ uống hoặc giá không hợp lệ.");
                    return false;
                }

                return drinkDAO.AddDrink(drink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi thêm đồ uống từ service.");
                throw;
            }
        }

        public bool UpdateDrink(Drink drink)
        {
            try
            {
                if (string.IsNullOrEmpty(drink.Name) || drink.Price <= 0)
                {
                    logger.LogWarning("Tên đồ uống hoặc giá không hợp lệ.");
                    return false;
                }

                return drinkDAO.UpdateDrink(drink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi cập nhật đồ uống từ service.");
                throw;
            }
        }

        public bool DeleteDrink(int drinkId)
        {
            try
            {
                return drinkDAO.DeleteDrink(drinkId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi xóa đồ uống từ service.");
                throw;
            }
        }
    }
}