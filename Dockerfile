# API + SPA de TallerGo en una sola imagen. El frontend compilado vive en ./wwwroot.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
COPY wwwroot ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    Data__Ruta=/data \
    Backups__Ruta=/backups

RUN mkdir -p /data /backups && chown -R app:app /data /backups
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "Backend-TallerGo.dll"]