# Genera la documentacion completa en PDF de la API con Swagger
#   Para personalizar la carpeta de salida:
#       .\Generate-SwaggerDocs.ps1 `
#           -OutputDirectory "D:\Documentacion\ZooTech"
#   
#   Para indicar explícitamente el servidor y elegir módulos:
#       .\Generate-SwaggerDocs.ps1 `
#           -BaseUrl "http://localhost:16001" `
#           -OutputDirectory "D:\Documentacion\ZooTech" `
#           -Documents @(
#               "animales",
#               "produccion-leche",
#               "reproduccion-celo"
#           )
param(
    [string]$EnvPath = "../.env",
    [string]$BaseUrl,
    [string]$OutputDirectory = "../docs/api-docs",
    [string[]]$Documents = @(
        "administracion",
        "gestionusuarios",
        "reproduccion-celo",
        "reproduccion-fecundacion",
        "sanidad-triaje",
        "vacuno",
        "produccionleche"
    )
)

$ErrorActionPreference = "Stop"

# Resolver rutas relativas desde la carpeta del script
$scriptDirectory = $PSScriptRoot

if (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $scriptDirectory $OutputDirectory
}

if (-not [System.IO.Path]::IsPathRooted($EnvPath)) {
    $EnvPath = Join-Path $scriptDirectory $EnvPath
}

$exportScript = Join-Path $scriptDirectory "Get-OpenApi-Json.ps1"
$htmlScript = Join-Path $scriptDirectory "Convert-SwaggetToHtml.ps1"

foreach ($script in @($exportScript, $htmlScript)) {
    if (-not (Test-Path $script)) {
        throw "No se encontró el script: $script"
    }
}

$jsonDirectory = Join-Path $OutputDirectory "json"
$htmlDirectory = Join-Path $OutputDirectory "html"

New-Item -ItemType Directory -Force -Path $jsonDirectory |
    Out-Null

New-Item -ItemType Directory -Force -Path $htmlDirectory |
    Out-Null

Write-Host "`n=== 1. Descargando documentación Swagger ===" `
    -ForegroundColor Cyan

$exportArgs = @{
    EnvPath = $EnvPath
    OutputDirectory = $jsonDirectory
    Documents = $Documents
}

if ($BaseUrl) {
    $exportArgs.BaseUrl = $BaseUrl
}

& $exportScript @exportArgs

if ($LASTEXITCODE -ne 0) {
    throw "Falló la descarga de Swagger."
}

# Comprobar que existen documentos descargados
$jsonFiles = @(Get-ChildItem $jsonDirectory -Filter "*.json")

if ($jsonFiles.Count -eq 0) {
    throw "No se descargaron documentos JSON."
}

Write-Host "`n=== 2. Generando documentación PDF ===" `
    -ForegroundColor Cyan

& $htmlScript `
    -InputDirectory $jsonDirectory `
    -OutputDirectory $htmlDirectory

if (-not $?) {
    throw "Falló la conversión de los documentos JSON a HTML."
}

Write-Host "`n=== Proceso finalizado ===" -ForegroundColor Green
Write-Host "JSON: $jsonDirectory"
Write-Host "HTML: $htmlDirectory"