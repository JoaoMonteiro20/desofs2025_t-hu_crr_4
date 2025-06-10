FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7020

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar e restaurar dependências
COPY EcoImpact.API/EcoImpact.API.csproj ./EcoImpact.API/
COPY EcoImpact.DataModel/EcoImpact.DataModel.csproj ./EcoImpact.DataModel/
RUN dotnet restore ./EcoImpact.API/EcoImpact.API.csproj

# Copiar tudo e build
COPY . .
WORKDIR /src/EcoImpact.API
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:7020
ENTRYPOINT ["dotnet", "EcoImpact.API.dll"]
