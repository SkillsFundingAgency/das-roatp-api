﻿IF 312 > (SELECT COUNT(*) FROM ProviderCourse)
BEGIN

    DELETE FROM [ProviderCourse]

    DELETE FROM [Provider]

    DECLARE @providerRowCount INT

    INSERT INTO [Provider] 
        (Ukprn, LegalName)
    VALUES
        (10063272,'APPRENTIFY LIMITED'),
        (10065784,'BLUE LION TRAINING ACADEMY LIMITED'),
        (10001259,'CENTRAL TRAINING ACADEMY LIMITED'),
        (10033758,'IXION HOLDINGS (CONTRACTS) LIMITED'),
        (10083150,'PURPLE BEARD LTD'),
        (10005897,'SKILLS TRAINING UK LIMITED'),
        (10062041,'SKILLS4STEM LTD.'),
        (10047111,'THE APPRENTICESHIP COLLEGE'),
        (10006770,'THE OLDHAM COLLEGE'),
        (10064513,'THE TRAINING INITIATIVE GROUP LTD'),
        (10007424,'THE WEST MIDLANDS CREATIVE ALLIANCE LIMITED'),
        (10000948,'London South East College'),
        (10000560,'Basingstoke College of Technology')

    SELECT @providerRowCount = @@ROWCOUNT

    DECLARE @currentRowCount INT = 0

    WHILE @currentRowCount < @providerRowCount
    BEGIN

        DECLARE @providerId INT

        SELECT @providerId = Id  
        FROM Provider 
        ORDER BY Id
        OFFSET @currentRowCount ROW
        FETCH NEXT 1 ROW ONLY

        INSERT INTO ProviderCourse 
            (ProviderId, LarsCode, IfateReferenceNumber, StandardInfoUrl)
        VALUES
            (@providerId, 240, 'ST0263', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/advanced-carpentry-and-joinery-v1-1'),
            (@providerId, 287, 'ST0095', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/bricklayer-v1-1'),
            (@providerId, 502, 'ST0048', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/construction-site-supervisor-v1-0'),
            (@providerId, 358, 'ST0463', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/industrial-coatings-applicator-v1-0'),
            (@providerId, 524, 'ST0464', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/smart-home-technician-v1-0'),
            (@providerId, 637, 'ST0442', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/stonemason-v1-0'),
            (@providerId, 429, 'ST0221', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/archaeological-technician-v1-0'),
            (@providerId, 547, 'ST0425', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/broadcast-and-media-systems-technician-v1-0'),
            (@providerId, 573, 'ST0900', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/camera-prep-technician-v1-0'),
            (@providerId, 229, 'ST0106', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/creative-venue-technician-v1-0'),
            (@providerId, 418, 'ST0611', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/cultural-heritage-conservation-technician-v1-0'),
            (@providerId, 407, 'ST0396', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/cultural-learning-and-participation-officer-v1-0'),
            (@providerId, 174, 'ST0105', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/junior-content-producer-v1-0'),
            (@providerId, 597, 'ST0903', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/junior-vfx-artist-generalist-v1-0'),
            (@providerId, 383, 'ST0255', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/live-event-technician-v1-1'),
            (@providerId, 443, 'ST0559', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/museums-and-galleries-technician-v1-0'),
            (@providerId, 438, 'ST0506', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/photographic-assistant-v1-0'),
            (@providerId, 516, 'ST0174', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/props-technician-v1-0'),
            (@providerId, 648, 'ST0902', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/vfx-artist-or-technical-director-v1-0'),
            (@providerId, 548, 'ST0825', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/devops-engineer-v1-0'),
            (@providerId, 650, 'ST0953', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/game-programmer-v1-0'),
            (@providerId,   2, 'ST0116', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/software-developer-v1-1'),
            (@providerId, 154, 'ST0128', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/software-development-technician-v1-0'),
            (@providerId, 188, 'ST0419', 'https://www.instituteforapprenticeships.org/apprenticeship-standards/rail-infrastructure-operator-v1-0')

        SET @currentRowCount = @currentRowCount + 1

    END

END
