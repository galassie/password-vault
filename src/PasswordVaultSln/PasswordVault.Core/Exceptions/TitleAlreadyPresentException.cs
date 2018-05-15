using System;

namespace PasswordVault.Core.Exceptions
{
    public class TitleAlreadyPresentException : Exception
    {
        public TitleAlreadyPresentException(string title)
            : base($"Title already present in the vault: {title}")
        { }

        public TitleAlreadyPresentException(string title, Exception innerException)
            : base($"Title already present in the vault: {title}", innerException)
        { }
    }
}