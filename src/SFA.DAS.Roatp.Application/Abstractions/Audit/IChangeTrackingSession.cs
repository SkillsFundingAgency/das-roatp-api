using System.Collections.Generic;
using SFA.DAS.Roatp.Abstractions.Events;

namespace SFA.DAS.Roatp.Abstractions.Audit
{
    public interface IChangeTrackingSession
    {
        void TrackInsert(ITrackableEntity trackedObject);
        void TrackUpdate(ITrackableEntity trackedObject);
        void TrackDelete(ITrackableEntity trackedObject);
        IEnumerable<IDomainEvent> FlushEvents();
    }
}
