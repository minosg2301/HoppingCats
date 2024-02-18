using System;
namespace moonNest
{
    [Serializable]
    public class SafeInt : SafeValue
    {
        public SafeInt(int value)
        {
            Value = value;
        }

        public int Value
        {
            get
            {
                InitAes();
                return dataProtector.DecryptToInt(bytes);
            }

            set
            {
                InitAes();
                bytes = dataProtector.Encrypt(value);
            }
        }
    }
}