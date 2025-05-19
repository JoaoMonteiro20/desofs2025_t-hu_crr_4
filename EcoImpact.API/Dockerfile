FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./EcoImpact.API.csproj", "EcoImpact.API/"]
COPY ["../EcoImpact.DataModel/EcoImpact.DataModel.csproj", "EcoImpact.DataModel/"]
RUN dotnet restore "./EcoImpact.API.csproj"
COPY . .
WORKDIR "/src/EcoImpact.API"
RUN dotnet build "EcoImpact.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcoImpact.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcoImpact.API.dll"]
