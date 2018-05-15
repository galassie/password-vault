using System;
using System.Security;
using System.Collections.Generic;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Core
{
    public class VaultSecureContainer 
    {
        private IVault vault;
        private ICipher cipher;

        public VaultSecureContainer(IVault vault, ICipher cipher)
        {
            this.vault = vault;
            this.cipher = cipher;
        }

        public IEnumerable<string> GetTitles() 
        {
            return vault.GetTitles();
        }

        public void Store(string title, string password, string superPassword) 
        {
            var sign = cipher.CalculateHash(superPassword);
            var cipherPassword = cipher.Encrypt(password, superPassword);
            
            var record = new VaultRecord
            {
                Title = title,
                CipherPassword = cipherPassword,
                Sign = sign
            };

            vault.Store(record);
        }

        public string GetPassword(string title, string superPassword)
        {
            var record = vault.Get(title);            
            var sign = cipher.CalculateHash(superPassword);

            if(record.Sign != sign) throw new InvalidSuperPasswordException();

            return cipher.Decrypt(record.CipherPassword, superPassword);
        }
    }
}