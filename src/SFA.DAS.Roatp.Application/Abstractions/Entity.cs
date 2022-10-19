using System;
using SFA.DAS.Roatp.Abstractions.Audit;

namespace SFA.DAS.Roatp.Abstractions
{
    public class Entity<T> : ITrackableEntity
    {
        public T Id { get; protected set; }

        public long GetTrackedEntityId()
        {
            return Convert.ToInt64(Id);
        }
    }
}
