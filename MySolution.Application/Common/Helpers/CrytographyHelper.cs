using System.Security.Cryptography;
using System.Text;

namespace MySolution.Application.Common.Helpers;

public class CrytographyHelper
{
    public static class CryptographyHelper
    {
        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string EncryptStringToBytes_Aes(string plainText, string key)
        {
            var keyInbytes = Encoding.UTF8.GetBytes(key);
            var IVInBytes = Encoding.UTF8.GetBytes(Reverse(key));

            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (keyInbytes == null || keyInbytes.Length <= 0)
                throw new ArgumentException("keyInbytes");
            if (IVInBytes == null || IVInBytes.Length <= 0)
                throw new ArgumentException("IVInBytes");

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = keyInbytes;
                aesAlg.IV = IVInBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                }

                encrypted = msEncrypt.ToArray();
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptStringFromBytes_Aes(string cipherText, string key)
        {
            var keyInbytes = Encoding.UTF8.GetBytes(key);
            var IVInBytes = Encoding.UTF8.GetBytes(Reverse(key));

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (keyInbytes == null || keyInbytes.Length <= 0)
                throw new ArgumentException("keyInbytes");
            if (IVInBytes == null || IVInBytes.Length <= 0)
                throw new ArgumentException("keyInbytes");

            string plaintext = null;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = keyInbytes;
                aesAlg.IV = IVInBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }

        private static string Reverse(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string Decrypt(string textEncrypted, string key)
        {
            try
            {
                var passwordDecrypted = DecryptStringFromBytes_Aes(textEncrypted, key);
                return passwordDecrypted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string Encrypt(string rawText, string key)
        {
            try
            {
                var passwordEncrypted = EncryptStringToBytes_Aes(rawText, key);
                return passwordEncrypted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GenerateSecret(string prefix = "wp_sec_", int size = 32)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(size);
            var sb = new StringBuilder(size * 2);

            foreach (var b in randomBytes) sb.Append(Base62Chars[b % Base62Chars.Length]);

            return prefix + sb;
        }
    }
}