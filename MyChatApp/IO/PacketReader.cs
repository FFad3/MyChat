using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyChatApp.IO
{
    internal class PacketReader : BinaryReader
    {
        private readonly NetworkStream _ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public async Task<string> ReadMessageAsync()
        {
            var length = ReadInt32();
            byte[] msgBuffer = new byte[length];
            await _ns.ReadAsync(msgBuffer.AsMemory(0, length));
            return Encoding.UTF8.GetString(msgBuffer);
        }

        public async Task<byte[]> ReadBytesAsync()
        {
            var length = ReadInt32();
            byte[] msgBuffer = new byte[length];
            await _ns.ReadAsync(msgBuffer.AsMemory(0, length));
            return msgBuffer;
        }
    }
}