param(
    [string]$GameDir = $env:DAVE_THE_DIVER_DIR
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($GameDir)) {
    throw "Pass -GameDir or set DAVE_THE_DIVER_DIR to the game installation directory."
}
$source = Join-Path $PSScriptRoot 'artifacts\BeetleBattlePredictor\BepInEx\plugins\BeetleBattlePredictor\BeetleBattlePredictor.dll'
$targetDir = Join-Path $GameDir 'BepInEx\plugins\BeetleBattlePredictor'

if (-not (Test-Path -LiteralPath (Join-Path $GameDir 'BepInEx\core\BepInEx.Unity.IL2CPP.dll'))) {
    throw 'BepInEx 6 IL2CPP was not found. Install it and launch the game once first.'
}
if (-not (Test-Path -LiteralPath $source)) {
    throw 'Built DLL was not found. Run .\build.ps1 first.'
}

New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
Copy-Item -LiteralPath $source -Destination $targetDir -Force
Write-Host "Installed to: $targetDir"
