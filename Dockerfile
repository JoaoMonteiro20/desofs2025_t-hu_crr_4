# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7020

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EcoImpact.API/EcoImpact.API.csproj EcoImpact.API/
COPY EcoImpact.DataModel/EcoImpact.DataModel.csproj EcoImpact.DataModel/
COPY EcoImpact.Tests/EcoImpact.Tests.csproj EcoImpact.Tests/

RUN dotnet restore EcoImpact.API/EcoImpact.API.csproj

COPY EcoImpact.API/ ./EcoImpact.API/
COPY EcoImpact.DataModel/ ./EcoImpact.DataModel/
COPY EcoImpact.Tests/ ./EcoImpact.Tests/

WORKDIR /src/EcoImpact.API
RUN dotnet build EcoImpact.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish EcoImpact.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=publish /app/publish .

USER appuser

ENV ASPNETCORE_URLS=http://+:7020
ENTRYPOINT ["dotnet", "EcoImpact.API.dll"]


## Base runtime
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#EXPOSE 7020
#
## SDK image for build
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#
## Copiar projetos e restaurar
#COPY EcoImpact.API/EcoImpact.API.csproj EcoImpact.API/
#COPY EcoImpact.DataModel/EcoImpact.DataModel.csproj EcoImpact.DataModel/
#COPY EcoImpact.Tests/EcoImpact.Tests.csproj EcoImpact.Tests/
#
#RUN dotnet restore EcoImpact.API/EcoImpact.API.csproj
#
## Copiar tudo e build
#COPY . .
#WORKDIR /src/EcoImpact.API
#RUN dotnet build EcoImpact.API.csproj -c Release -o /app/build
#
## Publish
#FROM build AS publish
#RUN dotnet publish EcoImpact.API.csproj -c Release -o /app/publish
#
## Final image
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENV ASPNETCORE_URLS=http://+:7020
#ENTRYPOINT ["dotnet", "EcoImpact.API.dll"]



## Base runtime
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#EXPOSE 7020
#
## SDK image for build
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#
## Copiar projetos individualmente para restaurar depend?ncias
#COPY EcoImpact.API/EcoImpact.API.csproj EcoImpact.API/
#COPY EcoImpact.DataModel/EcoImpact.DataModel.csproj EcoImpact.DataModel/
#COPY EcoImpact.Tests/EcoImpact.Tests.csproj EcoImpact.Tests/
#
## Restaurar depend?ncias
#RUN dotnet restore EcoImpact.API/EcoImpact.API.csproj
#
## Copiar tudo
#COPY . .
#
## Build
#WORKDIR /src/EcoImpact.API
#RUN dotnet build EcoImpact.API.csproj -c Release -o /app/build
#
## Publish
#FROM build AS publish
#RUN dotnet publish EcoImpact.API.csproj -c Release -o /app/publish
#
## Final image
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "EcoImpact.API.dll"]
#