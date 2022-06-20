CREATE TABLE [dbo].[ProviderCourseLocation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [NavigationId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderCourseId] INT NOT NULL,
    [ProviderLocationId] INT  NULL,
    [HasDayReleaseDeliveryOption] BIT NULL,
    [HasBlockReleaseDeliveryOption] BIT NULL,
    [IsImported] BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ProviderCourseLocation PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourseLocation_NavigationId UNIQUE ([NavigationId]),
    CONSTRAINT UK_ProviderCourseLocation_Course_Location UNIQUE (ProviderCourseId, [ProviderLocationId]),
    CONSTRAINT FK_ProviderCourseLocation_ProviderCourse FOREIGN KEY (ProviderCourseId) REFERENCES ProviderCourse (Id),
    CONSTRAINT FK_ProviderCourseLocation_ProviderLocation FOREIGN KEY ([ProviderLocationId]) REFERENCES ProviderLocation (Id)
)
