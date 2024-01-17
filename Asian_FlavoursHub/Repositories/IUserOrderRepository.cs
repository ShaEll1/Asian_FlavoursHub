namespace Asian_FlavoursHub.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
        Task CreateUserOrder(List<CartDetails> cartItems);
    }

   

}