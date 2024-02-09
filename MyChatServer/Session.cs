using System.Net.Sockets;

namespace MyChatServer
{
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
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
            string username = await _packetReader.ReadMessageAsync();
            if (opCode == 0)
            {
                await Console.Out.WriteLineAsync($"[{opCode}] User: {username} connected");
                this.Name = username;
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
                        await Console.Out.WriteLineAsync($"[{opCode}] {msg}");
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