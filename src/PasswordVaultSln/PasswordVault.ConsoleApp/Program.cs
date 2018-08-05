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
            var vault = new FileSystemVault(".", "store.vault");
            var cipher = new AesCipher();

            return new VaultSecureContainer(vault, cipher);
        }
        
        static Action GetCommand(VaultSecureContainer container, string commandText, params string[] args)
        {
            Action result;
            switch (commandText) 
            {
                case "store": 
                    result = () => container.Store(args[0], args[1], args[2]);
                    break;
                case "titles": 
                    result = () => 
                    {
                        foreach (var title in container.GetTitles())
                        {
                            Console.WriteLine(title);
                        }
                    };
                    break;
                case "read":
                    result =  () => Console.WriteLine(container.GetPassword(args[0], args[1]));
                    break;
                case "remove":
                    result =  () => container.RemoveTitle(args[0], args[1]);
                    break;
                default: 
                    result = () => Console.WriteLine("Command not recognized!");
                    break;
            }

            return CreateCommandContainer(result);
        }

        static Action CreateCommandContainer(Action command) 
        {
            return () => {
                try {
                    command();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            };
        }
    }
}
