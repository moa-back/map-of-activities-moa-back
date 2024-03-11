FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY MapOfActivitiesAPI/*.csproj ./
RUN dotnet restore

COPY MapOfActivitiesAPI/ .          
RUN dotnet build -c Release -o /app/build

COPY Directory.Build.props .

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "MapOfActivitiesAPI.dll"]
