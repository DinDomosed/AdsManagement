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
        protected AdvertisementImage() : base() { }

        public override bool Equals(object? obj)
        {
            if (obj is not AdvertisementImage image)
                return false;

            return Id == image.Id; 
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
