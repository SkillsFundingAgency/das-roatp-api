using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.UnitTests.Common
{
    public class UkprnValidatorTestObject : ILarsCodeUkprn
    {
        public int Ukprn { get; }

        public int LarsCode { get; }
        public UkprnValidatorTestObject(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
