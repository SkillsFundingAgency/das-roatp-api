CREATE TABLE [dbo].[ProviderCourseVersion]
(
    [Id] INT IDENTITY (1,1) NOT NULL,
    [ProviderCourseId] INT NOT NULL,
    [StandardUId] VARCHAR(20) NOT NULL,
    [Version] VARCHAR(20) NOT NULL,
    CONSTRAINT PK_ProviderCourseVersion PRIMARY KEY (Id),
    CONSTRAINT FK_ProviderCourseVersion_ProviderCourse FOREIGN KEY (ProviderCourseId) REFERENCES [ProviderCourse] (Id),
    CONSTRAINT UK_ProviderCourseVersion_Course_StandardUId UNIQUE ([ProviderCourseId], [StandardUId])
)
