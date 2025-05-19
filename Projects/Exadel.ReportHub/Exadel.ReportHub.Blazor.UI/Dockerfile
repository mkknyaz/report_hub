FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Сначала копируем только .csproj, props и ruleset для кэширования restore
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Blazor.UI/Exadel.ReportHub.Blazor.UI.csproj", "Exadel.ReportHub.Blazor.UI/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.Common/Exadel.ReportHub.Common.csproj", "Exadel.ReportHub.Common/"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.SDK/Exadel.ReportHub.SDK.csproj", "Exadel.ReportHub.SDK/"]
COPY ["Projects/Exadel.ReportHub/Directory.Build.props", "./"]
COPY ["Projects/Exadel.ReportHub/Exadel.ReportHub.ruleset", "./"]

RUN dotnet restore "Exadel.ReportHub.Blazor.UI/Exadel.ReportHub.Blazor.UI.csproj"

COPY Projects/Exadel.ReportHub/ .
WORKDIR "/src/Exadel.ReportHub.Blazor.UI"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:${PORT:-80}

EXPOSE 80

ENTRYPOINT ["dotnet", "Exadel.ReportHub.Blazor.UI.dll"]
