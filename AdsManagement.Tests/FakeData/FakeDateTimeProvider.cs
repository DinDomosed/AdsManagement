using AdsManagement.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Tests.FakeData
{
    internal class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; set; }
    }
}
