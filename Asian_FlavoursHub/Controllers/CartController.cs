using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace Asian_FlavoursHub.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IUserOrderRepository _userOrderRepo;

        public CartController(ICartRepository cartRepo, IUserOrderRepository userOrderRepo)
        {
            _cartRepo = cartRepo;
            _userOrderRepo = userOrderRepo;
        }
        public async Task<IActionResult> addItem(int foodId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(foodId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");

        }






        public async Task<IActionResult> RemoveItem(int foodId)
        {
            var cartCount = await _cartRepo.RemoveItem(foodId);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            try
            {
                int cartItem = await _cartRepo.GetTotalItemInCart();
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error in GetTotalItemInCart action: {ex.Message}");
                return BadRequest("Error getting total items in cart");
            }
        }





        public async Task<IActionResult> Checkout()
        {
            try
            {
                bool isCheckedOut = await _cartRepo.DoCheckOut();

                if (isCheckedOut)
                {
                    // Get the user ID from the current user's claims
                    string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Get items from the cart to create a user order
                    List<CartDetails> cartItems = await _cartRepo.GetCartItems();

                    // Store the cart items in TempData
                    TempData["CartDetailsList"] = Newtonsoft.Json.JsonConvert.SerializeObject(cartItems);

                    // Clear the cart after successful checkout
                    await _cartRepo.ClearCart(userId);

                    // Redirect to the user orders page
                    return RedirectToAction("UserOrders", "UserOrder");
                }
                else
                {
                    // Handle the case where checkout failed, maybe display an error message
                    return RedirectToAction("CheckoutFailed");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error in Checkout action: {ex.Message}");
                return RedirectToAction("CheckoutFailed");
            }
        }
    }
}