# ZooTech Backend

## Descripción
ZooTech Backend es el servicio central de la plataforma de gestión ganadera. Está construido con **.NET 10** y sigue los principios de **Clean Architecture**, proporcionando una base sólida, mantenible y escalable.

## ??? Arquitectura
El proyecto está organizado en las siguientes capas:

- **ZooTech.Domain**: Contiene las entidades, excepciones de dominio y lógica de negocio central. Sin dependencias externas.
- **ZooTech.Application**: Define los casos de uso, interfaces de servicios y lógica de aplicación.
- **ZooTech.Infrastructure**: Implementa las interfaces definidas en las capas superiores (persistencia, servicios externos, etc.).
- **ZooTech.InterfaceAdapters**: Contiene controladores, presentadores y adaptadores para exponer la lógica de negocio.
- **ZooTech.API**: Punto de entrada de la aplicación utilizando **Minimal APIs**. Configuración de servicios y middleware.

## ?? Tecnologías Principales
- **Framework**: .NET 10
- **API**: ASP.NET Core Minimal APIs
- **Documentación**: OpenAPI (Swagger)
- **Pruebas**: xUnit para Unit e Integration tests

## ??? Configuración Local

### Requisitos Previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 o VS Code

### Ejecución
1. Navega a la carpeta de la API:
   ```bash
   cd src/ZooTech.API
   ```
2. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```
La API estará disponible en `http://localhost:5000` (o el puerto configurado en `launchSettings.json`).

## ?? Pruebas
Para ejecutar las pruebas del sistema:
```bash
dotnet test
```
