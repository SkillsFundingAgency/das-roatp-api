CREATE VIEW [dbo].[RestrictedCourseView]
AS
    SELECT [LarsCode]
    FROM [dbo].[Standard] st
    WHERE [CourseType] = 'ShortCourse'
    UNION
    SELECT [LarsCode]  
    FROM [dbo].[RestrictedCourse]
;
