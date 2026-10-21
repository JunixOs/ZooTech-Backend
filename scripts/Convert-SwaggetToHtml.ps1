# Este script permite convertir los archivos OpenAPI JSON en un HTML
#   Ejecuta el script:
#       .\Convert-SwaggetToHtml.ps1
#
#   Especifica el directorio donde estan los archivos OpenAPI JSON
#       .\Convert-SwaggetToHtml.ps1 -InputDirectory "../docs/api-docs-json"
#
#   Especifica el directorio donde se almacenaran los resultados
#       .\Convert-SwaggetToHtml.ps1 -OutputDirectory "../docs/api-docs-html"


param(
    [string]$InputDirectory = "./docs/api/json",
    [string]$OutputDirectory = "./docs/api/html"
)

$ErrorActionPreference = "Stop"

# Resolver rutas relativas desde la raíz del proyecto.
$ProjectRoot = Split-Path -Parent $PSScriptRoot

if (-not [System.IO.Path]::IsPathRooted($InputDirectory)) {
    $InputDirectory = Join-Path $ProjectRoot $InputDirectory
}

if (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $ProjectRoot $OutputDirectory
}

if (-not (Test-Path -LiteralPath $InputDirectory -PathType Container)) {
    throw "No existe el directorio de entrada: $InputDirectory"
}

$JsonFiles = @(
    Get-ChildItem -LiteralPath $InputDirectory -Filter "*.json" -File
)

if ($JsonFiles.Count -eq 0) {
    throw "No se encontraron documentos JSON en: $InputDirectory"
}

New-Item -ItemType Directory -Path $OutputDirectory -Force |
    Out-Null

foreach ($JsonFile in $JsonFiles) {
    Write-Host "Generando Swagger UI: $($JsonFile.Name)"

    # Leer y validar el documento OpenAPI.
    try {
        $JsonContent = Get-Content -LiteralPath $JsonFile.FullName -Raw
        $OpenApiDocument = $JsonContent | ConvertFrom-Json
    }
    catch {
        throw "JSON inválido en '$($JsonFile.Name)': $($_.Exception.Message)"
    }

    if (-not $OpenApiDocument.openapi -and -not $OpenApiDocument.swagger) {
        throw "'$($JsonFile.Name)' no parece un documento OpenAPI/Swagger."
    }

    # Preparar el JSON para incrustarlo de forma segura en un script.
    $SpecJson = $OpenApiDocument |
        ConvertTo-Json -Depth 100 -Compress

    $SpecJson = $SpecJson.Replace("&", "\u0026")
    $SpecJson = $SpecJson.Replace("<", "\u003c")
    $SpecJson = $SpecJson.Replace(">", "\u003e")

    $PageTitle = [System.Net.WebUtility]::HtmlEncode(
        $OpenApiDocument.info.title
    )

    if ([string]::IsNullOrWhiteSpace($PageTitle)) {
        $PageTitle = [System.Net.WebUtility]::HtmlEncode(
            $JsonFile.BaseName
        )
    }

    $HtmlPath = Join-Path $OutputDirectory "$($JsonFile.BaseName).html"

    $Html = @"
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="Documentación de la API ZooTech">
    <title>$PageTitle</title>

    <link rel="stylesheet"
          href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css">

    <style>
        body {
            margin: 0;
            background: #fafafa;
        }

        .swagger-ui .topbar {
            background-color: #172554;
        }

        .swagger-ui .topbar-wrapper {
            padding: 8px 0;
        }

        .swagger-ui .info {
            margin: 32px 0;
        }

        .swagger-ui .scheme-container {
            box-shadow: none;
            background: #fff;
        }
    </style>
</head>
<body>
    <div id="swagger-ui"></div>

    <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
    <script>
        window.onload = function () {
            SwaggerUIBundle({
                spec: $SpecJson,
                dom_id: "#swagger-ui",
                deepLinking: true,
                docExpansion: "list",
                filter: true,
                displayRequestDuration: true,
                persistAuthorization: false,
                presets: [
                    SwaggerUIBundle.presets.apis
                ],
                layout: "BaseLayout"
            });
        };
    </script>
</body>
</html>
"@

    Set-Content -LiteralPath $HtmlPath -Value $Html -Encoding utf8

    Write-Host "Generado: $HtmlPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "Documentación Swagger UI generada en: $OutputDirectory" `
    -ForegroundColor Green