using Microsoft.Extensions.Logging;
using Subscribe_Store_Job.Models;
using Subscribe_Store_Job.OrderServices.Abstractions;
using Subscribe_Store_Job.Repositories.Abstractions;
using System.ComponentModel;
using System.Text.Json;

namespace Subscribe_Store_Job.Processors.Implementations
{
    internal class OrderProcessor : IOrderProcessor
    {
        private IOrderRepository _orderRepo;
        private IInventoryRepository _inventoryRepo;
        private IMatchRepository _matchRepo;
        private ILogger<OrderProcessor> _logger;

        public OrderProcessor(
            IOrderRepository orderRepo,
            IInventoryRepository inventoryRepo,
            IMatchRepository matchRepo,
            ILogger<OrderProcessor> logger
            )
        {
            _orderRepo = orderRepo;
            _inventoryRepo = inventoryRepo;
            _matchRepo = matchRepo;
            _logger = logger;
        }
        public async Task Process(Order order)
        {
            // Add Order
            await _orderRepo.AddOrder(order);
            // Get Inventories
            List<Inventory> inventories = await _inventoryRepo.SearchInventories(order.OrderReleasePk);
            foreach (Inventory i in inventories) 
            {
                string result = JsonSerializer.Serialize(i);
                _logger.LogInformation(result);
            }

            // Calculate Matches
            List<Match> newMatches = CalculateMatches(order, inventories);

            // -- Update Matches
            // -- -- Delete Old Matches
            List<Match> oldMatches = await _matchRepo.SearchMacthes(order.OrderReleasePk);
            await _matchRepo.DeleteMatches(oldMatches);
            // -- -- Add New  Matches
            await _matchRepo.AddMatches(newMatches);
        }

        private List<Match> CalculateMatches(Order order, List<Inventory> inventories) 
        {
            List<Match> matches = new List<Match>();
            foreach (Inventory i in inventories) 
            {
                foreach (OrderReleasePks orderReleasePks in i.OrderReleasePks) 
                {
                    if (orderReleasePks.OrderReleasePk == order.OrderReleasePk) 
                    {
                        matches.Add(new Match { OrderReleasePk = order.OrderReleasePk , MatchPk = Guid.NewGuid().ToString() });
                    }
                }
            }

            return matches;
        }
    }
}
