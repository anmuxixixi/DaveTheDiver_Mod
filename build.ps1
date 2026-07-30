param(
    [string]$GameDir = $env:DAVE_THE_DIVER_DIR,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($GameDir)) {
    throw "Pass -GameDir or set DAVE_THE_DIVER_DIR to the game installation directory."
}
$project = Join-Path $PSScriptRoot 'src\BeetleBattlePredictor\BeetleBattlePredictor.csproj'
$tests = Join-Path $PSScriptRoot 'tests\BeetleBattlePredictor.Tests\BeetleBattlePredictor.Tests.csproj'
$artifactDir = Join-Path $PSScriptRoot 'artifacts\BeetleBattlePredictor\BepInEx\plugins\BeetleBattlePredictor'

if (-not $SkipTests) {
    dotnet test $tests --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "Tests failed with exit code $LASTEXITCODE." }
}

dotnet build $project --configuration Release -p:GameDir="$GameDir"
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE." }
New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'src\BeetleBattlePredictor\bin\Release\net6.0\BeetleBattlePredictor.dll') -Destination $artifactDir -Force

$zip = Join-Path $PSScriptRoot 'artifacts\BeetleBattlePredictor-v1.0.7.zip'
Compress-Archive -Path (Join-Path $PSScriptRoot 'artifacts\BeetleBattlePredictor\*') -DestinationPath $zip -Force
Write-Host "Built: $zip"
