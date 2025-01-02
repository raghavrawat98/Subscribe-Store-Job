using NRedisStack.Search;
using Subscribe_Store_Job.Models;
using Subscribe_Store_Job.Repositories.Abstractions;
using System.Text.Json;

namespace Subscribe_Store_Job.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private IRedisRepository _redisRepo;
        private const string InventoryIndex = "InventoryIndex";
        public InventoryRepository(
            IRedisRepository redisRepo
            )
        {
            _redisRepo = redisRepo;
        }
        public async Task<bool> AddInventory(Inventory inventory)
        {
            string materialKey = $"MaterialPk:{inventory.MaterialPk}";
            return await _redisRepo.SetValueByKey(materialKey, inventory);
        }

        public async Task<List<Inventory>> SearchInventories(long orderReleasePk)
        {
            Query query = new Query($"@OrderReleasePk:[{orderReleasePk} {orderReleasePk}]");
            string result = await _redisRepo.GetValueByIndexQuery(InventoryIndex, query);
            return JsonSerializer.Deserialize<List<Inventory>>(result);
        }
    }
}
