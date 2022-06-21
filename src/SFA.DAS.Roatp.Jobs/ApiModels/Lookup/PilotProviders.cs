using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public static class PilotProviders
    {
        public static List<PilotProvider> Pilots =>
            new List<PilotProvider>
            {
                new PilotProvider(10063272, "APPRENTIFY LIMITED"),
                new PilotProvider(10065784, "BLUE LION TRAINING ACADEMY LIMITED"),
                new PilotProvider(10001259, "CENTRAL TRAINING ACADEMY LIMITED"),
                new PilotProvider(10033758, "IXION HOLDINGS (CONTRACTS) LIMITED"),
                new PilotProvider(10083150, "PURPLE BEARD LTD"),
                new PilotProvider(10005897, "SKILLS TRAINING UK LIMITED"),
                new PilotProvider(10062041, "SKILLS4STEM LTD."),
                new PilotProvider(10047111, "THE APPRENTICESHIP COLLEGE"),
                new PilotProvider(10006770, "THE OLDHAM COLLEGE"),
                new PilotProvider(10064513, "THE TRAINING INITIATIVE GROUP LTD"),
                new PilotProvider(10007424, "THE WEST MIDLANDS CREATIVE ALLIANCE LIMITED"),
                new PilotProvider(10000948, "London South East College"),
                new PilotProvider(10000560, "Basingstoke College of Technology")
            };
    }

    public class PilotProvider
        {
            public PilotProvider(int ukprn, string name)
            {
                Ukprn = ukprn;
                Name = name;
            }
            public int Ukprn { get; set; }
            public string Name { get; set; }
        }
    
}