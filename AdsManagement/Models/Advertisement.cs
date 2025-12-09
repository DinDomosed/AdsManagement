
using AdsManagement.Domain.Rules;
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
        public decimal Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public virtual User User { get; private set; }
        public virtual ICollection<AdvertisementImage> Images { get; private set; } = new List<AdvertisementImage>();
        public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();


        public Advertisement(Guid userId, string title, int number, string text, Guid? id = null) : base(id)
        {
            UserId = userId;
            Title = title?.Trim() ?? throw new ArgumentNullException(nameof(title));
            Number = number;
            Text = text?.Trim() ?? throw new ArgumentNullException(nameof(text));
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddDays(AdvertisementRules.ExpirationDays);
        }
        internal Advertisement(Guid userId, string title , int number, string text, DateTime createdAt, DateTime expiresAt) : this(userId, title, number, text)
        {
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }

        protected Advertisement() : base() { }

        public void UpdateRating(decimal rating)
        {
            if (rating > RatingRules.Max || rating < RatingRules.Min)
                throw new ArgumentOutOfRangeException(nameof(rating), $"Rating must be between {RatingRules.Min} and {RatingRules.Max}");

            Rating = Math.Round(rating, 1, MidpointRounding.AwayFromZero);
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title), "The title cannot be empty");
            if (title.Length > AdvertisementRules.TitleMaxLength)
                throw new ArgumentOutOfRangeException(nameof(title), "The title is too long");

            Title = title.Trim();
        }
        public void UpdateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text), "The text cannot be empty");

            if (text.Length > AdvertisementRules.TextMaxLength)
                throw new ArgumentOutOfRangeException(nameof(text), "The text is too long");

            Text = text.Trim();
        }
        public void ExtendExpiration(int days)
        {
            if (days <= 0)
                throw new ArgumentException(nameof(days), "Days must be positive");

            ExpiresAt = ExpiresAt.AddDays(days);
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
