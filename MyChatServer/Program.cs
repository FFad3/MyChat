using System.Net;
using MyChatServer;

namespace MyMyChatServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            ITCP_Server server = TCP_Server.GetInstance(IPAddress.Loopback, 9999);
            await server.Start();
        }
    }
}