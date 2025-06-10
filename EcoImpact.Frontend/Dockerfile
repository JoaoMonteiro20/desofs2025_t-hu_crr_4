FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EcoImpact.Frontend/EcoImpact.Frontend.csproj", "EcoImpact.Frontend/"]
RUN dotnet restore "EcoImpact.Frontend/EcoImpact.Frontend.csproj"
COPY . .
WORKDIR "/src/EcoImpact.Frontend"
RUN dotnet publish "EcoImpact.Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EcoImpact.Frontend.dll"]