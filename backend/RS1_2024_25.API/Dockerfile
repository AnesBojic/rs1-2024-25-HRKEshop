# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RS1_2024_25.API.csproj ./
RUN dotnet restore RS1_2024_25.API.csproj

COPY . ./
RUN dotnet publish RS1_2024_25.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:7000
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7000

HEALTHCHECK --interval=10s --timeout=5s --start-period=40s --retries=10 \
  CMD curl -fsS http://localhost:7000/swagger/index.html >/dev/null || exit 1

ENTRYPOINT ["dotnet", "RS1_2024_25.API.dll"]
