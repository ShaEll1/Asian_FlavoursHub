using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Asian_FlavoursHub.Models;

namespace Asian_FlavoursHub.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpcontextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpcontextAccessor = httpcontextAccessor;
        }

        public async Task<int> AddItem(int foodId, int qty)
        {
            try
            {
                string userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User not logged in");
                    return -1; // Return 
                }

                var cart = await GetCart(userId);

                if (cart == null)
                {
                    // Create a new cart
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                    };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync(); // Save the new cart to the database
                }

                // Explicitly load the CartDetails
                await _db.Entry(cart).Collection(c => c.CartDetails).LoadAsync();

                // Print existing cart details for debugging
                Console.WriteLine($"Existing Cart Details: {string.Join(", ", cart.CartDetails.Select(item => $"{item.FoodId}({item.Quantity})"))}");

                var cartItem = cart.CartDetails?.FirstOrDefault(item => item.FoodId == foodId);

                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var food = _db.Foods.Find(foodId);
                    var newCartItem = new CartDetails
                    {
                        FoodId = foodId,
                        Quantity = qty,
                        ShoppingCartId = cart.Id,
                        UnitPrice = food.Price 
                    };
                    _db.CartDetails.Add(newCartItem);
                }

                await _db.SaveChangesAsync(); // Save changes to the database

                // Reload the cart to get the updated details
                cart = await GetCart(userId);

                // Print updated cart details for debugging
                Console.WriteLine($"Updated Cart Details: {string.Join(", ", cart.CartDetails.Select(item => $"{item.FoodId}({item.Quantity})"))}");

                // Return the count directly
                return cart?.CartDetails?.Sum(item => item.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddItem method: {ex.Message}");
                return -2; // Indicates an error occurred
            }
        }



        public async Task<int> RemoveItem(int foodId)
        {
            string userId = GetUserId();
            try
            {

                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User Not Logged-In");

                var cart = await GetCart(userId);
                if (cart == null)

                    throw new Exception("Invalied Cart");

                var cartItems = _db.CartDetails
                                    .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.FoodId == foodId);
                if (cartItems == null)
                    throw new Exception("No Items in the Cart");

                else if (cartItems.Quantity == 1)
                    _db.CartDetails.Remove(cartItems);

                else
                    cartItems.Quantity = cartItems.Quantity - 1;

                _db.SaveChanges();


            }
            catch (Exception ex)
            {
               
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        //{
        //    var userId = GetUserId();
        //    if (userId == null)
        //        throw new Exception("Invalid User");

        //    var ShoppingCart = await _db.ShoppingCarts
        //        .Include(a => a.CartDetails)
        //        .ThenInclude(a => a.Food)
        //        .ThenInclude(a => a.Category)
        //        .Where(a => a.UserId == userId).FirstOrDefaultAsync();

        //    return ShoppingCart;
        //}
        {
            {
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid userid");
                }

                var shoppingCart = await _db.ShoppingCarts
                                            .Include(a => a.CartDetails)
                                            .ThenInclude(a => a.Food)
                                            .ThenInclude(a => a.Category)
                                            .Where(a => a.UserId == userId)
                                            .FirstOrDefaultAsync();

                if (shoppingCart != null)
                {
                    // Explicitly load the CartDetails
                    await _db.Entry(shoppingCart).Collection(c => c.CartDetails).LoadAsync();
                }

                return shoppingCart;
            }

        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join CartDetails in _db.CartDetails
                              on cart.Id equals CartDetails.ShoppingCartId
                              select new { CartDetails.Id }
                             ).ToListAsync();
            return data.Count();
        }
       

        public async Task<bool> DoCheckOut()
        {
            Console.WriteLine("DoCheckOut method called.");

            using var transaction = _db.Database.BeginTransaction();

            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User not logged in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("Invalid Cart");

                var cartDetails = _db.CartDetails
                    .Where(a => a.ShoppingCartId == cart.Id).ToList();

                if (cartDetails.Count == 0)
                    throw new Exception("Cart is Empty");

                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    OrderStatusId = 1 // pending
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var item in cartDetails)
                {
                    var orderDetails = new OrderDetails
                    {
                        FoodId = item.FoodId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    _db.OrderDetails.Add(orderDetails);
                }

                _db.SaveChanges();

                // Removing cart details
                _db.CartDetails.RemoveRange(cartDetails);
                _db.SaveChanges();

                transaction.Commit();

                // Clear the cart after successful checkout
                ClearCart(userId);

                Console.WriteLine("Checkout successful.");
                return true;


            }
            catch (Exception )
            {
                
                return false;
            }
        }




        private string GetUserId()
        
            //var user = _httpcontextAccessor.HttpContext.User;
            //var userId = _userManager.GetUserId(user);
            //return userId;

            {
                return _httpcontextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

        public async Task ClearCart(string userId)
        {
            try
            {
                var cart = await GetCart(userId);
                if (cart != null)
                {
                    _db.CartDetails.RemoveRange(cart.CartDetails);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClearCart method: {ex.Message}");
                
            }
        }


        public async Task ClearCartAfterLogout(string userId)
        {
            try
            {
                var cart = await GetCart(userId);
                if (cart != null)
                {
                    _db.CartDetails.RemoveRange(cart.CartDetails);
                    await _db.SaveChangesAsync();
                    Console.WriteLine("Cart cleared after logout.");
                }
                else
                {
                    Console.WriteLine("No cart found for the user after logout.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClearCartAfterLogout method: {ex.Message}");
                throw; // Rethrow the exception to propagate it to the calling code
            }
        }


        public async Task<List<CartDetails>> GetCartItems()
        {
            try
            {
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid userid");
                }

                var cart = await _db.ShoppingCarts
                    .Include(a => a.CartDetails)
                    .ThenInclude(a => a.Food)
                    .ThenInclude(a => a.Category)
                    .Where(a => a.UserId == userId)
                    .FirstOrDefaultAsync();

                if (cart != null)
                {
                    // Explicitly load the CartDetails
                    await _db.Entry(cart).Collection(c => c.CartDetails).LoadAsync();
                    return cart.CartDetails.ToList();
                }

                return new List<CartDetails>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCartItems method: {ex.Message}");
                // Handle the exception as needed
                return new List<CartDetails>();
            }
        }


        public async Task<int> GetTotalItemInCart(string userId = "")
        {
            userId = GetUserId(); // Remove the 'var'
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not logged in");

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


    }
}
