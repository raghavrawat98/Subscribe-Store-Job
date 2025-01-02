using Subscribe_Store_Job.Models;

namespace Subscribe_Store_Job.OrderServices.Abstractions
{
    public interface IOrderProcessor
    {
        Task Process(Order order);
    }
}
