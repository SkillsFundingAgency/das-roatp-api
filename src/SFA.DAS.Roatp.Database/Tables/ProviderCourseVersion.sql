CREATE TABLE [dbo].[ProviderCourseVersion]
(
    [Id] INT NOT NULL,
    [ProviderCourseId] INT NOT NULL,
    [StandardUId] VARCHAR(20) NOT NULL,
    [Version] VARCHAR(20) NOT NULL,
    CONSTRAINT PK_ProviderCourseVersion PRIMARY KEY (Id),
    CONSTRAINT FK_ProviderCourse_ProviderCourseVersion FOREIGN KEY (ProviderCourseId) REFERENCES [ProviderCourse] (Id),
    CONSTRAINT UK_ProviderCourseVersion UNIQUE ([ProviderCourseId], [StandardUId])
)
