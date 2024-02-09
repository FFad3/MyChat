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

        public async Task ConnectToServer(string username, CancellationToken token = default)
        {
            if (!_client.Connected)
            {
                await _client.ConnectAsync(_address, _port, token);
            }
            PacketBuilder packetBuilder = new();
            packetBuilder.WriteOpCode(0);
            packetBuilder.WriteString(username);
            await _client.Client.SendAsync(packetBuilder.GetPacketBytes());
            _ = Task.Run(async () => await HandleCommunicationAsync(), token);
        }

        public event Action<IEnumerable<User>>? ConnectedEvent;

        public event Action<Guid>? DisconectedEvent;

        private async Task HandleCommunicationAsync()
        {
            PacketReader reader = new(_client.GetStream());
            {
                while (_client.Connected)
                {
                    var opCode = (int?)reader.ReadByte();
                    var msg = await reader.ReadMessageAsync();
                    bool isValid = (opCode is not null && !string.IsNullOrEmpty(msg));
                    if (isValid)
                    {
                        switch (opCode)
                        {
                            case 1:
                                var users = JsonSerializer.Deserialize<IEnumerable<User>>(msg);
                                ConnectedEvent?.Invoke(users!);
                                break;

                            case 2:
                                DisconectedEvent?.Invoke(new Guid(msg));
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}