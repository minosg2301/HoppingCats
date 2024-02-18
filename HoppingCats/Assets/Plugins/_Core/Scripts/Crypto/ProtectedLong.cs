using System;
using Random = UnityEngine.Random;

namespace moonNest
{
    public struct ProtectedLong
    {
        static readonly byte[] entropy;
        static ProtectedLong()
        {
            entropy = new byte[32];
            for (int i = 0; i < entropy.Length; i++)
            {
                entropy[i] = (byte)Random.Range(1, 128);
            }
        }

        readonly byte[] encrypted;
        readonly byte pos;

        long Value
        {
            get
            {
                Decrypt(encrypted, pos);
                int val = BitConverter.ToInt32(encrypted, 0);
                Encrypt(encrypted, pos);
                return val;
            }
        }

        public bool IsNull => encrypted == null;

        public ProtectedLong(long value)
        {
            encrypted = BitConverter.GetBytes(value);
            pos = (byte)Random.Range(0, 25);
            Encrypt(encrypted, pos);
        }

        long AddValue(long b)
        {
            long a = Value;
            return a + b;
        }

        static void Encrypt(byte[] data, int pos)
        {
            for (byte i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos + i];
            }
        }

        static void Decrypt(byte[] data, int pos)
        {
            for (byte i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos + i];
            }
        }

        public static implicit operator long(ProtectedLong protectedLong)
        {
            return protectedLong.Value;
        }

        public static implicit operator ProtectedLong(long value)
        {
            return new ProtectedLong(value);
        }

        public static long operator +(ProtectedLong protectedLong, long b)
        {
            return protectedLong.AddValue(b);
        }

        public override string ToString() => Value.ToString();
    }
}