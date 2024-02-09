using System.Net.Sockets;
using System.Text;

namespace MyChatServer
{
    internal class PacketBuilder
    {
        //[1byte - op code][4byte-msg lenght(2^32-1)][rest is content]
        //packet[0] -op code
        //packet[1..4] msg lenght
        //packer [4..] msg
        private readonly MemoryStream _ms;

        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);// 1byte
        }

        public void WriteString(string msg)
        {
            var msgLength = (UInt32)msg.Length;
            if (msgLength > UInt32.MaxValue)
            {
                throw new ArgumentException($"Message length exceeds the maximum allowed size of {UInt32.MaxValue} bytes.", nameof(msgLength));
            }

            _ms.Write(BitConverter.GetBytes(msgLength)); //4 bytes
            _ms.Write(Encoding.UTF8.GetBytes(msg)); //rest
        }

        public byte[] GetPacketBytes() => _ms.ToArray();
    }

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
    }
}