using AdsManagement.App.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Tests.FakeData
{
    public class FileDataFake : IFileData
    {
        public string ContentType { get; }
        public long Length { get; }
        public Stream OpenReadStream() => _stream;
        private readonly Stream _stream;

        public FileDataFake(Stream stream, string contentType)
        {
            _stream = stream ?? throw new ArgumentNullException("the stream is null");
            ContentType = contentType;
            Length = _stream.Length;
        }

    }
}
