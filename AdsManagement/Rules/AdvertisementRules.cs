using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Rules
{
    public static class AdvertisementRules
    {
        public static int ExpirationDays { get; set; } = 60;
        public static int TitleMaxLength { get; set; } = 120;
        public static int TextMaxLength { get; set; } = 3000;
    }
}
