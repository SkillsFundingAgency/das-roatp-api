using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Abstractions.Audit;
using SFA.DAS.Roatp.Abstractions.Events;
using SFA.DAS.Roatp.Domain.Events;
using SFA.DAS.Roatp.Services.Audit;
using SFA.DAS.Roatp.Data;

namespace SFA.DAS.Roatp.Abstractions
{
    public class AggregateRoot<T> : Entity<T>
    {
        protected IChangeTrackingSession ChangeTrackingSession { get; private set; }
        private readonly List<Func<IDomainEvent>> _events = new List<Func<IDomainEvent>>();

        protected void StartTrackingSession(UserAction userAction, RoatpDataContext RoatpDataContext)
        {
            ChangeTrackingSession = new ChangeTrackingSession(new StateService(), userAction, RoatpDataContext);
        }

        protected void AddEvent(Func<IDomainEvent> @event)
        {
            lock (_events)
            {
                _events.Add(@event);
            }
        }

        protected void AddEvent(IDomainEvent @event)
        {
            lock (_events)
            {
                _events.Add(() => @event);
            }
        }

        public IEnumerable<IDomainEvent> FlushEvents()
        {
            lock (_events)
            {
                var result = new List<IDomainEvent>();
                foreach (var eventFunc in _events)
                {
                    result.Add(eventFunc.Invoke());
                }
                _events.Clear();

                if(ChangeTrackingSession != null)
                {
                    result.AddRange(ChangeTrackingSession.FlushEvents());
                }

                return result;
            }
        }
    }
}
