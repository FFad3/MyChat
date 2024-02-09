using System.Collections.Concurrent;
using System.Net.Sockets;

namespace MyChatServer
{
    public class SessionManager
    {
        public event Action? SessionCreated;

        public event Action<Guid>? SessionRemoved;

        private readonly ConcurrentDictionary<Guid, Session> _sessions = new();

        public Session CreateSession(TcpClient clientSocket) => new(clientSocket, this);

        public void AddSession(Session session)
        {
            _sessions.TryAdd(session.Id, session);
            SessionCreated?.Invoke();
        }

        public void RemoveSession(Session session)
        {
            _sessions.TryRemove(session.Id, out _);
            SessionRemoved?.Invoke(session.Id);
        }

        public Session? Get(Guid id)
        {
            _sessions.TryGetValue(id, out Session? session);
            return session;
        }

        public int Count()
        {
            _sessions.TryGetNonEnumeratedCount(out int count);
            return count;
        }

        public IEnumerable<Session> GetAll(Func<Session, bool>? predicate = null)
        {
            if (predicate == null)
                return _sessions.Values;
            return _sessions.Values.Where(predicate);
        }
    }
}