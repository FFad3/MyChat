using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace MyChatServer
{
    public sealed class TCP_Server : ITCP_Server
    {
        private static ITCP_Server? _instance;

        public static ITCP_Server GetInstance(IPAddress iPAddress, int port)
        {
            return _instance ??= new TCP_Server(iPAddress, port);
        }

        public IPAddress Address { get; }
        public int Port { get; }

        public IPEndPoint EndPoint { get; }
        private readonly TcpListener _listener;

        public bool IsRunning { get; private set; } = false;

        private readonly SessionManager _sessionManager = new();

        private TCP_Server(IPAddress iPAddress, int port)
        {
            Address = iPAddress;
            Port = port;

            EndPoint = new IPEndPoint(Address, Port);

            _listener = new TcpListener(EndPoint);

            _sessionManager.SessionCreated += BroadcastConneections;
            _sessionManager.SessionRemoved += BroadcastConnectionDisposed;
        }

        private async void BroadcastConnectionDisposed(Guid guid)
        {
            var sessions = _sessionManager.GetAll();
            var broadCastPacket = new PacketBuilder();
            broadCastPacket.WriteOpCode(2);
            broadCastPacket.WriteString(guid.ToString());
            var packetBytes = broadCastPacket.GetPacketBytes();

            var sendTasks = new List<Task>();

            foreach (var session in sessions)
            {
                sendTasks.Add(session.ClientSocket.Client.SendAsync(packetBytes));
            }

            await Task.WhenAll(sendTasks);
        }

        public async Task Start()
        {
            Console.WriteLine("Starting");
            _listener.Start();
            IsRunning = true;
            Console.WriteLine("Server started");

            await HandleNewConnectionsAsync();
        }

        public void Stop()
        {
            IsRunning = false;
            _listener.Stop();
        }

        private async Task HandleNewConnectionsAsync()
        {
            while (IsRunning)
            {
                TcpClient clientSocket = await _listener.AcceptTcpClientAsync();
                Session session = _sessionManager.CreateSession(clientSocket);
                await session.Start();
                _sessionManager.AddSession(session);
                //broadcast client join
                //await BroadcastConneections();
            }
        }

        private async void BroadcastConneections()
        {
            var sessions = _sessionManager.GetAll();
            var jsonPayload = JsonSerializer.Serialize(sessions.Select(x => new { x.Id, x.Name }));

            var broadCastPacket = new PacketBuilder();
            broadCastPacket.WriteOpCode(1);
            broadCastPacket.WriteString(jsonPayload);
            var packetBytes = broadCastPacket.GetPacketBytes();

            var sendTasks = new List<Task>();

            foreach (var session in sessions)
            {
                sendTasks.Add(session.ClientSocket.Client.SendAsync(packetBytes));
            }

            await Task.WhenAll(sendTasks);
        }
    }
}