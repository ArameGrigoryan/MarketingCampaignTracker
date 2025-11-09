FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# -------- skeleton copy (ԲՈԼՈՐ csproj-երը) --------
COPY MarketingCampaignTracker.sln ./

# Shared
COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/Shared/Shared.Abstractions/Shared.Abstractions.csproj src/Shared/Shared.Abstractions/

# Core.Api
COPY src/Core.Api/Core.Api.csproj src/Core.Api/

# CampaignSvc
COPY src/CampaignSvc/Campaign.Domain/Campaign.Domain.csproj src/CampaignSvc/Campaign.Domain/
COPY src/CampaignSvc/Campaign.Application/Campaign.Application.csproj src/CampaignSvc/Campaign.Application/
COPY src/CampaignSvc/Campaign.Infrastructure/Campaign.Infrastructure.csproj src/CampaignSvc/Campaign.Infrastructure/
COPY src/CampaignSvc/Campaign.Api/Campaign.Api.csproj src/CampaignSvc/Campaign.Api/

# AnalyticsSvc
COPY src/AnalyticsSvc/Analytics.Domain/Analytics.Domain.csproj src/AnalyticsSvc/Analytics.Domain/
COPY src/AnalyticsSvc/Analytics.Application/Analytics.Application.csproj src/AnalyticsSvc/Analytics.Application/
COPY src/AnalyticsSvc/Analytics.Infrastructure/Analytics.Infrastructure.csproj src/AnalyticsSvc/Analytics.Infrastructure/
COPY src/AnalyticsSvc/Analytics.Api/Analytics.Api.csproj src/AnalyticsSvc/Analytics.Api/

# -------- restore ամբողջ սոլյուշնի համար --------
RUN dotnet restore MarketingCampaignTracker.sln

# -------- հետո արդեն ամբողջ աղբյուրը --------
COPY . .
RUN dotnet publish src/CampaignSvc/Campaign.Api/Campaign.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Campaign.Api.dll"]
