using Newtonsoft.Json;
using SFA.DAS.Roatp.Abstractions.Audit;
using SFA.DAS.Roatp.Abstractions.Events;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Domain.Events;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.EventHandlers
{
    public class EntityStateChangedHandler : IDomainEventHandler<EntityStateChanged>
    {
        private readonly IDiffService _diffService;
        private readonly RoatpDataContext _dbContext;

        public EntityStateChangedHandler(RoatpDataContext dbContext, IDiffService diffService)
        {
            _dbContext = dbContext;
            _diffService = diffService;
        }

        public async Task Handle(EntityStateChanged @event, CancellationToken cancellationToken = default)
        {
            var initialState = @event.InitialState == null
                ? null
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(@event.InitialState);

            var updatedState = @event.UpdatedState == null
                ? null
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(@event.UpdatedState);

            var diff = _diffService.GenerateDiff(initialState, updatedState);

            if (diff.Count == 0) return;

            // Add audit table
        }
    }
}