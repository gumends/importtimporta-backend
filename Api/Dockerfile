# ============================
# BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os csproj (camadas)
COPY Api/Api.csproj Api/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

# Restore
RUN dotnet restore Api/Api.csproj

# Copia tudo
COPY . .

# Publish
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# ============================
# RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Start da aplicação
ENTRYPOINT ["dotnet", "api-importtimporta"]
