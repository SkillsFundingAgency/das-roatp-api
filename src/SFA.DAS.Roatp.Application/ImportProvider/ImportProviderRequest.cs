using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.Roatp.Domain.ApiModels.Import;

namespace SFA.DAS.Roatp.Application.ImportProvider
{
    public class ImportProviderRequest:IRequest<bool>
    {
        public Provider Provider { get; set; }
    }
}
