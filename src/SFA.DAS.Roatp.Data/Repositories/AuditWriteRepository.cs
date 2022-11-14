using SFA.DAS.Roatp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class AuditWriteRepository<T> where T : class 
    {
        private readonly RoatpDataContext _roatpDataContext;

        public AuditWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public void AddAudit(T entityInitialState, T entityUpdatedState, string entityId, string userId, string userDisplayName, string userAction)
        {
            var audit = new Audit
            {
                CorrelationId = Guid.Parse(Activity.Current.RootId),
                EntityType = typeof(T).Name,
                UserAction = userAction,
                UserId = userId,
                UserDisplayName = userDisplayName,
                EntityId = entityId,
                
                InitialState = JsonSerializer.Serialize(entityInitialState),
                UpdatedState = JsonSerializer.Serialize(entityUpdatedState),
                AuditDate = DateTime.Now
            };
            _roatpDataContext.Audits.Add(audit);
        }

        public void AddAudit(List<T> entityInitialState, List<T> entityUpdatedState, string entityId, string userId, string userDisplayName, string userAction )
        {
            var audit = new Audit
            {
                CorrelationId = Guid.Parse(Activity.Current.RootId),
                EntityType = typeof(T).Name,
                UserAction = userAction,
                UserId = userId,
                UserDisplayName = userDisplayName, 
                EntityId = entityId,
                InitialState = JsonSerializer.Serialize(entityInitialState),
                UpdatedState = JsonSerializer.Serialize(entityUpdatedState),
                AuditDate = DateTime.Now
            };
            _roatpDataContext.Audits.Add(audit);
        }
    }
}
