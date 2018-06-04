using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using PasswordVault.Core;
using PasswordVault.Core.Exceptions;

namespace PasswordVault.Vault
{
    public class FileSystemVault : IVault
    {
        public string VaultLocation { get; private set; }
        private char separator = '@';

        public FileSystemVault(string directory, string file)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            VaultLocation = Path.Combine(directory, file);   
            if (!File.Exists(VaultLocation)) File.Create(VaultLocation).Close();
        }

        public VaultRecord Get(string title)
        {
            var result = RetrieveVaultRecord(title);
            if (result == null) throw new TitleNotFoundException(title);

            return result;
        }

        public IEnumerable<string> GetTitles()
        {
            var result = new List<string>();

            using (var sr = File.OpenText(VaultLocation)) 
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var splits = line.Split(separator);

                    if (splits.Count() == 3) result.Add(splits[0]);
                }
            }
            
            return result;
        }

        public void Store(VaultRecord record)
        {
            var result = RetrieveVaultRecord(record.Title);
            if (result != null) throw new TitleAlreadyPresentException(record.Title);

            using (var sw = File.AppendText(VaultLocation))
            {
                sw.WriteLine($"{record.Title}@{record.CipherPassword}@{record.Sign}");
            }
        }

        private VaultRecord RetrieveVaultRecord(string title) 
        {
            using (var sr = File.OpenText(VaultLocation))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var splits = line.Split(separator);

                    if (splits.Count() == 3 && splits[0] == title) 
                        return new VaultRecord{
                            Title = splits[0],
                            CipherPassword = splits[1],
                            Sign = splits[2]
                        };
                }
            }
            return null;
        }
    }
}