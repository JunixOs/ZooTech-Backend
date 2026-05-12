# =========================================================
# ELIMINAR PROYECTOS ANTERIORES (OPCIONAL)
# =========================================================

Remove-Item src/ZooTech.InterfaceAdapters -Recurse -Force
Remove-Item src/ZooTech.API -Recurse -Force

# =========================================================
# CREAR INTERFACE ADAPTERS
# =========================================================

dotnet new classlib `
-n ZooTech.InterfaceAdapters `
-o src/ZooTech.InterfaceAdapters

# =========================================================
# CREAR API HOST
# =========================================================

dotnet new webapi `
-n ZooTech.API `
-o src/ZooTech.API

# =========================================================
# ELIMINAR CLASS1
# =========================================================

Remove-Item src/ZooTech.InterfaceAdapters/Class1.cs -Force

# =========================================================
# ESTRUCTURA INTERFACE ADAPTERS
# =========================================================

mkdir src/ZooTech.InterfaceAdapters/Filters
mkdir src/ZooTech.InterfaceAdapters/Middleware
mkdir src/ZooTech.InterfaceAdapters/Modules

mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche

mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Controllers

mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/DTOs
mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/DTOs/Requests
mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/DTOs/Responses

mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Mappers
mkdir src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Presentes

# =========================================================
# .gitkeep
# =========================================================

New-Item src/ZooTech.InterfaceAdapters/Filters/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Middleware/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Controllers/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/DTOs/Requests/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/DTOs/Responses/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Mappers/.gitkeep -ItemType File

New-Item src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Presentes/.gitkeep -ItemType File

# =========================================================
# REFERENCIAS INTERFACE ADAPTERS
# =========================================================

dotnet add src/ZooTech.InterfaceAdapters `
reference src/ZooTech.Application

# =========================================================
# REFERENCIAS API
# =========================================================

dotnet add src/ZooTech.API `
reference src/ZooTech.Application

dotnet add src/ZooTech.API `
reference src/ZooTech.Infrastructure

dotnet add src/ZooTech.API `
reference src/ZooTech.InterfaceAdapters

# =========================================================
# AGREGAR A SOLUCIÓN
# =========================================================

dotnet sln add src/ZooTech.InterfaceAdapters
dotnet sln add src/ZooTech.API

# =========================================================
# TESTS API
# =========================================================

dotnet new xunit `
-n ZooTech.API.IntegrationTests `
-o tests/Integration/ZooTech.API.IntegrationTests

# =========================================================
# REFERENCIAS TESTS
# =========================================================

dotnet add tests/Integration/ZooTech.API.IntegrationTests `
reference src/ZooTech.API

# =========================================================
# AGREGAR TESTS A SOLUCIÓN
# =========================================================

dotnet sln add tests/Integration/ZooTech.API.IntegrationTests

# =========================================================
# RESTORE
# =========================================================

dotnet restore

# =========================================================
# FIN
# =========================================================

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host " ZooTech.API recreado correctamente" -ForegroundColor Green
Write-Host " ZooTech.InterfaceAdapters recreado" -ForegroundColor Green
Write-Host " Tests creados correctamente" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""