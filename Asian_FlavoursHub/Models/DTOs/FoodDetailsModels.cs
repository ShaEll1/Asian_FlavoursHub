namespace Asian_FlavoursHub.Models.DTOs
{
    public class FoodDetailsModels
    {
        public Food FoodDescription { get; set; }
        
        public IEnumerable<Food> Foods { get; set; }
        public IEnumerable<Category> Categories { get; set; }
       
    }
}
