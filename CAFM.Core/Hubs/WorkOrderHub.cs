using CAFM.Database.Models;
using Microsoft.AspNetCore.SignalR;

namespace CAFM.Core.Hubs
{
    public class WorkOrderHub : Hub
    {
        // Method to send real-time updates to clients in a specific group (companyId + locationId)
        public async Task NotifyWorkOrderUpdate(long workOrderId, string message, int companyId, long locationId)
        {
            // Create a unique group name for each company and location
            var groupName = $"Company_{companyId}_Location_{locationId}";

            // Send the message to the specific group
            await Clients.Group(groupName).SendAsync("ReceiveWorkOrderUpdate", workOrderId, message);
        }

        // Method to handle client subscriptions by location or company
        public async Task SubscribeToWorkOrderUpdates(int companyId, long locationId)
        {
            // Create a unique group name for each company and location
            var groupName = $"Company_{companyId}_Location_{locationId}";

            // Add the client to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveWorkOrderUpdate", "Subscribe To Work Order Updates ");
        }

        // Method to unsubscribe from work order updates
        public async Task UnsubscribeFromWorkOrderUpdates(int companyId, long locationId)
        {
            var groupName = $"Company_{companyId}_Location_{locationId}";

            // Remove the client from the group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
