using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public class Advertisement : BaseEntity
    {
        public virtual User User { get; private set; }

        public Guid UserId { get; private set; }
        public int Number { get; private set; }
        public string Text { get; private set; }
        public float Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public virtual ICollection<AdvertisementImage> Images { get; private set; } = new List<AdvertisementImage>();

        public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();

        public Advertisement(Guid userId, int number, string text, Guid? id = null) : base(id)
        {
            UserId = userId;
            Number = number;
            Text = text;
        }
        internal Advertisement(Guid userId, int number, string text, DateTime createdAt, DateTime expiresAt) : this(userId, number, text)
        {
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }
    }
}
