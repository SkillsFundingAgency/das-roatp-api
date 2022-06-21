using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{

    public static class PilotProviderCourses
    {
        public static List<PilotProviderCourse> PilotCourses =>
            new List<PilotProviderCourse>
            {
                new PilotProviderCourse(287, "https://www.instituteforapprenticeships.org/apprenticeship-standards/bricklayer-v1-1"),
                new PilotProviderCourse( 502, "https://www.instituteforapprenticeships.org/apprenticeship-standards/construction-site-supervisor-v1-0"),
                new PilotProviderCourse( 358, "https://www.instituteforapprenticeships.org/apprenticeship-standards/industrial-coatings-applicator-v1-0"),
                new PilotProviderCourse( 524, "https://www.instituteforapprenticeships.org/apprenticeship-standards/smart-home-technician-v1-0"),
                new PilotProviderCourse( 637, "https://www.instituteforapprenticeships.org/apprenticeship-standards/stonemason-v1-0"),
                new PilotProviderCourse( 429, "https://www.instituteforapprenticeships.org/apprenticeship-standards/archaeological-technician-v1-0"),
                new PilotProviderCourse( 547, "https://www.instituteforapprenticeships.org/apprenticeship-standards/broadcast-and-media-systems-technician-v1-0"),
                new PilotProviderCourse( 573, "https://www.instituteforapprenticeships.org/apprenticeship-standards/camera-prep-technician-v1-0"),
                new PilotProviderCourse( 229, "https://www.instituteforapprenticeships.org/apprenticeship-standards/creative-venue-technician-v1-0"),
                new PilotProviderCourse( 418, "https://www.instituteforapprenticeships.org/apprenticeship-standards/cultural-heritage-conservation-technician-v1-0"),
                new PilotProviderCourse( 407, "https://www.instituteforapprenticeships.org/apprenticeship-standards/cultural-learning-and-participation-officer-v1-0"),
                new PilotProviderCourse( 174, "https://www.instituteforapprenticeships.org/apprenticeship-standards/junior-content-producer-v1-0"),
                new PilotProviderCourse( 597, "https://www.instituteforapprenticeships.org/apprenticeship-standards/junior-vfx-artist-generalist-v1-0"),
                new PilotProviderCourse( 383, "https://www.instituteforapprenticeships.org/apprenticeship-standards/live-event-technician-v1-1"),
                new PilotProviderCourse( 443, "https://www.instituteforapprenticeships.org/apprenticeship-standards/museums-and-galleries-technician-v1-0"),
                new PilotProviderCourse( 438, "https://www.instituteforapprenticeships.org/apprenticeship-standards/photographic-assistant-v1-0"),
                new PilotProviderCourse( 516, "https://www.instituteforapprenticeships.org/apprenticeship-standards/props-technician-v1-0"),
                new PilotProviderCourse( 648, "https://www.instituteforapprenticeships.org/apprenticeship-standards/vfx-artist-or-technical-director-v1-0"),
                new PilotProviderCourse( 548, "https://www.instituteforapprenticeships.org/apprenticeship-standards/devops-engineer-v1-0"),
                new PilotProviderCourse( 650, "https://www.instituteforapprenticeships.org/apprenticeship-standards/game-programmer-v1-0"),
                new PilotProviderCourse(   2, "https://www.instituteforapprenticeships.org/apprenticeship-standards/software-developer-v1-1"),
                new PilotProviderCourse( 154, "https://www.instituteforapprenticeships.org/apprenticeship-standards/software-development-technician-v1-0"),
                new PilotProviderCourse( 188, "https://www.instituteforapprenticeships.org/apprenticeship-standards/rail-infrastructure-operator-v1-0")
            };
    }

}

public class PilotProviderCourse
    {
        public PilotProviderCourse(int larsCode, string standardInfoUrl)
        {
            LarsCode = larsCode;
            StandardInfoUrl = standardInfoUrl;
        }
        public int LarsCode { get; set; }
        public string StandardInfoUrl { get; set; }
    }

