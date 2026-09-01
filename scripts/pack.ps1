param(
    [string]$Version,
    [switch]$Upload,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$csproj = Join-Path $root "src\Lumenhop\Lumenhop.csproj"
$publish = Join-Path $root "artifacts\publish"
$release = Join-Path $root "artifacts\release"
$icon = Join-Path $root "src\Lumenhop\Assets\Lumenhop.ico"
$notes = Join-Path $root "CHANGELOG.md"

if (-not $Version) {
    [xml]$proj = Get-Content $csproj
    $Version = $proj.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
}

if (-not $Version) {
    throw "Informe -Version ou defina <Version> no csproj."
}

New-Item -ItemType Directory -Force -Path $publish, $release | Out-Null

if (-not $SkipBuild) {
    if (Test-Path $publish) { Get-ChildItem $publish | Remove-Item -Recurse -Force }

    dotnet publish $csproj -c Release -r win-x64 -o $publish --self-contained true
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou." }

    $vpk = Get-Command vpk -ErrorAction SilentlyContinue
    if (-not $vpk) {
        throw "Instale o Velopack CLI: dotnet tool install -g vpk"
    }

    $packArgs = @(
        "pack",
        "--packId", "Lumenhop",
        "--packVersion", $Version,
        "--packDir", $publish,
        "--mainExe", "Lumenhop.exe",
        "--packTitle", "Lumenhop",
        "--icon", $icon,
        "--outputDir", $release,
        "--msi",
        "--instLocation", "Either"
    )
    if (Test-Path $notes) {
        $packArgs += @("--releaseNotes", $notes)
    }

    & vpk @packArgs
    if ($LASTEXITCODE -ne 0) { throw "vpk pack falhou." }
}

if (-not $Upload) {
    Write-Host "Pacote em $release"
    return
}

$repo = $env:LUMENHOP_REPO_URL
if (-not $repo) { $repo = "https://github.com/lbss9/lumenhop" }
$token = $env:GH_TOKEN
if (-not $token) { throw "Defina GH_TOKEN para enviar o Release." }

& vpk upload github `
    --repoUrl $repo `
    --publish `
    --releaseName "Lumenhop $Version" `
    --tag "v$Version" `
    --outputDir $release `
    --token $token

Write-Host "Release v$Version enviado."
