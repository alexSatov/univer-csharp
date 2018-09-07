using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
	public class ReadonlyBytes : IEnumerable<byte>
	{
		private readonly byte[] bytes;

        public int Length { get { return bytes.Length; } }

		public ReadonlyBytes(byte[] bytes)
		{
			this.bytes = bytes;
		}

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < bytes.Length; i++)
                yield return bytes[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= bytes.Length) throw new IndexOutOfRangeException();
                return bytes[index];
            }
            set
            {
                if (index < 0 || index >= bytes.Length) throw new IndexOutOfRangeException();
                bytes[index] = value;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < bytes.Length; i++)
                hashCode += bytes[i] * (int)Math.Pow(10, i);
            return hashCode;
        }
    }
}