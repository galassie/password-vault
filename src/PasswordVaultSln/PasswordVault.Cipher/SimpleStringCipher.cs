using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security;
using PasswordVault.Core;

// Credits to https://mikaelkoskinen.net/post/encrypt-decrypt-string-asp-net-core

namespace PasswordVault.Cipher
{
    public class SimpleStringCipher : ICipher
    {
        public string Encrypt(string plainText, string passPhrase)
        {
            var key = CreateKey(passPhrase);

            using (var aesAlg = Aes.Create())
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                var iv = aesAlg.IV;
                var decryptedContent = msEncrypt.ToArray();
                var result = new byte[iv.Length + decryptedContent.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
                return Convert.ToBase64String(result);
            }
        }

        public string Decrypt(string cipherText, string passPhrase)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = CreateKey(passPhrase);

            using (var aesAlg = Aes.Create())
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            using (var msDecrypt = new MemoryStream(cipher))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }

        public string CalculateHash(string input)
        {
            using(var shaHash = SHA256.Create()) 
            {
                byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        private static readonly byte[] Salt = new byte[] { 10, 20, 30 , 40, 50, 60, 70, 80};
        private static byte[] CreateKey(string passPhrase, int keyBytes = 32)
        {
            const int Iterations = 300;
            var keyGenerator = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(passPhrase), Salt, Iterations);
            return keyGenerator.GetBytes(keyBytes);
        }
    }
}