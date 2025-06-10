# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY ["EcoImpact.Frontend/EcoImpact.Frontend.csproj", "EcoImpact.Frontend/"]
RUN dotnet restore "EcoImpact.Frontend/EcoImpact.Frontend.csproj"
COPY . .
WORKDIR "/app/EcoImpact.Frontend"
RUN dotnet publish "EcoImpact.Frontend.csproj" -c Release -o /app/publish

# Etapa final: servir com Nginx
FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Define o ambiente para carregar o appsettings certo
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 80