\# Política de Desarrollo Colaborativo — Proyecto ZoftTech



> Documento oficial para la estandarización del desarrollo colaborativo del proyecto ZoftTech.



\---



\# 1. Objetivo



Este documento define las normas, responsabilidades y estándares técnicos que deben seguir todos los miembros del equipo durante el desarrollo del proyecto.



El propósito es:



\* Reducir conflictos entre desarrolladores

\* Mantener consistencia técnica

\* Mejorar mantenibilidad del sistema

\* Facilitar onboarding de nuevos miembros

\* Garantizar calidad del software

\* Estandarizar el flujo de trabajo

\* Minimizar deuda técnica

\* Proteger la arquitectura del proyecto



Este documento toma como referencia:



\* Buenas prácticas oficiales de \[Microsoft .NET Documentation](https://learn.microsoft.com/dotnet/?utm\_source=chatgpt.com)

\* Clean Architecture

\* Domain-Driven Design

\* Git Flow

\* Conventional Commits

\* Principios SOLID

\* Guías de ingeniería de software enterprise



\---



\# 2. Principios Generales



Todo desarrollo dentro del proyecto debe seguir estos principios:



\* Simplicidad

\* Legibilidad

\* Mantenibilidad

\* Modularidad

\* Responsabilidad única

\* Bajo acoplamiento

\* Alta cohesión

\* Código autodocumentado

\* Escalabilidad

\* Testabilidad



\---



\# 3. Arquitectura Oficial del Proyecto



La arquitectura oficial aprobada para el proyecto es:



```text

src/

&#x20;├── ZoftTech.Domain

&#x20;├── ZoftTech.Application

&#x20;├── ZoftTech.Infrastructure

&#x20;└── ZoftTech.InterfaceAdapters

```



\---



\# 4. Responsabilidad de Cada Capa



\---



\## 4.1 Domain



Contiene:



\* Entidades

\* Value Objects

\* Reglas de negocio

\* Excepciones de dominio

\* Eventos de dominio

\* Enumeraciones

\* Interfaces puramente de dominio



\### Restricciones



NO puede depender de:



\* ASP.NET Core

\* EF Core

\* Infrastructure

\* Controllers

\* HttpContext

\* ILogger

\* Bases de datos

\* Frameworks externos



\---



\## 4.2 Application



Contiene:



\* Casos de uso

\* CQRS

\* Commands

\* Queries

\* Handlers

\* Validators

\* Interfaces

\* DTOs internos

\* Behaviors



\### Restricciones



NO puede depender de:



\* EF Core directamente

\* Controllers

\* ASP.NET Core MVC

\* Infrastructure concreta



\---



\## 4.3 Infrastructure



Contiene:



\* EF Core

\* Persistencia

\* Identity

\* JWT

\* Redis

\* SMTP

\* APIs externas

\* Repositories concretos

\* Servicios externos



\### Restricciones



NO puede contener:



\* Reglas de negocio

\* Controllers

\* Casos de uso



\---



\## 4.4 InterfaceAdapters



Contiene:



\* Controllers ASP.NET Core

\* DTOs HTTP

\* Middleware

\* Filters

\* Presenters

\* Mappers

\* Configuración web



\### Restricciones



NO debe contener:



\* Reglas de negocio

\* Acceso directo a EF Core

\* Lógica compleja



\---



\# 5. Reglas de Dependencia



Las dependencias permitidas son:



```text

InterfaceAdapters

&#x20;       ↓

Application

&#x20;       ↓

Domain



Infrastructure

&#x20;       ↓

Application

&#x20;       ↓

Domain

```



\---



\# 6. Dependencias Prohibidas



Queda prohibido:



```text

Domain -> Infrastructure

Domain -> InterfaceAdapters

Application -> InterfaceAdapters

Application -> EF Core

Controllers -> DbContext

```



\---



\# 7. Política de Controllers



Los controllers deben:



\* Ser ligeros

\* Tener una sola responsabilidad

\* Delegar lógica a Application

\* Validar únicamente entrada HTTP básica

\* Retornar respuestas HTTP apropiadas



\---



\## Controllers NO deben:



\* Ejecutar lógica de negocio

\* Acceder a DbContext

\* Usar SQL

\* Realizar cálculos complejos

\* Implementar reglas del dominio



\---



\# 8. Política de Casos de Uso



Cada caso de uso debe:



\* Tener responsabilidad única

\* Ser independiente

\* Ser testeable

\* Tener nombre explícito



\---



\## Convención



```text

CreateUserCommand

CreateUserCommandHandler



GetUserByIdQuery

GetUserByIdQueryHandler

```



\---



\# 9. Política de Entidades



Las entidades deben:



\* Representar conceptos del negocio

\* Proteger invariantes

\* Encapsular comportamiento

\* Evitar setters públicos innecesarios



\---



\## Incorrecto



```csharp

public string Name { get; set; }

```



\---



\## Correcto



```csharp

public string Name { get; private set; }

```



\---



\# 10. Política de DTOs



Los DTOs deben:



\* Ser simples

\* No contener lógica

\* Representar transporte de datos

\* No ser reutilizados entre capas incorrectamente



\---



\# 11. Política de Validaciones



\---



\## Validaciones de entrada



Se realizan en:



```text

Application

```



usando:



\* FluentValidation

\* Behaviors



\---



\## Reglas de negocio



Se realizan en:



```text

Domain

```



\---



\# 12. Política de Base de Datos



\---



\## Migraciones



Las migraciones:



\* deben tener nombres descriptivos

\* deben ser pequeñas

\* no deben incluir cambios innecesarios



\---



\## Ejemplo



```text

AddUserRefreshTokens

```



NO:



```text

UpdateDatabase

```



\---



\# 13. Política de Branches



\---



\## Branch principal



```text

main

```



Debe permanecer:



\* estable

\* compilando

\* testeada



\---



\## Branch de desarrollo



```text

develop

```



\---



\## Feature branches



Formato obligatorio:



```text

feature/nombre-feature

```



Ejemplos:



```text

feature/auth-jwt

feature/user-registration

```



\---



\## Fix branches



```text

fix/nombre-fix

```



\---



\# 14. Política de Pull Requests



Toda Pull Request debe:



\* compilar correctamente

\* pasar tests

\* respetar arquitectura

\* respetar naming conventions

\* respetar Conventional Commits



\---



\## Toda PR debe incluir:



\* objetivo

\* descripción técnica

\* impacto

\* evidencia de pruebas



\---



\# 15. Política de Code Review



Ningún código debe llegar a:



```text

main

```



sin revisión.



\---



\## El reviewer debe verificar:



\* arquitectura

\* calidad

\* naming

\* seguridad

\* performance

\* duplicación

\* claridad

\* complejidad



\---



\# 16. Política de Naming



\---



\## Clases



PascalCase



```csharp

CreateAnimalCommand

```



\---



\## Métodos



PascalCase



```csharp

GetByIdAsync()

```



\---



\## Variables privadas



```csharp

\_privateField

```



\---



\## Interfaces



```csharp

IUserRepository

```



\---



\# 17. Política de Asincronía



Todo acceso IO debe ser async.



\---



\## Correcto



```csharp

await repository.GetByIdAsync(id);

```



\---



\## Incorrecto



```csharp

repository.GetById(id);

```



\---



\# 18. Política de Exceptions



NO usar exceptions para flujo normal.



\---



\## Correcto



```csharp

Result<T>

```



o:



```text

ValidationResult

```



\---



\# 19. Política de Logs



Los logs deben:



\* ser claros

\* tener contexto

\* no exponer secretos



\---



\## Prohibido



\* passwords

\* tokens

\* connection strings

\* datos sensibles



\---



\# 20. Política de Secrets



Queda prohibido subir:



\* claves

\* passwords

\* tokens

\* certificados

\* connection strings reales



\---



\## Uso obligatorio



\* variables de entorno

\* Secret Manager

\* Azure Key Vault

\* AWS Secrets Manager



\---



\# 21. Política de Testing



Todo código crítico debe tener pruebas.



\---



\## Tipos mínimos



\### Unit Tests



\* Domain

\* Application



\---



\### Integration Tests



\* Infrastructure

\* APIs



\---



\# 22. Política de Cobertura



Objetivo mínimo recomendado:



```text

80%

```



en:



\* lógica crítica

\* casos de uso

\* dominio



\---



\# 23. Política de Dependencias



Antes de agregar una librería:



Debe evaluarse:



\* mantenimiento

\* licencia

\* seguridad

\* comunidad

\* necesidad real



\---



\# 24. Política de Refactorización



Toda refactorización debe:



\* mantener comportamiento

\* mantener tests pasando

\* reducir complejidad

\* mejorar claridad



\---



\# 25. Política de Deuda Técnica



La deuda técnica debe:



\* registrarse

\* documentarse

\* priorizarse



\---



\# 26. Política de Documentación



Debe documentarse:



\* arquitectura

\* módulos críticos

\* flujos importantes

\* decisiones técnicas



\---



\# 27. Política de Seguridad



Queda prohibido:



\* SQL Injection

\* concatenación SQL

\* exponer stack traces

\* hardcodear secretos



\---



\# 28. Política de Performance



Evitar:



\* consultas N+1

\* carga innecesaria

\* loops costosos

\* operaciones bloqueantes



\---



\# 29. Política de Comunicación Técnica



Toda decisión arquitectónica importante debe:



\* discutirse

\* documentarse

\* aprobarse



\---



\# 30. Resolución de Conflictos Técnicos



Cuando exista desacuerdo técnico:



1\. Revisar estándares oficiales

2\. Revisar arquitectura aprobada

3\. Evaluar impacto

4\. Consultar líder técnico

5\. Documentar decisión



\---



\# 31. Herramientas Obligatorias



\---



\## Control de versiones



\* Git



\---



\## IDEs permitidos



\* Visual Studio

\* Rider

\* VS Code



\---



\## Formateo



\* EditorConfig

\* dotnet format



\---



\## Testing



\* MSTest

\* xUnit

\* NUnit



\---



\# 32. Checklist Obligatorio Antes de Push



Todo desarrollador debe verificar:



\* \[ ] Compila correctamente

\* \[ ] Tests pasan

\* \[ ] No hay warnings críticos

\* \[ ] Commits cumplen estándar

\* \[ ] No hay secretos

\* \[ ] No hay código comentado innecesario

\* \[ ] Arquitectura respetada

\* \[ ] No hay archivos basura



\---



\# 33. Conducta Profesional



Se espera:



\* respeto técnico

\* colaboración

\* comunicación clara

\* revisión constructiva

\* responsabilidad



\---



\# 34. Consideraciones Finales



La arquitectura y estándares NO son opcionales.



Todo miembro del equipo acepta estas reglas al contribuir al proyecto.



El incumplimiento reiterado puede provocar:



\* rechazo de PRs

\* solicitud de refactorización

\* bloqueo temporal de merges



\---



\# 35. Referencias



\* \[Microsoft .NET Documentation](https://learn.microsoft.com/dotnet/?utm\_source=chatgpt.com)

\* \[ASP.NET Core Architecture Guide](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/?utm\_source=chatgpt.com)

\* \[Entity Framework Core Documentation](https://learn.microsoft.com/ef/core/?utm\_source=chatgpt.com)

\* \[Conventional Commits](https://www.conventionalcommits.org/es/v1.0.0/?utm\_source=chatgpt.com)

\* \[SOLID Principles Overview](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/architectural-principles?utm\_source=chatgpt.com#solid)

\* \[Git Documentation](https://git-scm.com/doc?utm\_source=chatgpt.com)



