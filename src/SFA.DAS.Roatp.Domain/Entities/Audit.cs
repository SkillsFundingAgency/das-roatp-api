using System;
using System.Diagnostics;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class Audit
    {
        public long Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string UserAction { get; set; }
        public DateTime AuditDate { get; set; }
        public string InitialState { get; set; }
        public string UpdatedState { get; set; }

        public Audit(string entityType, string entityId, string userId, string userDisplayName, string userAction, string initialState, string updatedState)       
        {
            CorrelationId = Guid.Parse(Activity.Current.RootId);
            EntityType = entityType;
            EntityId = entityId;
            UserId = userId;
            UserDisplayName = userDisplayName;
            UserAction = userAction;
            AuditDate = DateTime.Now;
            InitialState = initialState;
            UpdatedState = updatedState;
        }
    }
}
