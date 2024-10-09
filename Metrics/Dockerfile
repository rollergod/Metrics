FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Metrics/Metrics.csproj", "./"]
RUN dotnet restore "Metrics.csproj"

COPY . .
RUN dotnet build "Metrics/Metrics.csproj" -c Development -o /app/build

FROM build AS publish
RUN dotnet publish "Metrics/Metrics.csproj" -c Development -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metrics.dll",  "--environment=Development"]