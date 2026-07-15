using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Api.Models;

public class LarsCodeUkrpnModel : ILarsCodeUkprn
{
    public string LarsCode { get; set; }

    public int Ukprn { get; set; }
}
