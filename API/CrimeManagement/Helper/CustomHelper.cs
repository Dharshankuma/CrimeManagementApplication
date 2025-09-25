using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Cryptography;
using System.Text;

namespace CrimeManagement.Helper
{

    public static class CustomHelper
    {
        private static byte[] _encryptionKey;

        public static string _success = "success";
        public static string _failure = "failure";

        public static void InitializeKey(IConfiguration config)
        {
            string? keyString = config["EncryptionSettings:Key"];

            if(string.IsNullOrEmpty(keyString))
            {
                throw new Exception("No Encryption Key Found");
            }

            _encryptionKey = Encoding.UTF8.GetBytes(keyString);

            if (!(_encryptionKey.Length == 16 || _encryptionKey.Length == 24 || _encryptionKey.Length == 32))
                throw new Exception("AES Key must be 16, 24, or 32 bytes long.");

        }
        public static byte[] GenerateRandomKey(int length)
        {
            byte[] bytes = new byte[length];
            using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
        public static string Encrypt(string text)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.GenerateIV();

                ICryptoTransform encrypt = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        public static string Decrypt(string cipherText)
        {
            byte[] cipher = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                byte[] iv = new byte[16];

                Array.Copy(cipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                byte[] acutal = new byte[cipher.Length - iv.Length];
                Array.Copy(cipher, iv.Length, acutal, 0, acutal.Length);

                ICryptoTransform decrypt = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string DoGenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public static DateTime DoGetDateTime()
        {
            return DateTime.Now;
        }
    }
}
