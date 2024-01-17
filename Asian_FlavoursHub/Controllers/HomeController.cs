using Asian_FlavoursHub.Models;
using Asian_FlavoursHub.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Asian_FlavoursHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int categoryId = 0)
        {
            IEnumerable<Food> foods;

            if (categoryId > 0)
            {
                foods = await _homeRepository.GetFoods("", categoryId); // Get all foods for the selected category
            }
            else
            {
                foods = await _homeRepository.GetFoods(sterm, categoryId); // Get all foods without filtering by category
            }

            IEnumerable<Category> categories = await _homeRepository.Categories();
            FoodDisplayModel foodModel = new FoodDisplayModel
            {
                Foods = foods,
                Categories = categories,
                SelectedCategoryId = categoryId,
                SearchTerm = sterm
            };

            return View(foodModel);
        }


        public async Task<IActionResult> About()
        {
            var categories = await _homeRepository.Categories();
            var model = new AboutViewModel { Categories = categories.ToList() };
            
            return View(model);

        }

        //public async Task<IActionResult> About()
        //{
        //    var categories = await _homeRepository.Categories();
        //    var model = categories.ToList();
        //    return View(model);
        //}


        public async Task<IActionResult> Contact()
        {
            var categories = await _homeRepository.Categories();
            var model = new ContactViewModel { Categories = categories.ToList() };

            return View(model);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> FoodDetails(int foodId)
        {
            // Fetch the food details from data source based on the foodId
            var foodDetails = await _homeRepository.GetFoodDetails(foodId);

            // Fetch categories
            var categories = await _homeRepository.Categories();

            // Create an instance of FoodDetailsModel
            var foodDetailsModel = new FoodDetailsModels
            {
                FoodDescription = foodDetails,
                Categories = categories
            };

            return View(foodDetailsModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(ContactViewModel model)
        {
            // Process the form submission and send the email

            // Fetch categories from the repository
            var categories = await _homeRepository.Categories();

            // Populate the Categories property of the ContactViewModel
            model.Categories = categories.ToList();

            // Redirect to the desired page with the ContactViewModel
            return View("Contact", model);
        }




    }
}