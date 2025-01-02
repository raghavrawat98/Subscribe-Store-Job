using Subscribe_Store_Job.Models;
using Subscribe_Store_Job.Repositories.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Subscribe_Store_Job.Repositories.Implementations
{
    public class OrderRepostiory : IOrderRepository
    {
        private IRedisRepository _redisRepo;

        public OrderRepostiory(
            IRedisRepository redisRepo
            )
        {
            _redisRepo = redisRepo;
        }
        public async Task<bool> AddOrder(Subscribe_Store_Job.Models.Order order)
        {
            string orderKey = $"OrderPk:{order.OrderPk}";
            return await _redisRepo.SetValueByKey(orderKey, order);
        }

        public async Task<Subscribe_Store_Job.Models.Order> SearchOrder(string orderPk)
        {
            string result = await _redisRepo.GetValueByKey($"OrderPk:{orderPk}");
            return JsonSerializer.Deserialize<Order>(result);
        }
    }
}
