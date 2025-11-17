FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MarketingCampaignTracker.sln ./

COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/Shared/Shared.Abstractions/Shared.Abstractions.csproj src/Shared/Shared.Abstractions/

COPY src/CampaignSvc/Campaign.Domain/Campaign.Domain.csproj src/CampaignSvc/Campaign.Domain/
COPY src/CampaignSvc/Campaign.Application/Campaign.Application.csproj src/CampaignSvc/Campaign.Application/
COPY src/CampaignSvc/Campaign.Infrastructure/Campaign.Infrastructure.csproj src/CampaignSvc/Campaign.Infrastructure/
COPY src/CampaignSvc/Campaign.Api/Campaign.Api.csproj src/CampaignSvc/Campaign.Api/

COPY src/AnalyticsSvc/Analytics.Domain/Analytics.Domain.csproj src/AnalyticsSvc/Analytics.Domain/
COPY src/AnalyticsSvc/Analytics.Application/Analytics.Application.csproj src/AnalyticsSvc/Analytics.Application/
COPY src/AnalyticsSvc/Analytics.Infrastructure/Analytics.Infrastructure.csproj src/AnalyticsSvc/Analytics.Infrastructure/
COPY src/AnalyticsSvc/Analytics.Api/Analytics.Api.csproj src/AnalyticsSvc/Analytics.Api/

COPY src/Core.Api/Core.Api.csproj src/Core.Api/

RUN dotnet restore MarketingCampaignTracker.sln

COPY . .

RUN dotnet publish src/Core.Api/Core.Api.csproj -c Release -o /app/publish --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Core.Api.dll"]
