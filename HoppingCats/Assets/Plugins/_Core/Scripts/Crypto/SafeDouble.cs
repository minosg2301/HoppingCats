using System;
namespace moonNest
{
    [Serializable]
    public class SafeDouble : SafeValue
    {
        public SafeDouble(double value)
        {
            Value = value;
        }

        public double Value
        {
            get
            {
                InitAes();
                return dataProtector.DecryptToDouble(bytes);
            }

            set
            {
                InitAes();
                bytes = dataProtector.Encrypt(value);
            }
        }
    }
}

