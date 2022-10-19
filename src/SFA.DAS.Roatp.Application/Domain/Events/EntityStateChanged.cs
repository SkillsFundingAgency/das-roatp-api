﻿using System;
using SFA.DAS.Roatp.Abstractions.Events;

namespace SFA.DAS.Roatp.Domain.Events
{
    public class EntityStateChanged : IDomainEvent
    {
        public Guid CorrelationId { get; set; }
        public UserAction UserAction { get; set; }
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public string InitialState { get; set; }
        public string UpdatedState { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
