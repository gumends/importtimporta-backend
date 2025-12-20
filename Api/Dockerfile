# ============================
# BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia csproj de TODOS os projetos
COPY Api/Api.csproj Api/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Domain/Domain.csproj Domain/

RUN dotnet restore Api/Api.csproj

# Copia tudo
COPY . .

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
