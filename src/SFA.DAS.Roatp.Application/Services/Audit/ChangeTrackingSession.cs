using Newtonsoft.Json;
using SFA.DAS.Roatp.Abstractions.Audit;
using SFA.DAS.Roatp.Abstractions.Events;
using SFA.DAS.Roatp.Domain.Events;
using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Data;

namespace SFA.DAS.Roatp.Services.Audit
{
    public class ChangeTrackingSession : IChangeTrackingSession
    {
        private readonly IStateService _stateService;
        private readonly List<TrackedItem> _trackedItems;
        private readonly Guid _correlationId;
        private readonly UserAction _userAction;
        private readonly RoatpDataContext _RoatpDataContext;


        public ChangeTrackingSession(IStateService stateService, UserAction userAction, RoatpDataContext RoatpDataContext)
        {
            _stateService = stateService;
            _userAction = userAction;
            _RoatpDataContext = RoatpDataContext;
            _correlationId = Guid.NewGuid();
            _trackedItems = new List<TrackedItem>();
        }

        public IReadOnlyList<TrackedItem> TrackedItems => _trackedItems.AsReadOnly();

        public void TrackInsert(ITrackableEntity trackedObject)
        {
            _trackedItems.Add(TrackedItem.CreateInsertTrackedItem(trackedObject));
        }

        public void TrackUpdate(ITrackableEntity trackedObject)
        {
            var initialState = _stateService.GetState(trackedObject);
            _trackedItems.Add(TrackedItem.CreateUpdateTrackedItem(trackedObject, initialState));
        }

        public void TrackDelete(ITrackableEntity trackedObject)
        {
            var initialState = _stateService.GetState(trackedObject);
            _trackedItems.Add(TrackedItem.CreateDeleteTrackedItem(trackedObject, initialState));
        }

        public IEnumerable<IDomainEvent> FlushEvents()
        {
            var result = new List<IDomainEvent>();

            foreach (var item in _trackedItems)
            {
                var updated = item.Operation == ChangeTrackingOperation.Delete ? null : _stateService.GetState(item.TrackedEntity);

                result.Add(new EntityStateChanged
                {
                    CorrelationId = _correlationId,
                    UserAction = _userAction,
                    EntityType = item.TrackedEntity.GetType().Name,
                    EntityId = item.TrackedEntity.GetTrackedEntityId(),
                    InitialState = item.InitialState == null ? null : JsonConvert.SerializeObject(item.InitialState),
                    UpdatedState = updated == null ? null : JsonConvert.SerializeObject(updated),
                    UpdatedOn = DateTime.UtcNow,
                });
            }

            _trackedItems.Clear();

            return result;
        }
    }

    public enum ChangeTrackingOperation
    {
        Insert, Delete, Update
    }
}
