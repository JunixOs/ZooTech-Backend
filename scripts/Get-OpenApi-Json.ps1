
# Este script permite obtener el OpenAPI JSON de un proyecto con Swagger
#   Para indicar una URL manualmente:
#       .\Export-Swagger.ps1 -BaseUrl "http://localhost:16001"
# 
# 
#   Para utilizar otro archivo de entorno:
#       .\Export-Swagger.ps1 -EnvPath ".env.Development"
#
#   Para especificar otro directorio de salida:
#       .\Export-Swagger.ps1 -OutputDirectory "../docs/api-docs-json"
# 
#   Para cambiar los documentos que deseas exportar:
#       .\Export-Swagger.ps1 `
#           -BaseUrl "http://localhost:16001" `
#           -Documents @("animales", "reproduccion-celo")

param(
    [string]$EnvPath = "../.env",
    [string]$BaseUrl,
    [string[]]$Documents = @(
        "administracion",
        "gestionusuarios",
        "reproduccion-celo",
        "reproduccion-fecundacion",
        "sanidad-triaje",
        "vacuno",
        "produccionleche"
    ),
    [string]$OutputDirectory = "../docs/api-docs/json"
)

$ErrorActionPreference = "Stop"

# 1. Leer el archivo .env
if (Test-Path $EnvPath) {
    Get-Content $EnvPath | ForEach-Object {
        $line = $_.Trim()

        if ($line -match '^\s*([^#=\s]+)\s*=\s*(.*?)\s*$') {
            $name = $matches[1]
            $value = $matches[2].Trim('"').Trim("'")

            # No sobrescribir variables de entorno existentes
            if (-not [Environment]::GetEnvironmentVariable($name)) {
                [Environment]::SetEnvironmentVariable(
                    $name, $value, "Process"
                )
            }
        }
    }
}

# 2. Determinar la URL base
if (-not $BaseUrl) {
    $urls = $env:ASPNETCORE_URLS

    if ($urls) {
        $firstUrl = ($urls -split ';|,')[0].Trim()

        # Reemplazar comodines de escucha por localhost
        $BaseUrl = $firstUrl `
            -replace '://\+', '://localhost' `
            -replace '://\*', '://localhost'
    }
    elseif ($env:ASPNETCORE_HTTP_PORTS) {
        $BaseUrl = "http://localhost:$($env:ASPNETCORE_HTTP_PORTS.Split(';')[0])"
    }
    else {
        throw "No se encontró la URL. Usa -BaseUrl o configura ASPNETCORE_URLS en .env."
    }
}

$BaseUrl = $BaseUrl.TrimEnd('/')

Write-Host "URL base: $BaseUrl"
Write-Host "Documentos: $($Documents -join ', ')"

# 3. Validar conexión antes de descargar
Write-Host "`nComprobando conexión con Swagger..."

$connected = $false

foreach ($document in $Documents) {
    $testUrl = "$BaseUrl/swagger/$document/swagger.json"

    try {
        $response = Invoke-WebRequest `
            -Uri $testUrl `
            -Method Get `
            -TimeoutSec 8

        if ($response.StatusCode -eq 200) {
            $connected = $true
            Write-Host "Conexión correcta. Documento disponible: $document" -ForegroundColor Green
            break
        }
    }
    catch {
        Write-Host "No disponible: $testUrl"
    }
}

if (-not $connected) {
    throw "No se encontró ningún documento Swagger disponible. Verifica que la API esté ejecutándose, la URL base y los nombres de los documentos."
}

# 4. Crear la carpeta de salida
New-Item -ItemType Directory -Force -Path $OutputDirectory |
    Out-Null

# 5. Descargar cada documento
$success = 0
$failed = 0

foreach ($document in $Documents) {
    $url = "$BaseUrl/swagger/$document/swagger.json"
    $destination = Join-Path $OutputDirectory "$document.json"
    $tempFile = "$destination.tmp"

    try {
        Invoke-WebRequest `
            -Uri $url `
            -OutFile $tempFile `
            -TimeoutSec 30

        # Verificar que la respuesta sea JSON válido
        $json = Get-Content $tempFile -Raw |
            ConvertFrom-Json

        if (-not $json.openapi -and -not $json.swagger) {
            throw "La respuesta no parece un documento OpenAPI/Swagger válido."
        }

        Move-Item $tempFile $destination -Force
        Write-Host "Descargado: $document" -ForegroundColor Green
        $success++
    }
    catch {
        Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
        Write-Warning "Error con '$document': $($_.Exception.Message)"
        $failed++
    }
}

Write-Host "`nProceso finalizado."
Write-Host "Correctos: $success"
Write-Host "Fallidos: $failed"
Write-Host "Archivos: $((Resolve-Path $OutputDirectory).Path)"
