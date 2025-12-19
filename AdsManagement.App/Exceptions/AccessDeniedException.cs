using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public Guid TargetUserId { get; }
        public Guid RequesterUserId { get; }
        public AccessDeniedException(Guid targetUserId, Guid requesterUserId) : base($"User does not have permission to perform this action")
        {
            TargetUserId = targetUserId;
            RequesterUserId = requesterUserId;
        }
    }
}
