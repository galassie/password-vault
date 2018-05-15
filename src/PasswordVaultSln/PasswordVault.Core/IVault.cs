using System;
using System.Collections.Generic;

namespace PasswordVault.Core
{
    public interface IVault
    {
        IEnumerable<string> GetTitles(); 
        VaultRecord Get(string title);
        void Store(VaultRecord record);
    }
}