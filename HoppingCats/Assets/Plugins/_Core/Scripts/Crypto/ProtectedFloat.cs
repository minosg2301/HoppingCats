using System;
using Random = UnityEngine.Random;

namespace moonNest
{
    public struct ProtectedFloat
    {
        static readonly byte[] entropy;
        static ProtectedFloat()
        {
            entropy = new byte[32];
            for (int i = 0; i < entropy.Length; i++)
            {
                entropy[i] = (byte)Random.Range(1, 128);
            }
        }

        readonly byte[] encrypted;
        readonly byte pos;

        float Value
        {
            get
            {
                Decrypt(encrypted, pos);
                float val = BitConverter.ToSingle(encrypted, 0);
                Encrypt(encrypted, pos);
                return val;
            }
        }

        public bool IsNull => encrypted == null;

        public ProtectedFloat(float value)
        {
            encrypted = BitConverter.GetBytes(value);
            pos = (byte)Random.Range(0, 29);
            Encrypt(encrypted, pos);
        }

        float AddValue(float b)
        {
            float a = Value;
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

        public static implicit operator float(ProtectedFloat protectedFloat)
        {
            return protectedFloat.Value;
        }

        public static implicit operator ProtectedFloat(float value)
        {
            return new ProtectedFloat(value);
        }

        public static float operator +(ProtectedFloat protectedFloat, float b)
        {
            return protectedFloat.AddValue(b);
        }

        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
    }
}