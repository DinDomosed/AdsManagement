using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions
{
    public class ExceedingTheAdLimitException : Exception
    {
        public ExceedingTheAdLimitException() : base() { }
        public ExceedingTheAdLimitException(string? message) : base(message) { }
        public ExceedingTheAdLimitException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
