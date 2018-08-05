using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using PasswordVault.Core;
using PasswordVault.Vault;
using PasswordVault.Cipher;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Tests
{
    public class VaultSecureContainerTest
    {
        [Fact]
        public void GetTitlesIsSameAsVaultGetTitlesTest() 
        {
            var vault = new InMemoryVault();
            vault.Store(new VaultRecord
            {
                Title = "Test",
                CipherPassword = "qweiyqpwuepyqw",
                Sign = "12ug30172hy012"
            });
            var cipher = new SimpleStringCipher();

            var container = new VaultSecureContainer(vault, cipher);

            Assert.Equal(vault.GetTitles(), container.GetTitles());
        }

        [Fact]
        public void StoreFunctionInsertProperlyTheVaultRecordTest() 
        {
            var vault = new InMemoryVault();
            var cipher = new SimpleStringCipher();
            var container = new VaultSecureContainer(vault, cipher);

            var title = "Twitter";
            var password = "password";
            var superPassword = "1234567890";
            container.Store(title, password, superPassword);

            var record = vault.Get("Twitter");

            Assert.NotNull(record);
            Assert.Equal(title, record.Title);
            Assert.Equal(password, cipher.Decrypt(record.CipherPassword, superPassword));            
            Assert.Equal(cipher.CalculateHash(superPassword), record.Sign);       
        }

        [Fact]
        public void GetPasswordProperlyReturnPasswordTest() 
        {
            var vault = new InMemoryVault();
            var cipher = new SimpleStringCipher();
            var container = new VaultSecureContainer(vault, cipher);

            var title = "Twitter";
            var password = "password";
            var superPassword = "1234567890";
            container.Store(title, password, superPassword);
            var retrievedPassword = container.GetPassword(title, superPassword);

            Assert.Equal(password, retrievedPassword);           
        }

        [Fact]
        public void ThrowInvalidSuperPasswordExceptionWhenSuperPasswordIsWrong()
        {
            var vault = new InMemoryVault();
            var cipher = new SimpleStringCipher();
            var container = new VaultSecureContainer(vault, cipher);

            var title = "Twitter";
            var password = "password";
            var superPassword = "1234567890";
            container.Store(title, password, superPassword);

            Assert.Throws<InvalidSuperPasswordException>(() => container.GetPassword(title, "wrong"));         
        }

        [Fact]
        public void RemoveTitleShouldRemoveRecordFromVaultIfPassowrdIsCorrect() 
        {
            var vault = new InMemoryVault();
            var cipher = new SimpleStringCipher();
            var container = new VaultSecureContainer(vault, cipher);

            var title = "Twitter";
            var password = "password";
            var superPassword = "1234567890";
            container.Store(title, password, superPassword);

            Assert.True(container.GetTitles().Count() == 1);

            Assert.Throws<InvalidSuperPasswordException>(() => container.RemoveTitle(title, "wrong"));

            container.RemoveTitle(title, superPassword);
            Assert.True(container.GetTitles().Count() == 0);
        }
    }
}