using System;

namespace PasswordVault.Core
{
    public interface ICipher
    {
        string CalculateHash(string text);
        string Encrypt(string text, string password);
        string Decrypt(string cipherText, string password);
    }
}