using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace MikietaApi.Hubs;

public interface IMessageHub
{
    Task OrderMade();
    Task OrderChanged();
    Task ReservationMade();
    Task Join(int id);
}

public class MessageHub : Hub<IMessageHub>
{
    public static readonly ConcurrentDictionary<string, Guid> Dictionary = new();

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Dictionary.TryRemove(Context.ConnectionId, out _);
        
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Join(Guid orderId)
    {
        if (Dictionary.ContainsKey(Context.ConnectionId))
        {
            Dictionary.TryRemove(Context.ConnectionId, out _);
        }

        Dictionary.TryAdd(Context.ConnectionId, orderId);

        await Task.CompletedTask;
    }
}

public static class MessageHubExtensions
{
    public static IMessageHub Clients(this IHubContext<MessageHub, IMessageHub> hub, Guid orderId)
    {
        var connectionIds = new List<string>();
        foreach (var connectionId in MessageHub.Dictionary.Where(x => x.Value == orderId).Select(x => x.Key))
        {
            connectionIds.Add(connectionId);
        }

        return hub.Clients.Clients(connectionIds);
    }
}