namespace Asian_FlavoursHub.Repositories
{
    public interface ICartRepository
    {
        Task<int > AddItem(int foodId, int qty);
        Task<int> RemoveItem(int foodId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string uderId = "");
        Task<int> GetTotalItemInCart(string userId = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckOut();
        Task ClearCart(string userId); 
        Task ClearCartAfterLogout(string userId);

        Task<List<CartDetails>> GetCartItems();

    }
}
