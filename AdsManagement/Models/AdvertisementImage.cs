using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public class AdvertisementImage : BaseEntity
    {
        public virtual Advertisement Advertisement { get; private set; }
        public Guid AdvertisementId { get; private set; }
        public string OriginalImagePath { get; private set; }
        public string SmallImagePath { get; private set; }

        public AdvertisementImage(string originalPath, string smallPath, Guid? id = null) : base(id)
        {
            OriginalImagePath = originalPath;
            SmallImagePath = smallPath;
        }
    }
}
