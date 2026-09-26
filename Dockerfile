# ============================================================
# ETAPA 1: Restore
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore

WORKDIR /src

# Copiamos primero los archivos de proyecto.
# Esto permite aprovechar la caché de Docker cuando
# solamente cambia el código fuente.
COPY ["ZooTech Backend - Solution.slnx" , "./"]

COPY src/ZooTech.API/ZooTech.API.csproj src/ZooTech.API/
COPY src/ZooTech.Application/ZooTech.Application.csproj src/ZooTech.Application/
COPY src/ZooTech.Domain/ZooTech.Domain.csproj src/ZooTech.Domain/
COPY src/ZooTech.Infrastructure/ZooTech.Infrastructure.csproj src/ZooTech.Infrastructure/
COPY src/ZooTech.InterfaceAdapters/ZooTech.InterfaceAdapters.csproj src/ZooTech.InterfaceAdapters/

# Si API depende de CodeGeneration, habría que copiar
# también su .csproj aquí.
# COPY tools/ZooTech.CodeGeneration/ZooTech.CodeGeneration.csproj tools/ZooTech.CodeGeneration/

RUN dotnet restore src/ZooTech.API/ZooTech.API.csproj


# ============================================================
# ETAPA 2: Build
# ============================================================
FROM restore AS build

COPY src/ src/

# Si CodeGeneration forma parte de las referencias de compilación:
# COPY tools/ tools/

RUN dotnet build \
    src/ZooTech.API/ZooTech.API.csproj \
    -c Release \
    --no-restore


# ============================================================
# ETAPA 3: Publish
# ============================================================
FROM build AS publish

RUN dotnet publish \
    src/ZooTech.API/ZooTech.API.csproj \
    -c Release \
    --no-restore \
    --no-build \
    -o /app/publish


# ============================================================
# ETAPA 4: Runtime
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Solo copiamos los archivos necesarios para ejecutar
# la aplicación. El SDK no llega a la imagen final.
COPY --from=publish /app/publish .

# Puerto HTTP de la aplicación.
EXPOSE 8080

# ASP.NET Core escuchará dentro del contenedor en HTTP:8080.
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ZooTech.API.dll"]