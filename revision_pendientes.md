# Reporte de Revisión Pendiente: Error 500 en Endpoints por Multitenancy

Se ha completado la fusión de la rama de Sebasthian resolviendo los conflictos a favor de tu rama (`HEAD`), logrando compilar y pasar el 100% de las pruebas unitarias. Sin embargo, al iniciar la API y probar los endpoints localmente, se identificó el siguiente comportamiento pendiente:

---

## 1. Comportamiento Observado
* **Acción:** Petición HTTP GET a `http://localhost:16000/api/v1/vacunos` (u otros endpoints).
* **Resultado:** Error de Servidor `500 Internal Server Error`.
* **Detalle del Error:**
  ```text
  System.InvalidOperationException: No se ha inicializado la propiedad ConnectionString.
     at Microsoft.Data.SqlClient.SqlConnection.PermissionDemand()
     at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(...)
     at ZooTech.Infrastructure.Tenant.TenantStore.GetBySubDomainAsync(String subDomain)
     at ZooTech.InterfaceAdapters.Middleware.TenantResolutionMiddleware.InvokeAsync(...)
  ```

---

## 2. Causa Raíz
* El `TenantResolutionMiddleware` intercepta todas las peticiones para mapear el inquilino (Tenant) a su respectiva base de datos utilizando el contexto `TenantCatalogDb`.
* Este contexto se registra en `Program.cs` usando la cadena de conexión `"TenantCatalogConnection"`.
* En el entorno de desarrollo local, tu archivo de secretos de usuario (`secrets.json` bajo `%APPDATA%\Microsoft\UserSecrets\a5c25857-40bb-4c2a-aaa8-6ee1dbed12a8\secrets.json`) no tiene definidas las cadenas de conexión multitenant necesarias:
  * `ConnectionStrings:TenantCatalogConnection`
  * `ConnectionStrings:TenantTemplate`

---

## 3. Recomendación de Solución
Agregar las claves ausentes en tus secretos locales de desarrollo (`secrets.json`) haciendo que apunten a la base de datos local o de pruebas que tiene el catálogo de inquilinos. Por ejemplo:

```json
  "ConnectionStrings:TenantCatalogConnection": "Server=69.164.246.85,1433;Database=zootech;User Id=aracely.gonzales;Password=ZooTech@2026#03;TrustServerCertificate=True;",
  "ConnectionStrings:TenantTemplate": "Server=69.164.246.85,1433;Database=zootech;User Id=aracely.gonzales;Password=ZooTech@2026#03;TrustServerCertificate=True;"
```
