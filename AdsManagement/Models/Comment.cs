using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public class Comment : BaseEntity
    {
        public virtual Advertisement Advertisement { get; private set; }
        public virtual User User { get; private set; }

        public Guid UserId { get; private set; }
        public Guid AdvertisementId { get; private set; }
        public string Text { get; private set; }
        public int Estimation { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Comment() : base() { }
        public Comment(Guid advertisementId, Guid userId, string text, int estimation, Guid? id = null) : base(id)
        {
            AdvertisementId = advertisementId;
            UserId = userId;
            Text = text;
            Estimation = estimation;
        }
        internal Comment(Guid advertisementId, Guid userId, string text, int estimation, DateTime createAt) : this(advertisementId, userId, text, estimation)
        {
            CreatedAt = createAt;
        }
    }
}
