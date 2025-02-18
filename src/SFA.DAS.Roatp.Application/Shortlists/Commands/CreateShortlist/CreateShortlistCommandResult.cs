using System;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandResult
{
    public Guid ShortlistId { get; set; }
    public bool IsCreated { get; set; }
}
