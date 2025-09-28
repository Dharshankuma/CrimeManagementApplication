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
        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.GenerateIV();

                using (MemoryStream ms = new MemoryStream())
                {
                    // First write IV
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;

                byte[] iv = new byte[aes.BlockSize / 8]; // AES block size is 16 bytes
                byte[] cipher = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream(cipher))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static DateTime? ParseDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            return DateTime.ParseExact(date, "dd-MM-yyyy", null);
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
