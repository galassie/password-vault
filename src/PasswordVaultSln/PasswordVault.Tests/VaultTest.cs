using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using PasswordVault.Core;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Tests
{
    public class VaultTest
    {
        public static IEnumerable<object[]> GetVaults()
        {
            yield return new object[] 
            { 
                new InMemoryVault() 
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
}