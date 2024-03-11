FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 30002
EXPOSE 443

ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.csproj ./

RUN dotnet restore "MapOfActivitiesAPI.csproj"
COPY . /src
RUN dotnet build "MapOfActivitiesAPI.csproj" -c Release -o /app/build

COPY Directory.Build.props .

FROM build AS publish
RUN dotnet publish "MapOfActivitiesAPI.csproj" -c Release -o /app/publish

FROM build as final
WORKDIR /moa-back/map-of-activities-moa-back/MapOfActivitiesAPI/Dockerfile
ENTRYPOINT [ "dotnet", "watch", "run" ]

#For deploy
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# #ENTRYPOINT dotnet watch run --no-restore
# ENTRYPOINT [ "dotnet", "TelegramMessages.dll" ]