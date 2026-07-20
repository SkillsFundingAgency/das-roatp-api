using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Api.Models;

public class UkrpnAndLarsCodeModel : IUkprnAndLarsCodeValidator
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
}
