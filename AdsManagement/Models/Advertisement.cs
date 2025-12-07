using AdsManagement.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public class Advertisement : BaseEntity
    {
        public Guid UserId { get; private set; }
        public int Number { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }
        public float Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public virtual User User { get; private set; }
        public virtual ICollection<AdvertisementImage> Images { get; private set; } = new List<AdvertisementImage>();

        public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();


        public Advertisement(Guid userId, string title, int number, string text, Guid? id = null) : base(id)
        {
            UserId = userId;
            Title = title;
            Number = number;
            Text = text;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddDays(AdvertisementConfig.ExpirationDays);
        }
        internal Advertisement(Guid userId, string title , int number, string text, DateTime createdAt, DateTime expiresAt) : this(userId, title, number, text)
        {
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Advertisement adv)
                return false;

            return Id == adv.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
