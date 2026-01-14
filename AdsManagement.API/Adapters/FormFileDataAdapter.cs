using AdsManagement.App.Interfaces.Service;

namespace AdsManagement.API.Adapters
{
    public class FormFileDataAdapter : IFileData
    {
        private readonly IFormFile _formFile;

        public FormFileDataAdapter(IFormFile formFile)
        {
            _formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
        }
        public string ContentType => _formFile.ContentType;
        public long Length => _formFile.Length;
        public Stream OpenReadStream() => _formFile.OpenReadStream();
    }
}
