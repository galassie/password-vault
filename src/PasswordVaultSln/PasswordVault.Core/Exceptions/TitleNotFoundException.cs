using System;

namespace PasswordVault.Core.Exceptions
{
    public class TitleNotFoundException : Exception
    {
        public TitleNotFoundException(string title)
            : base($"Title not found in the vault: {title}")
        { }

        public TitleNotFoundException(string title, Exception innerException)
            : base($"Title not found in the vault: {title}", innerException)
        { }
    }
}