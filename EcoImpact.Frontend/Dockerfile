# Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY EcoImpact.Frontend.csproj .
RUN dotnet restore "EcoImpact.Frontend.csproj"
COPY . .
RUN dotnet publish "EcoImpact.Frontend.csproj" -c Release -o /app/publish

# Imagem final com Nginx
FROM nginx:alpine
COPY nginx/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html