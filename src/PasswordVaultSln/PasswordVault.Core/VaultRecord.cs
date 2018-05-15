namespace PasswordVault.Core
{
    public class VaultRecord
    {
        public string Title { get; set; }
        public string CipherPassword { get; set; }
        public string Sign { get; set; }
    }
}