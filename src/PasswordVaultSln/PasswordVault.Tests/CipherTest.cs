using System;
using System.Security;
using System.Collections.Generic;
using PasswordVault.Cipher;
using PasswordVault.Core;
using Xunit;

namespace PasswordVault.Tests
{
    public class CipherTest
    {
        public static IEnumerable<object[]> GetCiphers() 
        {
            yield return new object[] { new SimpleStringCipher() };
            yield return new object[] { new AesCipher() };
        }

        [Fact]
        public void SecureStringLearningTest() 
        {
            var secureString = new SecureString();

            secureString.AppendChar('s');
            secureString.AppendChar('e');
            secureString.AppendChar('c');
            secureString.AppendChar('r');
            secureString.AppendChar('e');
            secureString.AppendChar('t');

            var toString = secureString.ToString();
            Assert.NotEqual("secret", toString);
        }

        [Theory]
        [MemberData(nameof(GetCiphers))]
        public void CipherPlainTextProperlyTest(ICipher cipher)
        {
            var plainText = "Secret Text";
            var passPhrase = "password1";

            var cipherText = cipher.Encrypt(plainText, passPhrase);
            
            Assert.True(plainText != cipherText);
        }
        
        [Theory]
        [MemberData(nameof(GetCiphers))]
        public void CipherAndDecipherTextTest(ICipher cipher)
        {
            var plainText = "Secret Text";
            var passPhrase = "password1";

            var cipherText = cipher.Encrypt(plainText, passPhrase);
            var decipherText = cipher.Decrypt(cipherText, passPhrase);

            Assert.True(plainText == decipherText);
        }

        [Theory]
        [MemberData(nameof(GetCiphers))]
        public void SameStringsGenerateSameHashesTest(ICipher cipher) 
        {
            var stringEqual1 = "Check if this long string will be equal! 1234567890";
            var stringNotEqual = "Check if this long string will be equal! 1234576890";
            var stringEqual2 = "Check if this long string will be equal! 1234567890";

            var hashEqual1 = cipher.CalculateHash(stringEqual1);
            var hashEqual2 = cipher.CalculateHash(stringEqual2);
            var hashNotEqual = cipher.CalculateHash(stringNotEqual);

            Assert.True(hashEqual1 == hashEqual2);
            Assert.False(hashEqual1 == hashNotEqual);
        }
    }
}
