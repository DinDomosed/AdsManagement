using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions
{
    public class CommentAlreadyExistsException : Exception
    {
        public CommentAlreadyExistsException() : base() { }
        public CommentAlreadyExistsException(string? message) : base(message) { }
    }
}
