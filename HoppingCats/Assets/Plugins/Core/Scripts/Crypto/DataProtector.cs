using System;
using System.IO;
using System.Security.Cryptography;
namespace moonNest
{
    public class DataProtector
    {
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        public DataProtector(AesManaged myAes)
        {
            encryptor = myAes.CreateEncryptor();
            decryptor = myAes.CreateDecryptor();
        }

        public byte[] Encrypt(double data)
        {
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    var bytes = BitConverter.GetBytes(data);
                    csEncrypt.Write(BitConverter.GetBytes(data), 0, bytes.Length);
                }
                encrypted = msEncrypt.ToArray();
            }
            return encrypted;
        }

        public double DecryptToDouble(byte[] encryptedData)
        {
            byte[] bytes = new byte[8];
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.Read(bytes, 0, bytes.Length);
                }
            }
            return BitConverter.ToDouble(bytes, 0);
        }

        public byte[] Encrypt(int data)
        {
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    var bytes = BitConverter.GetBytes(data);
                    csEncrypt.Write(BitConverter.GetBytes(data), 0, bytes.Length);
                }
                encrypted = msEncrypt.ToArray();
            }
            return encrypted;
        }

        public int DecryptToInt(byte[] encryptedData)
        {
            byte[] bytes = new byte[4];
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.Read(bytes, 0, bytes.Length);
                }
            }
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}