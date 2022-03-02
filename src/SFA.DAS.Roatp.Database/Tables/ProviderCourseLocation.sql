CREATE TABLE [dbo].[ProviderCourseLocation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ExternalId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderCourseId] INT NOT NULL,
    [ProviderLocationId] INT NOT NULL,
    [HasDayReleaseDeliveryOption] BIT NULL,
    [HasBlockReleaseDeliveryOption] BIT NULL,
    [HasNationalDeliveryOption] BIT NULL,
    [OffersPortableFlexiJob] BIT NULL,
    CONSTRAINT PK_ProviderCourseLocation PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourseLocation_Id UNIQUE ([ExternalId]),
    CONSTRAINT UK_ProviderCourseLocation_CourseLocation UNIQUE (ProviderCourseId, ProviderLocationId),
    CONSTRAINT FK_ProviderCourse_ProviderCourseLocation FOREIGN KEY (ProviderCourseId) REFERENCES ProviderCourse (Id),
    CONSTRAINT FK_ProviderLocation_ProviderCourseLocation FOREIGN KEY (ProviderLocationId) REFERENCES ProviderLocation (Id)
)
