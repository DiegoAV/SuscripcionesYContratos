# syntax=docker/dockerfile:1

# =========================
# Build (SDK .NET 8)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero los .csproj para aprovechar caché de Docker
COPY SuscripcionesYContratos.API/SuscripcionesYContratos.API.csproj SuscripcionesYContratos.API/
COPY SuscripcionesYContratos.Aplicacion/SuscripcionesYContratos.Aplicacion.csproj SuscripcionesYContratos.Aplicacion/
COPY SuscripcionesYContratos.Infraestructura/SuscripcionesYContratos.Infraestructura.csproj SuscripcionesYContratos.Infraestructura/
COPY SuscripcionesYContratos.Dominio/SuscripcionesYContratos.Dominio.csproj SuscripcionesYContratos.Dominio/

RUN dotnet restore SuscripcionesYContratos.API/SuscripcionesYContratos.API.csproj

# Copiamos el resto del código
COPY . .

# Publicamos
RUN dotnet publish SuscripcionesYContratos.API/SuscripcionesYContratos.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime (ASP.NET .NET 8)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SuscripcionesYContratos.API.dll"]