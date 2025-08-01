using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;
public interface IImportFeedbackSummariesRepository
{
    Task Import(DateTime timeStarted, IEnumerable<ProviderApprenticeStars> providerApprenticeStars, IEnumerable<ProviderEmployerStars> providerEmployerStars);
}
