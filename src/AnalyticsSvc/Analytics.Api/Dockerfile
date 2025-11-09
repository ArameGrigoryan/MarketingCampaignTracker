FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MarketingCampaignTracker.sln ./
COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/AnalyticsSvc/Analytics.Domain/Analytics.Domain.csproj src/AnalyticsSvc/Analytics.Domain/
COPY src/AnalyticsSvc/Analytics.Application/Analytics.Application.csproj src/AnalyticsSvc/Analytics.Application/
COPY src/AnalyticsSvc/Analytics.Infrastructure/Analytics.Infrastructure.csproj src/AnalyticsSvc/Analytics.Infrastructure/
COPY src/AnalyticsSvc/Analytics.Api/Analytics.Api.csproj src/AnalyticsSvc/Analytics.Api/
RUN dotnet restore src/AnalyticsSvc/Analytics.Api/Analytics.Api.csproj

COPY . .
RUN dotnet publish src/AnalyticsSvc/Analytics.Api/Analytics.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Analytics.Api.dll"]
