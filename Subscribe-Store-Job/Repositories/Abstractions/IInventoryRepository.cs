using Subscribe_Store_Job.Models;

namespace Subscribe_Store_Job.Repositories.Abstractions
{
    public interface IInventoryRepository
    {
        Task<bool> AddInventory(Inventory inventory);
        Task<List<Inventory>> SearchInventories(long orderReleasePk);
    }
}
