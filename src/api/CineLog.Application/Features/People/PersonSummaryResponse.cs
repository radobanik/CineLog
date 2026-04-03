namespace CineLog.Application.Features.People;

public record PersonSummaryResponse(Guid Id, string Name, string? ProfilePath);
