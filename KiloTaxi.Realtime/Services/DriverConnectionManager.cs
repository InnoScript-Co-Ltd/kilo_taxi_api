namespace KiloTaxi.Realtime.Services
{
    public class DriverConnectionManager
    {
        private readonly Dictionary<string, string> _connections = new();

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

    }
}
