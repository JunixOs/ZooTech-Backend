# Política de Commits — Estándar del Equipo ZoftTech

> Documento oficial para la estandarización de commits del proyecto ZoftTech.

- [Política de Commits — Estándar del Equipo ZoftTech](#política-de-commits--estándar-del-equipo-zofttech)
- [1. Objetivo](#1-objetivo)
- [2. Regla Principal](#2-regla-principal)
- [3. Estructura Oficial del Commit](#3-estructura-oficial-del-commit)
  - [Formato completo](#formato-completo)
- [4. Tipos de Commit Permitidos](#4-tipos-de-commit-permitidos)
  - [feat](#feat)
  - [fix](#fix)
  - [docs](#docs)
  - [style](#style)
  - [refactor](#refactor)
  - [test](#test)
  - [chore](#chore)
  - [perf](#perf)
  - [ci](#ci)
  - [build](#build)
  - [revert](#revert)
- [5. Uso del Scope (Alcance)](#5-uso-del-scope-alcance)
  - [Ejemplos válidos](#ejemplos-válidos)
- [6. Scopes Oficiales del Proyecto](#6-scopes-oficiales-del-proyecto)
  - [Arquitectura](#arquitectura)
  - [Módulos funcionales](#módulos-funcionales)
  - [Técnicos](#técnicos)
- [7. Reglas Obligatorias](#7-reglas-obligatorias)
  - [7.1 Un commit = un propósito](#71-un-commit--un-propósito)
  - [7.2 La descripción debe ser corta y clara](#72-la-descripción-debe-ser-corta-y-clara)
  - [7.3 Escribir en infinitivo o imperativo](#73-escribir-en-infinitivo-o-imperativo)
  - [7.4 NO usar mayúsculas innecesarias](#74-no-usar-mayúsculas-innecesarias)
  - [7.5 NO usar punto final](#75-no-usar-punto-final)
  - [7.6 Commits prohibidos](#76-commits-prohibidos)
- [8. Breaking Changes](#8-breaking-changes)
- [9. Ejemplos Correctos](#9-ejemplos-correctos)
  - [Nuevo endpoint](#nuevo-endpoint)
  - [Corrección de bug](#corrección-de-bug)
  - [Refactor](#refactor-1)
  - [Pruebas](#pruebas)
  - [Documentación](#documentación)
- [10. Ejemplos Incorrectos](#10-ejemplos-incorrectos)
- [11. Política de Pull Requests](#11-política-de-pull-requests)
- [12. Política de Commits Temporales](#12-política-de-commits-temporales)
- [13. Reglas para el Equipo](#13-reglas-para-el-equipo)
  - [Obligatorio](#obligatorio)
- [14. Buenas Prácticas Recomendadas](#14-buenas-prácticas-recomendadas)
  - [Commits pequeños](#commits-pequeños)
  - [Commits frecuentes](#commits-frecuentes)
  - [Commits atómicos](#commits-atómicos)
- [15. Flujo Recomendado](#15-flujo-recomendado)
- [16. Herramientas Recomendadas](#16-herramientas-recomendadas)
  - [Commitlint](#commitlint)
  - [Commitizen](#commitizen)
  - [Semantic Release](#semantic-release)
- [17. Ejemplos Reales del Proyecto](#17-ejemplos-reales-del-proyecto)
  - [Domain](#domain)
  - [Application](#application)
  - [Infrastructure](#infrastructure)
  - [InterfaceAdapters](#interfaceadapters)
- [18. Consideraciones Finales](#18-consideraciones-finales)
- [19. Referencias](#19-referencias)


---

# 1. Objetivo

Este documento define las reglas y convenciones oficiales para la creación de commits dentro de los repositorios del proyecto ZoftTech.

El objetivo es garantizar:

* Historial Git legible y mantenible
* Facilidad para auditoría y revisión
* Mejor colaboración entre miembros del equipo
* Automatización futura de changelogs y versionado
* Mayor trazabilidad de cambios
* Estándares consistentes independientemente del nivel de experiencia

Este estándar está basado en:

* [Conventional Commits 1.0.0](https://www.conventionalcommits.org/es/v1.0.0/?utm_source=chatgpt.com)
* Semantic Versioning (SemVer)
* Buenas prácticas de ingeniería de software utilizadas en la industria moderna

([Conventional Commits][1])

---

# 2. Regla Principal

Todos los commits realizados en el proyecto DEBEN seguir el siguiente formato:

```text
<tipo>[alcance opcional]: <descripción>
```

Ejemplos:

```text
feat(auth): agregar autenticación JWT
fix(users): corregir validación de correo
docs(readme): actualizar instrucciones de instalación
refactor(api): simplificar manejo de excepciones
```

([Conventional Commits][1])

---

# 3. Estructura Oficial del Commit

## Formato completo

```text
<tipo>[alcance opcional]: <descripción corta>

[cuerpo opcional]

[pie opcional]
```

---

# 4. Tipos de Commit Permitidos

## feat

Nueva funcionalidad.

```text
feat(auth): implementar login con JWT
```

---

## fix

Corrección de errores.

```text
fix(users): corregir error al registrar usuario
```

---

## docs

Cambios únicamente en documentación.

```text
docs(api): actualizar endpoints de autenticación
```

---

## style

Cambios visuales o de formato que NO afectan lógica.

Ejemplos:

* espacios
* indentación
* formato
* linting

```text
style(domain): corregir formato de clases
```

---

## refactor

Reestructuración de código sin cambiar comportamiento funcional.

```text
refactor(application): simplificar handler de creación
```

---

## test

Adición o modificación de pruebas.

```text
test(auth): agregar pruebas unitarias de login
```

---

## chore

Tareas de mantenimiento o configuración.

Ejemplos:

* dependencias
* scripts
* configuración
* paquetes

```text
chore(ci): actualizar pipeline de GitHub Actions
```

---

## perf

Mejoras de rendimiento.

```text
perf(cache): optimizar consultas de usuarios
```

---

## ci

Cambios relacionados con integración continua o despliegue.

```text
ci(github): agregar validación automática de tests
```

---

## build

Cambios relacionados con compilación o dependencias de build.

```text
build(dotnet): actualizar SDK a .NET 9
```

---

## revert

Reversión de commits previos.

```text
revert(auth): revertir implementación de refresh token
```

---

# 5. Uso del Scope (Alcance)

El alcance es OPCIONAL, pero ALTAMENTE recomendado.

Permite identificar rápidamente qué módulo fue afectado.

## Ejemplos válidos

```text
feat(auth): agregar refresh tokens
fix(users): corregir validación
refactor(domain): mover reglas de negocio
test(application): agregar pruebas de CQRS
```

---

# 6. Scopes Oficiales del Proyecto

## Arquitectura

```text
domain
application
infrastructure
interface-adapters
api
```

---

## Módulos funcionales

```text
auth
users
animals
production
reproduction
health
inventory
reports
```

---

## Técnicos

```text
ci
docker
efcore
identity
cache
logging
testing
```

---

# 7. Reglas Obligatorias

## 7.1 Un commit = un propósito

Cada commit debe representar un cambio lógico y coherente.

✅ Correcto:

```text
feat(auth): agregar login JWT
```

❌ Incorrecto:

```text
feat: agregar login y corregir inventario y actualizar README
```

---

## 7.2 La descripción debe ser corta y clara

Máximo recomendado:

```text
50 caracteres
```

Ideal:

```text
feat(auth): agregar expiración de tokens
```

❌ Evitar:

```text
feat(auth): se agregó una funcionalidad muy compleja relacionada con JWT
```

---

## 7.3 Escribir en infinitivo o imperativo

✅ Correcto:

```text
fix(auth): corregir validación de contraseña
```

❌ Incorrecto:

```text
fix(auth): corregida validación
```

---

## 7.4 NO usar mayúsculas innecesarias

✅ Correcto:

```text
feat(users): agregar paginación
```

❌ Incorrecto:

```text
FEAT(Users): Agregar Paginación
```

---

## 7.5 NO usar punto final

✅ Correcto:

```text
docs(readme): actualizar instalación
```

❌ Incorrecto:

```text
docs(readme): actualizar instalación.
```

---

## 7.6 Commits prohibidos

Quedan prohibidos commits como:

```text
update
changes
fix
misc
asdf
prueba
avance
tmp
aaaa
```

---

# 8. Breaking Changes

Cuando un commit rompe compatibilidad anterior:

```text
feat(api)!: eliminar endpoint legacy
```

O:

```text
BREAKING CHANGE: se eliminó autenticación antigua
```

([Conventional Commits][2])

---

# 9. Ejemplos Correctos

## Nuevo endpoint

```text
feat(users): agregar endpoint de registro
```

---

## Corrección de bug

```text
fix(auth): corregir expiración de refresh token
```

---

## Refactor

```text
refactor(application): dividir handler de usuarios
```

---

## Pruebas

```text
test(domain): agregar pruebas de entidad Animal
```

---

## Documentación

```text
docs(readme): agregar guía de instalación
```

---

# 10. Ejemplos Incorrectos

❌

```text
cambios
```

❌

```text
fix stuff
```

❌

```text
Update
```

❌

```text
feat: muchas cosas
```

❌

```text
auth fix
```

---

# 11. Política de Pull Requests

Toda Pull Request debe:

* Tener commits limpios y descriptivos
* No incluir commits temporales
* No incluir código comentado innecesario
* Tener tests pasando
* Mantener coherencia con la arquitectura del proyecto

---

# 12. Política de Commits Temporales

Durante desarrollo local se permiten commits temporales.

ANTES de hacer push:

```bash
git rebase -i HEAD~N
```

El desarrollador DEBE:

* limpiar commits
* agrupar commits innecesarios
* renombrar commits incorrectos

---

# 13. Reglas para el Equipo

## Obligatorio

* Todos los commits deben seguir este estándar
* Todo miembro nuevo debe leer este documento
* Ningún commit puede romper compilación principal
* Todo commit debe compilar correctamente

---

# 14. Buenas Prácticas Recomendadas

## Commits pequeños

Preferir:

```text
5 commits claros
```

en lugar de:

```text
1 commit gigante
```

---

## Commits frecuentes

No esperar varios días para commitear.

---

## Commits atómicos

Cada commit debe poder:

```text
ser revertido independientemente
```

---

# 15. Flujo Recomendado

```text
feature branch
    ↓
commits convencionales
    ↓
pull request
    ↓
review
    ↓
merge
```

---

# 16. Herramientas Recomendadas

## Commitlint

Permite validar commits automáticamente.

[Commitlint](https://commitlint.js.org/?utm_source=chatgpt.com)

---

## Commitizen

Asistente interactivo para crear commits.

[Commitizen](https://commitizen-tools.github.io/commitizen/?utm_source=chatgpt.com)

---

## Semantic Release

Automatización de versionado y changelog.

[Semantic Release](https://semantic-release.gitbook.io/semantic-release/?utm_source=chatgpt.com)

---

# 17. Ejemplos Reales del Proyecto

## Domain

```text
feat(domain): agregar value object Email
```

---

## Application

```text
refactor(application): separar comandos y queries
```

---

## Infrastructure

```text
fix(infrastructure): corregir migración de EF Core
```

---

## InterfaceAdapters

```text
feat(interface-adapters): agregar endpoint de autenticación
```

---

# 18. Consideraciones Finales

El objetivo de esta política NO es burocracia.

El objetivo es:

* mejorar mantenibilidad
* facilitar onboarding
* permitir escalabilidad
* mejorar trazabilidad
* facilitar debugging
* profesionalizar el flujo de desarrollo

Los commits forman parte de la documentación técnica del proyecto.

Un historial Git limpio es un activo del equipo.

---

# 19. Referencias

* [Conventional Commits 1.0.0](https://www.conventionalcommits.org/es/v1.0.0/?utm_source=chatgpt.com)
* [Semantic Versioning](https://semver.org/lang/es/?utm_source=chatgpt.com)
* [Commitlint](https://commitlint.js.org/?utm_source=chatgpt.com)
* [Commitizen](https://commitizen-tools.github.io/commitizen/?utm_source=chatgpt.com)
* [Semantic Release](https://semantic-release.gitbook.io/semantic-release/?utm_source=chatgpt.com)

([Conventional Commits][1])

[1]: https://www.conventionalcommits.org/en/v1.0.0-beta/?utm_source=chatgpt.com "Conventional Commits"
[2]: https://www.conventionalcommits.org/en/v1.0.0/?embedable=true&utm_source=chatgpt.com "Conventional Commits"
