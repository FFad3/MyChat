using System.Net;

namespace MyChatServer
{
    public interface ITCP_Server
    {
        Task Start();

        static abstract ITCP_Server GetInstance(IPAddress iPAddress, int port);
    }
}