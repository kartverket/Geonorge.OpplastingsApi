using Geonorge.OpplastingsApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Geonorge.OpplastingsApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly IHubContext<MessageHub> _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string HeaderName = "signalr-connectionid";

        public MessageService(
            IHubContext<MessageHub> hub,
            IHttpContextAccessor httpContextAccessor)
        {
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendAsync(object message)
        {
            var connectionId = GetConnectionId();

            if (connectionId != null)
                await _hub.Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        private string? GetConnectionId() => _httpContextAccessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();
    }

    public interface IMessageService
    {
        Task SendAsync(object message);
    }
}
