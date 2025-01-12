namespace KiloTaxi.Realtime.Services
{
    public class DriverConnectionManager
    {
        private readonly Dictionary<string, string> _connections = new();
        private readonly Dictionary<string, Action<bool>> _responseSubscriptions = new();


        public void AddConnection(string key, string connectionId)
        {
            _connections[key] = connectionId;
        }

        public void RemoveConnection(string connectionId)
        {
            var item = _connections.FirstOrDefault(x => x.Value == connectionId);
            if (item.Key != null)
            {
                _connections.Remove(item.Key);
            }
        }

        public string? GetConnectionId(string key)
        {
            _connections.TryGetValue(key, out var connectionId);
            return connectionId;
        }
        
        public string? GetVehiclId(string connectionId)
        {
            var item = _connections.FirstOrDefault(x => x.Value == connectionId);
            return item.Key;
        }
        public void SubscribeToDriverResponse(string connectionId, Action<bool> callback)
        {
            _responseSubscriptions[connectionId] = callback;
        }
        public void UnsubscribeFromDriverResponse(string connectionId)
        {
            _responseSubscriptions.Remove(connectionId);
        }
        public void NotifyOrderAccepted(string connectionId, string orderId)
        {
            if (_responseSubscriptions.TryGetValue(connectionId, out var handler))
            {
                handler(true); // Notify the subscriber that the order was accepted
            }
        }
       
    }
}
