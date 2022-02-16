CREATE TABLE [dbo].[ProviderCourseLocation]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ProviderCourseId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderLocationId] UNIQUEIDENTIFIER NOT NULL,
    [HasDayReleaseDeliveryOption] BIT NOT NULL,
    [HasBlockReleaseDeliveryOption] BIT NOT NULL,
    [OffersPortableFlexiJob] BIT NOT NULL,
    CONSTRAINT PK_ProviderCourseLocation PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourseLocation UNIQUE (ProviderCourseId, ProviderLocationId),
    CONSTRAINT FK_ProviderCourse_ProviderCourseLocation FOREIGN KEY (ProviderCourseId) REFERENCES ProviderCourse (Id),
    CONSTRAINT FK_ProviderLocation_ProviderCourseLocation FOREIGN KEY (ProviderLocationId) REFERENCES ProviderLocation (Id)
)
