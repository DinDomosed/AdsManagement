using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces.Events
{
    public interface ICommentEstimationChangedHandler
    {
        Task HandleAsync(Guid advertisementAd, CancellationToken token);
    }
}
