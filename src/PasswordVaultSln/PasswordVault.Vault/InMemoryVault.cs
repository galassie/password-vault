using System;
using System.Linq;
using System.Collections.Generic;
using PasswordVault.Core;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Vault
{
    public class InMemoryVault : IVault
    {
        private Dictionary<string, VaultRecord> vault;

        public InMemoryVault()
        {
            vault = new Dictionary<string, VaultRecord>();    
        }

        public VaultRecord Get(string title)
        {
            if(!vault.ContainsKey(title)) throw new TitleNotFoundException(title);
            return vault[title];
        }

        public IEnumerable<string> GetTitles()
        {
            return vault.Keys.ToList();
        }

        public void Store(VaultRecord record)
        {
            if(vault.ContainsKey(record.Title)) throw new TitleAlreadyPresentException(record.Title);
            vault.Add(record.Title, record);
        }

        public bool Remove(string title)
        {
            return vault.Remove(title);
        }
    }
}