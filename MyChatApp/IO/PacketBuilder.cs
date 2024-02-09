using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyChatApp.IO
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
}
