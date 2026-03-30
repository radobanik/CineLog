FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY src/CineLog.Domain/CineLog.Domain.csproj             src/CineLog.Domain/
COPY src/CineLog.Application/CineLog.Application.csproj   src/CineLog.Application/
COPY src/CineLog.Contracts/CineLog.Contracts.csproj       src/CineLog.Contracts/
COPY src/CineLog.Infrastructure/CineLog.Infrastructure.csproj src/CineLog.Infrastructure/
COPY tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj       tools/CineLog.TmdbSync/

RUN dotnet restore tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj

COPY src/CineLog.Domain/       src/CineLog.Domain/
COPY src/CineLog.Application/  src/CineLog.Application/
COPY src/CineLog.Contracts/    src/CineLog.Contracts/
COPY src/CineLog.Infrastructure/ src/CineLog.Infrastructure/
COPY tools/CineLog.TmdbSync/   tools/CineLog.TmdbSync/

RUN dotnet publish tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "CineLog.TmdbSync.dll"]
