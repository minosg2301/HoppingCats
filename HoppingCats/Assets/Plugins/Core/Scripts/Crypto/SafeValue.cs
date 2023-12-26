using System.Security.Cryptography;
using UnityEngine;

namespace moonNest
{
    public abstract class SafeValue
    {
        private static byte[] key = new byte[] { 108, 158, 199, 52, 148, 181, 165, 4, 147, 242, 16, 239, 254, 15, 58, 125, 177, 207, 171, 19, 184, 208, 239, 203, 217, 252, 100, 166, 130, 105, 42, 19 };
        private static byte[] iv = new byte[] { 213, 93, 37, 158, 241, 247, 195, 145, 137, 192, 159, 170, 107, 177, 104, 176 };

        [SerializeField] protected byte[] bytes = new byte[8];

        protected DataProtector dataProtector;

        public void InitAes()
        {
            if (dataProtector == null)
            {
                AesManaged aes = new AesManaged();
                if (key.Length == 0) { key = aes.Key; iv = aes.IV; }
                else { aes.Key = key; aes.IV = iv; }
                dataProtector = new DataProtector(aes);
            }
        }
    }
}