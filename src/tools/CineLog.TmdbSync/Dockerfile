FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY src/api/CineLog.Domain/CineLog.Domain.csproj             src/api/CineLog.Domain/
COPY src/api/CineLog.Application/CineLog.Application.csproj   src/api/CineLog.Application/
COPY src/api/CineLog.Contracts/CineLog.Contracts.csproj       src/api/CineLog.Contracts/
COPY src/api/CineLog.Infrastructure/CineLog.Infrastructure.csproj src/api/CineLog.Infrastructure/
COPY src/tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj       src/tools/CineLog.TmdbSync/

RUN dotnet restore src/tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj

COPY src/api/CineLog.Domain/       src/api/CineLog.Domain/
COPY src/api/CineLog.Application/  src/api/CineLog.Application/
COPY src/api/CineLog.Contracts/    src/api/CineLog.Contracts/
COPY src/api/CineLog.Infrastructure/ src/api/CineLog.Infrastructure/
COPY src/tools/CineLog.TmdbSync/   src/tools/CineLog.TmdbSync/

RUN dotnet publish src/tools/CineLog.TmdbSync/CineLog.TmdbSync.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "CineLog.TmdbSync.dll"]
