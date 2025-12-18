using AdsManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions.NotFound
{
    public class AdImageNotFoundException : EntityNotFoundException
    {
        public AdImageNotFoundException (Guid id) : base(nameof(AdvertisementImage), id) { }
    }
}
 