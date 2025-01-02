using Subscribe_Store_Job.Models;

namespace Subscribe_Store_Job.Repositories.Abstractions
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(Order order);
        Task<Order> SearchOrder(string orderPk);
    }
}
