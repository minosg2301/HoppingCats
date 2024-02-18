using System;
using Random = UnityEngine.Random;

namespace moonNest
{
    public struct ProtectedInt
    {
        static readonly byte[] entropy;
        static ProtectedInt()
        {
            entropy = new byte[32];
            for (int i = 0; i < entropy.Length; i++)
            {
                entropy[i] = (byte)Random.Range(1, 128);
            }
        }

        readonly byte[] encrypted;
        readonly byte pos;

        int Value
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

        public ProtectedInt(int value)
        {
            encrypted = BitConverter.GetBytes(value);
            pos = (byte)Random.Range(0, 29);
            Encrypt(encrypted, pos);
        }

        int AddValue(int b)
        {
            int a = Value;
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

        public static implicit operator int(ProtectedInt protectedInt)
        {
            return protectedInt.Value;
        }

        public static implicit operator ProtectedInt(int value)
        {
            return new ProtectedInt(value);
        }

        public static int operator +(ProtectedInt protectedInt, int b)
        {
            return protectedInt.AddValue(b);
        }

        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
    }
}