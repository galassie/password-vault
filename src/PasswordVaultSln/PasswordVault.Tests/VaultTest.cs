using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using PasswordVault.Core;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Tests
{
    public class VaultTest : IClassFixture<VaultTestFixture>
    {
        public static IEnumerable<object[]> GetVaults()
        {
            yield return new object[] 
            { 
                new InMemoryVault() 
            };
            yield return new object[] 
            { 
                new FileSystemVault(VaultTestFixture.VAULT_PATH) 
            };
        }

        [Theory]
        [MemberData(nameof(GetVaults))]
        public void VaultsFunctionalityTest(IVault vault)
        {
            var titles = vault.GetTitles();
            
            Assert.Empty(titles);

            var title = "Test";
            var cipherPassword = "piu3781620837'8achas"; 
            var sign = "jo1h23o7tb120d301823801"; 
            var record = new VaultRecord 
            {
                Title = title,
                CipherPassword = cipherPassword,
                Sign = sign
            };
            vault.Store(record);
            titles = vault.GetTitles();

            Assert.True(titles.Count() == 1);
            Assert.True(titles.ElementAt(0) == title);
            Assert.Throws<TitleAlreadyPresentException>(() => vault.Store(record));

            record = vault.Get(title);

            Assert.Equal(title, record.Title);
            Assert.Equal(cipherPassword, record.CipherPassword);
            Assert.Equal(sign, record.Sign);

            Assert.Throws<TitleNotFoundException>(() => vault.Get("Title not present"));
        }
    }

    public class VaultTestFixture : IDisposable 
    {
        public static readonly string VAULT_PATH = "store.vault";

        public VaultTestFixture()
        {
            DeleteVaultIfExist();
            File.Create(VAULT_PATH).Close();
        }

        public void Dispose()
        {
            DeleteVaultIfExist();
        }

        private void DeleteVaultIfExist() 
        {
            if (File.Exists(VAULT_PATH)) File.Delete(VAULT_PATH);
        }
    }
}