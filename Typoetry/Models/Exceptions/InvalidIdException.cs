using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typoetry.Models.Exceptions
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException()
            : base("The provided ID is invalid.")
        {
        }

        public InvalidIdException(string message)
            : base(message)
        {
        }

        public InvalidIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidIdException(int invalidId)
            : base($"The ID '{invalidId}' is invalid.")
        {
        }
    }
}
