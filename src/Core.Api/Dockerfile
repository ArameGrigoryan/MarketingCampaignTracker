FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MarketingCampaignTracker.sln ./
COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/Core.Api/Core.Api.csproj src/Core.Api/
RUN dotnet restore src/Core.Api/Core.Api.csproj

COPY . .
RUN dotnet publish src/Core.Api/Core.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Core.Api.dll"]
