using System;
using MediatR;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteShortlist;

public record DeleteShortlistCommand(Guid ShortlistId) : IRequest<DeleteShortlistCommandResult>;
