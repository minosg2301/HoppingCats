using System;
namespace moonNest
{
    [Serializable]
    public class SafeLong : SafeValue
    {
        public SafeLong(long value)
        {
            Value = value;
        }

        public long Value
        {
            get
            {
                InitAes();
                return dataProtector.DecryptToLong(bytes);
            }

            set
            {
                InitAes();
                bytes = dataProtector.Encrypt(value);
            }
        }
    }
}