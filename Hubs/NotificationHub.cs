using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Hubs
{
  public class NotificationHub : Hub
  {
    // You can add methods here to send notifications
    public async Task SendNotification(string message)
    {
      await Clients.All.SendAsync("ReceiveNotification", message);
    }
  }
}
