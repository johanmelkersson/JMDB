namespace JMDB.Services
{
    public class UserTracker
    {
        // userId → antal aktiva anslutningar
        private readonly Dictionary<string, int> _connections = new();
        private readonly object _lock = new();

        public int Count
        {
            get { lock (_lock) return _connections.Count; }
        }

        public int Add(string userId)
        {
            lock (_lock)
            {
                _connections.TryGetValue(userId, out var count);
                _connections[userId] = count + 1;
                return _connections.Count;
            }
        }

        public int Remove(string userId)
        {
            lock (_lock)
            {
                if (_connections.TryGetValue(userId, out var count))
                {
                    if (count <= 1)
                        _connections.Remove(userId);
                    else
                        _connections[userId] = count - 1;
                }
                return _connections.Count;
            }
        }
    }
}
