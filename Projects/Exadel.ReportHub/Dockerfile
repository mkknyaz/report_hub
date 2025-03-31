FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app


COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Data/Exadel.ReportHub.Data.csproj", "Exadel.ReportHub.Data/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Handlers/Exadel.ReportHub.Handlers.csproj", "Exadel.ReportHub.Handlers/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Host/Exadel.ReportHub.Host.csproj", "Exadel.ReportHub.Host/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.RA/Exadel.ReportHub.RA.csproj", "Exadel.ReportHub.RA/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.SDK/Exadel.ReportHub.SDK.csproj", "Exadel.ReportHub.SDK/"]
COPY ["Projects/Exadel.ReportHub/Directory.Build.props", "./"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.ruleset", "./"]

RUN dotnet restore "Exadel.ReportHub.Host/Exadel.ReportHub.Host.csproj"


COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Common/", "Exadel.ReportHub.Common/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Handlers/", "Exadel.ReportHub.Handlers/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Host/", "Exadel.ReportHub.Host/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.RA/", "Exadel.ReportHub.RA/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Data/", "Exadel.ReportHub.Data/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.SDK/", "Exadel.ReportHub.SDK/"]


RUN dotnet publish "Exadel.ReportHub.Host/Exadel.ReportHub.Host.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .


ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=5000
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "Exadel.ReportHub.Host.dll"]
