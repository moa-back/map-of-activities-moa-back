FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI


COPY /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI/*.csproj ./
RUN dotnet restore

COPY /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI/ .          
RUN dotnet build -c Release -o /app/build

COPY Directory.Build.props .

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapOfActivitiesAPI.dll"]

