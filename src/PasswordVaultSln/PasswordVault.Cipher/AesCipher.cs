using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using PasswordVault.Core;

// Credits to https://www.codeproject.com/Tips/1156169/Encrypt-Strings-with-Passwords-AES-SHA

namespace PasswordVault.Cipher
{
  public class AesCipher : ICipher 
  {
    public string CalculateHash(string text) 
    {
      using(var sha2 = SHA256.Create())
      {
        var rawText = Encoding.UTF8.GetBytes(text);
        var hash = sha2.ComputeHash(rawText);
        return Convert.ToBase64String(hash);
      }
    }

    public string Encrypt(string text, string password) {
      string cipherText = null;
      var keys = GetHashKeys(password);

      try {
        cipherText = EncryptStringToBytes_Aes(text, keys[0], keys[1]);
      } catch (CryptographicException) { } catch (ArgumentNullException) { }

      return cipherText;
    }

    public string Decrypt (string cipherText, string password) {
      string text = null;
      var keys = GetHashKeys(password);

      try {
        text = DecryptStringFromBytes_Aes(cipherText, keys[0], keys[1]);
      } catch (CryptographicException) { } catch (ArgumentNullException) { }

      return text;
    }

    private byte[][] GetHashKeys (string key) {
      var result = new byte[2][];

      using(var sha2 = SHA256.Create())
      {
        var rawKey = Encoding.UTF8.GetBytes(key);
        var rawIV = Encoding.UTF8.GetBytes(key);

        var hashKey = sha2.ComputeHash(rawKey);
        var hashIV = sha2.ComputeHash(rawIV);

        Array.Resize(ref hashIV, 16);

        result[0] = hashKey;
        result[1] = hashIV;
      }

      return result;
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV) {
      if(plainText == null || plainText.Length <= 0)
        throw new ArgumentNullException("plainText");
      if(Key == null || Key.Length <= 0)
        throw new ArgumentNullException("Key");
      if(IV == null || IV.Length <= 0)
        throw new ArgumentNullException("IV");

      byte[] encrypted;

      using(AesManaged aesAlg = new AesManaged()) 
      {
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (var msEncrypt = new MemoryStream())
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
          using (var swEncrypt = new StreamWriter(csEncrypt))
            swEncrypt.Write(plainText);

          encrypted = msEncrypt.ToArray();
        }
      }

      return Convert.ToBase64String(encrypted);
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string DecryptStringFromBytes_Aes(string cipherText, byte[] Key, byte[] IV) {
      var rawCipherText = Convert.FromBase64String(cipherText);

      if (rawCipherText == null || rawCipherText.Length <= 0)
        throw new ArgumentNullException("cipherText");
      if (Key == null || Key.Length <= 0)
        throw new ArgumentNullException("Key");
      if (IV == null || IV.Length <= 0)
        throw new ArgumentNullException("IV");

      string plaintext = null;

      using (var aesAlg = Aes.Create())
      {
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using (var msDecrypt = new MemoryStream(rawCipherText))
        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
          plaintext = srDecrypt.ReadToEnd();
      }

      return plaintext;
    }
  }
}