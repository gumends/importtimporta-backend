# ============================
# BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ============================
# RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia build final
COPY --from=build /app/publish .

# Porta interna do container
EXPOSE 80

# Variável padrão (Docker sobrescreve via compose)
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Start da aplicação
ENTRYPOINT ["dotnet", "api-importtimporta"]
