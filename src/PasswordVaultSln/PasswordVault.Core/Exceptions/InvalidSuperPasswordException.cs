using System;

namespace PasswordVault.Core.Exceptions
{
    public class InvalidSuperPasswordException : Exception
    {
        public InvalidSuperPasswordException()
            : base("Invalid super-password for the specific title!")
        { }

        public InvalidSuperPasswordException(Exception innerException)
            : base("Invalid super-password for the specific title!", innerException)
        { }
    }
}