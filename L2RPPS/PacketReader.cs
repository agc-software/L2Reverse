using System;
using System.Numerics;
using System.Text;
using System.Windows;

namespace L2RPPS
{
    public class PacketReader
    {
        public PacketReader(byte[] bytes)
            : this(bytes, 0)
        {
        }

        private PacketReader(byte[] bytes, int index)
        {
            _bytes = bytes;
            _index = index;
        }

        public UInt16 ReadUInt16()
        {
            var value = BitConverter.ToUInt16(_bytes, _index);
            _index += 2;
            return value;
        }

        public UInt32 ReadUInt32()
        {
            var value = BitConverter.ToUInt32(_bytes, _index);
            _index += 4;
            return value;
        }

        public UInt64 ReadUInt64()
        {
            var value = BitConverter.ToUInt64(_bytes, _index);
            _index += 8;
            return value;
        }

        public Int16 ReadInt16()
        {
            var value = BitConverter.ToInt16(_bytes, _index);
            _index += 2;
            return value;
        }

        public Int32 ReadInt32()
        {
            var value = BitConverter.ToInt32(_bytes, _index);
            _index += 4;
            return value;
        }

        public Int64 ReadInt64()
        {
            var value = BitConverter.ToInt64(_bytes, _index);
            _index += 8;
            return value;
        }

        public Vector3 ReadVector3()
        {
            var returnVector = new Vector3(0,0,0);
            var x = BitConverter.ToSingle(_bytes, _index);
            _index += 4;
            returnVector.X = x;
            var y = BitConverter.ToSingle(_bytes, _index);
            _index += 4;
            returnVector.Y = y;
            var z = BitConverter.ToSingle(_bytes, _index);
            _index += 4;
            returnVector.Z = z;
            return returnVector;
        }

        public byte ReadByte()
        {
            var value = _bytes[_index];
            _index += 1;
            return value;
        }
        public string ReadString()
        {
            int length = ReadUInt16();
            if (length > 0)
            {
                var value = Encoding.UTF8.GetString(_bytes, _index, length);
                _index += length;
                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        public Single ReadSingle()
        {
            var value = BitConverter.ToSingle(_bytes, _index);
            _index += 4;
            return value;
        }

        // TODO: Consider custom reader extensions for different types instead of embedding interpretation here.
        public DateTime ReadDate()
        {
            var seconds = ReadInt64();
            return seconds > 0 ? new DateTime(1970, 1, 1).AddSeconds(seconds /*- 18000 /*This adjusts timezone to EST. Server time is UTC-2 */) : DateTime.MaxValue;
        }

        public void Skip(int count)
        {
            _index += count;
        }

        public void SetIndex(int newindex)
        {
            _index = newindex;
        }

        public int Remaining => _bytes.Length - _index;

        public byte[] ReadBytes(int length)
        {
            var value = new byte[length];
            Array.Copy(_bytes, _index, value, 0, length);
            _index += length;
            return value;
        }

        public PacketReader Clone()
        {
            return new PacketReader(_bytes, _index);
        }

        private readonly byte[] _bytes;
        private int _index;
    }
}
