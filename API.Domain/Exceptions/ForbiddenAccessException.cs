using System;

namespace API.Domain.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base("You do not have permission to perform this action.") { }

        public ForbiddenAccessException(string message) : base(message) { }
    }
}
