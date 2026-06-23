namespace SFA.DAS.Roatp.Domain.Entities;

public class RestrictedCourseView
{
    public string LarsCode { get; set; }

    public virtual Standard Standard { get; set; }
}