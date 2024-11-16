using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typoetry.Models.Exceptions
{
    public class EntryDataNotSetException : Exception
    {
        public EntryDataNotSetException()
            : base("The Entry data has not been set.")
        {
        }

        public EntryDataNotSetException(string message)
            : base(message)
        {
        }

        public EntryDataNotSetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
