# Skill: Integration Testing en Multi-tenant Clean Architecture

## Objetivo
Validar la comunicación correcta entre el Controller, el Interactor y la Persistencia real (o InMemory).

## Estándares Técnicos
- **Herramienta:** WebApplicationFactory para levantar el pipeline de ASP.NET Core en memoria.
- **Base de Datos:** Utilizar Entity Framework Core InMemory o una instancia de SQL Server de pruebas.
- **Aislamiento de Datos:** La prueba DEBE incluir el envío de un Header de Tenant (ej. `X-Tenant-Id` o Subdominio) para validar que el Middleware de resolución funcione.

## Flujo de Validación
1. Simular la petición HTTP hacia el Controller en la capa de InterfaceAdapters.
2. El Middleware debe resolver el Tenant y guardarlo en el `ITenantContext`.
3. El `AnimalRepository` debe usar el `ITenantDbContextFactory` para conectarse a la DB correcta.
4. Validar que la respuesta HTTP (JSON) coincida con los datos insertados en la base de datos del Tenant específico.

## Reglas Críticas para ZooTech
- **Aislamiento de Tenants:** Crear dos bases de datos temporales (Tenant A y Tenant B). Insertar datos en A y validar que la consulta al Tenant B devuelva una lista vacía.
- **Mapping:** Verificar que el `AnimalMapper` de la infraestructura no pierda datos al convertir de `AnimalEntity` a `Animal` (Dominio).