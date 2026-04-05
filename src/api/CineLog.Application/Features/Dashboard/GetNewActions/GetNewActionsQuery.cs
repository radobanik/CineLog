using MediatR;

namespace CineLog.Application.Features.Dashboard.GetNewActions;

public record GetNewActionsQuery(int Count = 20) : IRequest<List<NewActionResponse>>;
