using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        public override bool CanRead => toRead;
        public override bool CanWrite => !toRead;
        public override long Length => baseStream.Length;
        public override bool CanSeek => false;

        public override long Position
        {
            get { return baseStream.Position; }
            set { throw new InvalidOperationException(); }
        }

        private int tail;
        private readonly bool toRead;
        private readonly Stream baseStream;
        private readonly byte[] innerBuffer;
        private readonly List<byte> readedBytes;

        public CustomCompressionStream(Stream baseStream, bool toRead)
        {
            tail = -1;
            this.toRead = toRead;
            this.baseStream = baseStream;
            innerBuffer = new byte[1024];
            readedBytes = new List<byte>();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!toRead)
                throw new InvalidOperationException("Попытка прочесть данные через поток для записи!");

            while (readedBytes.Count < count)
            {
                int cnt;
                if (tail == -1)
                    cnt = baseStream.Read(innerBuffer, 0, innerBuffer.Length);
                else
                {
                    cnt = baseStream.Read(innerBuffer, 1, innerBuffer.Length);
                    innerBuffer[0] = (byte) tail;
                    tail = -1;
                }
                if (cnt == 0) break;
                if (cnt%2 == 1)
                    tail = innerBuffer[cnt - 1];

                Decompress(readedBytes, innerBuffer.Take(cnt).ToArray());
            }

            count = Math.Min(readedBytes.Count, count);
            var data = readedBytes.Take(count).ToArray();
            readedBytes.RemoveRange(0, count);

            for (var i = 0; i < data.Length; i++)
                buffer[i + offset] = data[i];

            return data.Length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (toRead)
                throw new InvalidOperationException("Попытка записать данные через поток для чтения!");

            var data = Compress(buffer);
            baseStream.Write(data, 0, data.Length);
        }

        private byte[] Compress(byte[] data)
        {
            byte repetitionsCount = 1;
            var repeatedValue = data[0];
            var compressedData = new List<byte>();

            for (var i = 1; i < data.Length; i++)
            {
                if (data[i] != repeatedValue || repetitionsCount == 255)
                {
                    compressedData.Add(repetitionsCount);
                    compressedData.Add(repeatedValue);
                    repeatedValue = data[i];
                    repetitionsCount = 1;
                }
                else
                    repetitionsCount++;
            }
            compressedData.Add(repetitionsCount);
            compressedData.Add(repeatedValue);
            return compressedData.ToArray();
        }

        private static void Decompress(List<byte> decompressedData, byte[] data)
        {
            for (var i = 0; i < data.Length - 1; i += 2)
            {
                decompressedData.AddRange(Enumerable.Repeat(data[i + 1], data[i]));
            }
        }

        public override void Flush()
        {
           
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }
    }
}