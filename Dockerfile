# Imagen base de .NET para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Exponer el puerto 8080 para la API
EXPOSE 8080

# Imagen para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["purpuraMain.csproj", "./"]
RUN dotnet restore

# Copiar el resto de los archivos y compilar en modo Release
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Imagen final con la app lista para ejecutar
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "purpuraMain.dll"]