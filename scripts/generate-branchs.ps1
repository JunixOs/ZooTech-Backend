# ============================================
# Script para crear ramas Git automáticamente
# Proyecto: ZoftTech
# ============================================

# Lista de nombres
$branches = @(
    "BRENNIS_CASTRO",
    "ADRIEL_GAVILAN",
    "ARACELY_GONZALES",
    "AMELIA_MAURICIO",
    "ANDERSON_MELENDEZ",
    "YONEL_ORDOÑEZ",
    "LEONARDO_PARDO",
    "DANIEL_PINEDO",
    "JOSE_QUESHYAC",
    "SEBASTHIAN_RAMIREZ",
    "JUAN_RAMOS",
    "JUAN_RENGIFO",
    "JORGE_ROMERO",
    "PAUL_TARAZONA",
    "JAVIER_TARRILLO",
    "ALAN_VALLES",
    "AKEMI_ZAMBRANO"
)

# Verificar que estamos en un repositorio Git
if (-not (Test-Path ".git")) {
    Write-Host "ERROR: No se encontró un repositorio Git en este directorio." -ForegroundColor Red
    exit
}

# Crear ramas
foreach ($branch in $branches) {

    # Nombre final de la rama
    $branchName = "$branch/feature"

    Write-Host "Creando rama: $branchName" -ForegroundColor Cyan

    # Crear rama
    git branch $branchName

    # Subir rama al remoto
    git push -u origin $branchName
}

Write-Host ""
Write-Host "===================================" -ForegroundColor Green
Write-Host "Todas las ramas fueron creadas." -ForegroundColor Green
Write-Host "===================================" -ForegroundColor Green