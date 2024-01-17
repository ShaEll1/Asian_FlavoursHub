using System.Threading.Tasks;

namespace Asian_FlavoursHub
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Food>> GetFoods(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Category>> Categories();
        Task<Food> GetFoodDetails(int foodId);
    }
}