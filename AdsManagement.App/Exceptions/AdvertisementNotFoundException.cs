using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions
{
    public class AdvertisementNotFoundException : Exception
    {
        public AdvertisementNotFoundException() : base () { }
        public AdvertisementNotFoundException(string? message) : base (message) { }
        public AdvertisementNotFoundException(string? message, Exception? innerException) : base (message, innerException) { }


    }
}
