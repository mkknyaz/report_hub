FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Кэшируем restore, копируя только проекты и props
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Data/Exadel.ReportHub.Data.csproj", "Exadel.ReportHub.Data/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Handlers/Exadel.ReportHub.Handlers.csproj", "Exadel.ReportHub.Handlers/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Host/Exadel.ReportHub.Host.csproj", "Exadel.ReportHub.Host/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.RA/Exadel.ReportHub.RA.csproj", "Exadel.ReportHub.RA/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.SDK/Exadel.ReportHub.SDK.csproj", "Exadel.ReportHub.SDK/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Common/Exadel.ReportHub.Common.csproj", "Exadel.ReportHub.Common/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Identity/Exadel.ReportHub.Identity.csproj", "Exadel.ReportHub.Identity/"]
COPY ["Projects/Exadel.ReportHub/Directory.Build.props", "./"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.ruleset", "./"]

RUN dotnet restore "Exadel.ReportHub.Blazor.UI/Exadel.ReportHub.Blazor.UI.csproj"

COPY Projects/Exadel.ReportHub/ .
WORKDIR "/src/Exadel.ReportHub.Blazor.UI"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      libgdiplus \
      software-properties-common \
      ttf-mscorefonts-installer \
 && echo "deb http://deb.debian.org/debian bookworm contrib non-free" > /etc/apt/sources.list.d/contrib.list \
 && apt-get upgrade -y \
 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=5000
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 80

ENTRYPOINT ["dotnet", "Exadel.ReportHub.Blazor.UI.dll"]
