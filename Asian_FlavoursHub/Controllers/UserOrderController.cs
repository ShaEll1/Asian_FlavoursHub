using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Asian_FlavoursHub.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserOrderController> _logger;


        public UserOrderController(IUserOrderRepository userOrderRepo, ICartRepository cartRepo, SignInManager<IdentityUser> signInManager, ILogger<UserOrderController> logger)
        {
            _userOrderRepo = userOrderRepo;
            _cartRepo = cartRepo;
            _signInManager = signInManager;
            _logger = logger;
        }
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepo.UserOrders();
            return View(orders);
        }


        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logout method called."); 

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await _cartRepo.ClearCartAfterLogout(userId);
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}
