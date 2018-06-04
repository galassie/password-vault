using System;
using System.Linq;
using PasswordVault.Core;
using PasswordVault.Vault;
using PasswordVault.Cipher;

namespace PasswordVault.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = ConfigureContainer();

            var exit = false;
            while (!exit) 
            {
                var input = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(input)) exit = true;
                else
                {
                    var elements = input.Split(" ");
                    var command = GetCommand(container, elements[0], elements.Skip(1).ToArray());
                    command();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Finish!");
        }

        static VaultSecureContainer ConfigureContainer() 
        {
            var vault = new FileSystemVault("C:\\Temp", "store.vault");
            var cipher = new AesCipher();

            return new VaultSecureContainer(vault, cipher);
        }
        
        static Action GetCommand(VaultSecureContainer container, string commandText, params string[] args)
        {
            switch (commandText) 
            {
                case "store": 
                    return () => container.Store(args[0], args[1], args[2]);
                
                case "titles": 
                    return () => 
                    {
                        foreach (var title in container.GetTitles())
                        {
                            Console.WriteLine(title);
                        }
                    };
                
                case "read":
                    return () => Console.WriteLine(container.GetPassword(args[0], args[1]));

                default: 
                    return () => Console.WriteLine("Command not recognized!");
            }
        }
    }
}
