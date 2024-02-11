using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MyChatApp.IO;
using MyChatApp.MVVM.Models;

namespace MyChatApp.Net
{
    public class Server
    {
        private readonly TcpClient _client;
        private readonly int _port = 9999;
        private readonly IPAddress _address = IPAddress.Loopback;

        public Server()
        {
            _client = new TcpClient();
        }

        public Server(IPAddress adreess, int port)
        {
            _port = port;
            _address = adreess;
            _client = new TcpClient();
        }

        public async Task ConnectToServerAsync(UserContext userContext, CancellationToken token = default)
        {
            if (!_client.Connected)
            {
                await _client.ConnectAsync(_address, _port, token);
            }
            PacketBuilder packetBuilder = new();
            packetBuilder.WriteOpCode(0);
            var json = JsonSerializer.Serialize(userContext);
            packetBuilder.WriteString(json);
            await _client.Client.SendAsync(packetBuilder.GetPacketBytes());
            _ = Task.Run(async () => await HandleCommunicationAsync(), token);
        }

        public async Task SendMessageAsync(Message message, CancellationToken token = default)
        {
            if (!_client.Connected)
            {
                await _client.ConnectAsync(_address, _port, token);
            }
            PacketBuilder packetBuilder = new();
            packetBuilder.WriteOpCode(3);
            packetBuilder.WriteString(JsonSerializer.Serialize(message));
            await _client.Client.SendAsync(packetBuilder.GetPacketBytes());
        }

        public event Action<IEnumerable<User>>? OnConnected;

        public event Action<Guid>? OnDisconectedEvent;

        public event Action<int, string?>? OnMessageRecived;

        private async Task HandleCommunicationAsync()
        {
            PacketReader reader = new(_client.GetStream());
            {
                while (_client.Connected)
                {
                    var opCode = reader.ReadByte();
                    var msg = await reader.ReadMessageAsync();

                    OnMessageRecived?.Invoke(opCode, msg);
                }
            }
        }
    }
}