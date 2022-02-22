CREATE TABLE [dbo].[ProviderCourseVersion]
(
    [ProviderCourseId] INT NOT NULL,
    [StandardUId] VARCHAR(20) NOT NULL,
    [Version] VARCHAR(20) NOT NULL,
    CONSTRAINT PK_ProviderCourseVersion PRIMARY KEY (ProviderCourseId, StandardUId),
    CONSTRAINT FK_ProviderCourse_ProviderCourseVersion FOREIGN KEY (ProviderCourseId) REFERENCES [ProviderCourse] (Id)
)
