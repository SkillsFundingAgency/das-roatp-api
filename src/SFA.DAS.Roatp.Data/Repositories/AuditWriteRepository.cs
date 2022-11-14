using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
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
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            };
            var audit = new Audit(typeof(T).Name, entityId, userId, userDisplayName, userAction, JsonSerializer.Serialize(entityInitialState, jsonSerializerOptions), JsonSerializer.Serialize(entityUpdatedState, jsonSerializerOptions));
            
            _roatpDataContext.Audits.Add(audit);
        }

        public void AddAudit(List<T> entityInitialState, List<T> entityUpdatedState, string entityId, string userId, string userDisplayName, string userAction )
        {
            var audit = new Audit(typeof(T).Name, entityId, userId, userDisplayName, userAction, JsonSerializer.Serialize(entityInitialState), JsonSerializer.Serialize(entityUpdatedState));

            _roatpDataContext.Audits.Add(audit);
        }
    }
}
