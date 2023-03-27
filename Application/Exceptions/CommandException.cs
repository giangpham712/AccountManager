using System;

namespace AccountManager.Application.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException() : base(null)
        {
        }

        public CommandException(string message) : base(message)
        {
        }

        public CommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}