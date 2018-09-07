using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        public override bool CanRead => underlyingStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => underlyingStream.Length;
        public override long Position
        {
            get { return underlyingStream.Position; }
            set { throw new InvalidOperationException();}
        }

        private bool isSectionFound;
        private bool isSectionReaded;
        private readonly byte[] buffer;
        private List<byte> cachedBytes;
        private readonly string section;
        private readonly Stream baseStream;
        private readonly Stream underlyingStream;

        public ResourceReaderStream(Stream stream, string section)
        {
            baseStream = stream;
            isSectionFound = false;
            isSectionReaded = false;
            this.section = section;
            underlyingStream = stream;
            cachedBytes = new List<byte>();
            buffer = new byte[Constants.BufferSize];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (isSectionReaded)
                return 0;
            if (!isSectionFound)
                FindSection();

            while (cachedBytes.Count < count)
            {
                if (!TryReadFromBaseStream()) break;
            }

            count = Math.Min(count, cachedBytes.Count);
            count = FromCacheToBuffer(buffer, offset, count);

            return count;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private int FromCacheToBuffer(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count - 1; i++)
            {
                if (cachedBytes[i] == 0 && cachedBytes[i + 1] == 1)
                {
                    isSectionReaded = true;
                    cachedBytes.RemoveRange(0, i + 2);
                    return i;
                }

                if (cachedBytes[i] == 0 && cachedBytes[i + 1] == 0)
                    cachedBytes.RemoveAt(i);

                buffer[i + offset] = cachedBytes[i];
            }

            buffer[count - 1] = cachedBytes[count - 1];
            cachedBytes.RemoveRange(0, count);
            return count;
        }

        private bool TryReadFromBaseStream()
        {
            var count = baseStream.Read(buffer, 0, buffer.Length);

            if (count == 0) return false;

            cachedBytes.AddRange(buffer.Take(count));
            return true;
        }

        private void FindSection()
        {
            while (true)
            {
                var someSection = Encoding.ASCII.GetString(ReadBytesBeforeSeparator());

                if (someSection == section)
                {
                    isSectionFound = true;
                    return;
                }

                if (ReadBytesBeforeSeparator() == null)
                    throw new ArgumentException($"В базовом потоке нет секции с названием {section}");
            }
        }

        private byte[] ReadBytesBeforeSeparator()
        {
            while (true)
            {
                if (!TryReadFromBaseStream()) break;

                for (var i = 0; i < cachedBytes.Count - 1; i++)
                {
                    if (cachedBytes[i] == 0 && cachedBytes[i + 1] == 1)
                    {
                        var byteLine = cachedBytes.Take(i).ToArray();
                        cachedBytes.RemoveRange(0, i + 2);
                        return byteLine;
                    }

                    if (cachedBytes[i] == 0 && cachedBytes[i + 1] == 0)
                        cachedBytes.RemoveAt(i);
                }
            }

            var lastByteLine = cachedBytes.ToArray();
            cachedBytes = null;
            return lastByteLine;
        }
    }
}
