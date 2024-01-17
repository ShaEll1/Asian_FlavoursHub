namespace Asian_FlavoursHub.Models.DTOs
{
    public class FoodDisplayModel
    {
        public IEnumerable<Food> Foods { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int SelectedCategoryId { get; set; }

        public string SearchTerm { get; set; }
    }
}
