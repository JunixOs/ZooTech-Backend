# Este script permite convertir los archivos OpenAPI JSON en un PDF legible
#   Ejecuta el script:
#       .\Convert-SwaggerToPdf.ps1
#
#   Especifica el directorio donde estan los archivos OpenAPI JSON
#       .\Convert-SwaggerToPdf.ps1 -InputDirectory "../docs/api-docs-json"
#
#   Especifica el directorio donde se almacenaran los resultados
#       .\Convert-SwaggerToPdf.ps1 -OutputDirectory "../docs/api-docs-pdf"

param(
    [string]$InputDirectory = "../docs/api-docs/json",
    [string]$OutputDirectory = "../docs/api-docs/pdf"
)

$ErrorActionPreference = "Stop"

# Buscar Microsoft Edge
$edgeCandidates = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe",
    "$env:LOCALAPPDATA\Microsoft\Edge\Application\msedge.exe"
)

$edge = $edgeCandidates |
    Where-Object { $_ -and (Test-Path $_) } |
    Select-Object -First 1

if (-not $edge) {
    throw "No se encontró Microsoft Edge. Instálalo o ajusta la ruta del ejecutable."
}

New-Item -ItemType Directory -Force -Path $OutputDirectory |
    Out-Null

$jsonFiles = Get-ChildItem $InputDirectory -Filter "*.json"

if (-not $jsonFiles) {
    throw "No hay archivos JSON en $InputDirectory. Ejecuta primero Export-Swagger.ps1."
}

foreach ($file in $jsonFiles) {
    $htmlPath = Join-Path $InputDirectory "$($file.BaseName).html"
    $pdfPath = Join-Path $OutputDirectory "$($file.BaseName).pdf"

    Write-Host "Generando HTML: $($file.BaseName)"

    & npx --yes @redocly/cli build-docs $file.FullName `
        --output $htmlPath

    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $htmlPath)) {
        Write-Warning "No se pudo generar HTML para $($file.Name)"
        continue
    }

    $htmlUri = ([System.Uri]::new(
        (Resolve-Path $htmlPath).Path
    )).AbsoluteUri

    Write-Host "Generando PDF: $($file.BaseName)"

    & $edge `
        --headless `
        --disable-gpu `
        --no-pdf-header-footer `
        "--print-to-pdf=$((Join-Path (Resolve-Path $OutputDirectory).Path "$($file.BaseName).pdf"))" `
        $htmlUri

    if ($LASTEXITCODE -eq 0 -and (Test-Path $pdfPath)) {
        Write-Host "PDF generado: $pdfPath" -ForegroundColor Green
    }
    else {
        Write-Warning "No se pudo generar el PDF para $($file.BaseName)"
    }
}

Write-Host "`nConversión finalizada. Revisa $OutputDirectory"