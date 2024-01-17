using Microsoft.EntityFrameworkCore;

namespace Asian_FlavoursHub.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }
        public async Task<IEnumerable<Food>> GetFoods(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Food> foods = await (
                from food in _db.Foods
                join category in _db.Categories
                    on food.CategoryId equals category.Id  
                where string.IsNullOrWhiteSpace(sTerm) || (food != null && food.FoodName.ToLower().Contains(sTerm))
                select new Food
                {
                    Id = food.Id,
                    Image = food.Image,
                    FoodName = food.FoodName,
                    Description = food.Description,
                    ShortDescription = food.ShortDescription,
                    Price = food.Price,
                    CategoryName = category.CategoryName,
                    CategoryId = category.Id  

                }
            ).ToListAsync();

            if (categoryId > 0)
            {
                foods = foods.Where(a => a.CategoryId == categoryId).ToList(); 
            }
            return foods;
        }
        public async Task<Food> GetFoodDetails(int foodId)
        {
            return await _db.Foods.FindAsync(foodId);
        }

        public async Task<Food> GetFoodImage(int foodId)
        {
            return await _db.Foods.FindAsync(foodId);
        }
    }
}
