using Microsoft.AspNetCore.SignalR;

namespace EventTicket.Orchestrator.Hubs;

public class BookingHub : Hub
{
    public async Task JoinBookingGroup(string bookingId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, bookingId);
    }
}
