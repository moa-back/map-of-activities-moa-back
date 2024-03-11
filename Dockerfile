FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Assuming your project files are in a subdirectory named 'src'
COPY /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI/*.csproj ./
RUN dotnet restore

COPY /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI/ .             # Make sure 'src' directory exists in your Docker context
RUN dotnet build -c Release -o /app/build

COPY Directory.Build.props .

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapOfActivitiesAPI.dll"]

