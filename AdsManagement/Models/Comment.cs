using AdsManagement.Domain.Rules;
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
                if (estimation > RatingRules.Max || estimation < RatingRules.Min)
                    throw new ArgumentOutOfRangeException(nameof(estimation), $"Estimation must be between {RatingRules.Min} and {RatingRules.Max}");

                AdvertisementId = advertisementId;
                UserId = userId;
                Text = text?.Trim() ?? throw new ArgumentNullException(nameof(text));
                Estimation = estimation;
                CreatedAt = DateTime.UtcNow;
            }
            internal Comment(Guid advertisementId, Guid userId, string text, int estimation, DateTime createAt) : this(advertisementId, userId, text, estimation)
            {
                CreatedAt = createAt;
            }
            public void UpdateText(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    throw new ArgumentNullException(nameof(text), "The text cannot be empty");

                if (text.Length > CommentRules.TextMaxLength)
                    throw new ArgumentOutOfRangeException(nameof(text), "The text is too long");

                Text = text.Trim();
            }
            public void UpdateEstimation(int estimation)
            {
                if (estimation > RatingRules.Max || estimation < RatingRules.Min)
                    throw new ArgumentOutOfRangeException(nameof(estimation), $"Estimation must be between {RatingRules.Min} and {RatingRules.Max}");

                Estimation = estimation;
            }
            public override bool Equals(object? obj)
            {
                if (obj is not Comment comment)
                    return false;

                return Id == comment.Id;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(Id);
            }
        }
}
