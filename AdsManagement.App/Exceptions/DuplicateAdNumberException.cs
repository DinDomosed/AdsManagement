using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions
{
    public class DuplicateAdNumberException : Exception
    {
        public int Number { get; }
        public DuplicateAdNumberException(int number) : base()
        {
            Number = number;
        }
    }
}
