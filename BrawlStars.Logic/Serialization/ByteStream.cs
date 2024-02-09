namespace BrawlStars.Logic.Serialization;

using System.Buffers.Binary;
using System.Text;

public ref struct ByteStream
{
    private readonly Span<byte> _buffer;
    private int _position;
    private int _bitPosition;

    public readonly int Offset => _position;

    public ByteStream(Span<byte> buffer)
    {
        _buffer = buffer;
    }

    public void WriteByte(byte value)
    {
        _bitPosition = 0;
        _buffer[_position++] = value;
    }

    public void WriteBoolean(bool value)
    {
        if (_bitPosition == 0)
            _buffer[_position++] = 0;

        if (value)
            _buffer[_position - 1] |= (byte)(1 << _bitPosition);

        _bitPosition = (_bitPosition + 1) % 8;
    }

    public void WriteShort(short value)
    {
        _bitPosition = 0;

        BinaryPrimitives.WriteInt16BigEndian(_buffer.Slice(_position, 2), value);
        _position += 2;
    }

    public void WriteInt(int value)
    {
        _bitPosition = 0;

        BinaryPrimitives.WriteInt32BigEndian(_buffer.Slice(_position, 4), value);
        _position += 4;
    }

    public void WriteString(string? value)
    {
        _bitPosition = 0;

        if (value == null)
        {
            WriteInt(-1);
            return;
        }
        else if (value.Length == 0)
        {
            WriteInt(0);
            return;
        }

        var size = Encoding.UTF8.GetByteCount(value);
        WriteInt(size);

        if (_buffer.Length - _position < size)
            throw new OverflowException();

        Encoding.UTF8.GetBytes(value, _buffer.Slice(_position, size));
        _position += size;
    }

    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        _bitPosition = 0;

        bytes.CopyTo(_buffer.Slice(_position, bytes.Length));
        _position += bytes.Length;
    }

    public void WriteVInt(int value)
    {
        _bitPosition = 0;

        int tmp = (value >> 25) & 0x40;
        int flipped = value ^ (value >> 31);

        tmp |= value & 0x3F;
        value >>= 6;

        if ((flipped >>= 6) == 0)
        {
            _buffer[_position++] = (byte)tmp;
            return;
        }

        _buffer[_position++] = (byte)(tmp | 0x80);

        do
        {
            _buffer[_position++] = (byte)((value & 0x7F) | ((flipped >>= 7) != 0 ? 0x80 : 0));
            value >>= 7;
        } while (flipped != 0);
    }

    public byte ReadByte()
    {
        _bitPosition = 0;
        return _buffer[_position++];
    }

    public bool ReadBoolean()
    {
        if (_bitPosition == 0)
            ++_position;

        bool value = (_buffer[_position] & (1 << _bitPosition)) != 0;
        _bitPosition = (_bitPosition + 1) % 8;

        return value;
    }

    public short ReadShort()
    {
        _bitPosition = 0;

        var result = BinaryPrimitives.ReadInt16BigEndian(_buffer.Slice(_position, 2));
        _position += 2;

        return result;
    }

    public int ReadInt()
    {
        _bitPosition = 0;

        var result = BinaryPrimitives.ReadInt32BigEndian(_buffer.Slice(_position, 4));
        _position += 4;

        return result;
    }

    public string ReadString()
    {
        _bitPosition = 0;

        var size = ReadInt();
        if (size is <= 0 or > 900000)
        {
            if (size > 900000)
                throw new IOException($"Too long string encountered, length: {size}");

            return string.Empty;
        }

        var result = Encoding.UTF8.GetString(_buffer.Slice(_position, size));
        _position += size;

        return result;
    }

    public byte[] ReadBytes(int length)
    {
        _bitPosition = 0;

        var bytes = new byte[length];
        _buffer.Slice(_position, length).CopyTo(bytes);
        _position += length;

        return bytes;
    }

    public int ReadVInt()
    {
        _bitPosition = 0;

        int b, sign = ((b = _buffer[_position++]) >> 6) & 1, i = b & 0x3F, offset = 6;

        for (int j = 0; j < 4 && (b & 0x80) != 0; j++, offset += 7)
        {
            i |= ((b = _buffer[_position++]) & 0x7F) << offset;
        }

        return (b & 0x80) != 0 ? -1 : i | (sign == 1 && offset < 32 ? i | (int)(0xFFFFFFFF << offset) : i);
    }
}
