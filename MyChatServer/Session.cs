using System.Net.Sockets;
using System.Text.Json;

namespace MyChatServer
{
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public TcpClient ClientSocket { get; set; }
        private bool IsRunning = false;
        private readonly SessionManager _sessionManager;
        private readonly PacketReader _packetReader;

        public Session(TcpClient clientSocket, SessionManager sessionManager)
        {
            Id = Guid.NewGuid();
            ClientSocket = clientSocket;
            _sessionManager = sessionManager;
            _packetReader = new(this.ClientSocket.GetStream());
        }

        internal async Task Start()
        {
            IsRunning = true;

            int opCode = _packetReader.ReadByte();
            string json = await _packetReader.ReadMessageAsync();
            UserContext? context = JsonSerializer.Deserialize<UserContext>(json);

            if (opCode == 0)
            {
                await Console.Out.WriteLineAsync($"[{opCode}] User: {context?.Username} connected");
                this.Name = context?.Username ?? "UNKNOWN";
                this.Color = context?.Color ?? "#FF0000";
            }

            _ = Task.Run(async () => await HandleCommunicationAsync());
        }

        private async Task HandleCommunicationAsync()
        {
            try
            {
                PacketReader reader = new(ClientSocket.GetStream());
                {
                    while (IsRunning)
                    {
                        if (!ClientSocket.Connected)
                        {
                            IsRunning = false;
                        }

                        var opCode = reader.ReadByte();
                        var msg = await reader.ReadMessageAsync();

                        var sessions = _sessionManager.GetAll();
                        var broadCastPacket = new PacketBuilder();
                        broadCastPacket.WriteOpCode(opCode);
                        broadCastPacket.WriteString(msg ?? string.Empty);

                        var packetBytes = broadCastPacket.GetPacketBytes();
                        foreach (var session in sessions)
                        {
                            await session.ClientSocket.Client.SendAsync(packetBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Connection lost {Id} {Name}");
            }
            finally
            {
                await Console.Out.WriteLineAsync();
                ClientSocket.Close();
                ClientSocket.Dispose();
                _sessionManager.RemoveSession(this);
            }
        }
    }
}