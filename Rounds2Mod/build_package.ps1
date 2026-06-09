# Builds all three mods and assembles a Thunderstore-ready ZIP.
# Usage: .\build_package.ps1
# Requires icon.png in ThunderstorePackage\ (committed to the repo).

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot

$ModPackProject = Join-Path $Root "ModPack.csproj"

$Projects = @(
    @{ Name = "DeckBuilder"; Project = "DeckBuilder\DeckBuilder.csproj"; Dll = "DeckBuilder.dll" },
    @{ Name = "ShieldsMod";  Project = "ShieldsMod\ShieldsMod.csproj";                 Dll = "ShieldsMod.dll" },
    @{ Name = "InfoOverhaul"; Project = "InfographicsOverhaul\InfoOverhaul.csproj";   Dll = "InfoOverhaul.dll" }
)

$PackageDir = Join-Path $Root "ThunderstorePackage"
$ManifestPath = Join-Path $PackageDir "manifest.json"
$IconPath = Join-Path $PackageDir "icon.png"

if (-not (Test-Path $ManifestPath)) {
    Write-Error "Missing $ManifestPath"
}

$manifest = Get-Content $ManifestPath -Raw | ConvertFrom-Json
$version = $manifest.version_number
$zipName = "RoundsTheGathering_$version.zip"
$staging = Join-Path $Root "Release_Build"
$stageRoot = Join-Path $staging "stage"

Write-Host "=== Building Rounds The Gathering v$version ===" -ForegroundColor Cyan

Write-Host "Building all mods (ModPack)..." -ForegroundColor Yellow
dotnet build $ModPackProject -c Release
if ($LASTEXITCODE -ne 0) { Write-Error "Build failed: ModPack.csproj" }

if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory -Path $stageRoot -Force | Out-Null

Copy-Item $ManifestPath (Join-Path $stageRoot "manifest.json")
Copy-Item (Join-Path $PackageDir "README.md") (Join-Path $stageRoot "README.md")

if (-not (Test-Path $IconPath)) {
    Write-Error "icon.png not found at $IconPath. The file should be committed to ThunderstorePackage\."
}
Copy-Item $IconPath (Join-Path $stageRoot "icon.png")

foreach ($p in $Projects) {
    $projectDir = Join-Path $Root (Split-Path $p.Project -Parent)
    $dllSrc = Join-Path $projectDir "bin\Release\net471\$($p.Dll)"
    if (-not (Test-Path $dllSrc)) {
        Write-Error "DLL not found: $dllSrc (did the build succeed?)"
    }
    Copy-Item $dllSrc (Join-Path $stageRoot $p.Dll)
    Write-Host "  + $($p.Dll)" -ForegroundColor Green
}

$assetsDst = Join-Path $stageRoot "assets"
New-Item -ItemType Directory -Path $assetsDst -Force | Out-Null

$assetDirs = @(
    (Join-Path $Root "ShieldsMod\assets"),
    (Join-Path $Root "DeckBuilder\assets")
)
foreach ($assetsSrc in $assetDirs) {
    if (Test-Path $assetsSrc) {
        Copy-Item (Join-Path $assetsSrc "*.png") $assetsDst -Force
    } else {
        Write-Warning "$assetsSrc not found - some card art may be missing from the package."
    }
}
$count = (Get-ChildItem $assetsDst -Filter "*.png" -ErrorAction SilentlyContinue).Count
Write-Host "  + assets/ ($count PNGs)" -ForegroundColor Green

$zipPath = Join-Path $staging $zipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

$items = Get-ChildItem $stageRoot
Compress-Archive -Path ($items | ForEach-Object { $_.FullName }) -DestinationPath $zipPath -Force

Write-Host ""
Write-Host "Package ready: $zipPath" -ForegroundColor Cyan
Write-Host "Upload this ZIP to Thunderstore as Rounds2 / RoundsTheGathering" -ForegroundColor Cyan
