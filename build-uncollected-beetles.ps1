param(
    [string]$Configuration = 'Release',
    [string]$GameDir = '',
    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'src\UncollectedBeetleSpawner\UncollectedBeetleSpawner.csproj'
$tests = Join-Path $PSScriptRoot 'tests\UncollectedBeetleSpawner.Tests\UncollectedBeetleSpawner.Tests.csproj'
$artifactRoot = Join-Path $PSScriptRoot 'artifacts\UncollectedBeetleSpawner'
$pluginDir = Join-Path $artifactRoot 'BepInEx\plugins\UncollectedBeetleSpawner'
$zip = Join-Path $PSScriptRoot 'artifacts\UncollectedBeetleSpawner-v1.0.10.zip'

if (-not $NoRestore) {
    dotnet restore $tests --configfile (Join-Path $PSScriptRoot 'NuGet.Config')
    if ($LASTEXITCODE -ne 0) { throw 'Test restore failed.' }
}
dotnet test $tests -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }

$buildArgs = @('build', $project, '-c', $Configuration)
if ($GameDir) {
    $buildArgs += "-p:GameDir=$GameDir"
}
if (-not $NoRestore) {
    dotnet restore $project --configfile (Join-Path $PSScriptRoot 'NuGet.Config')
    if ($LASTEXITCODE -ne 0) { throw 'Mod restore failed.' }
}
$buildArgs += '--no-restore'
dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Mod build failed.' }

New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "src\UncollectedBeetleSpawner\bin\$Configuration\net6.0\UncollectedBeetleSpawner.dll") -Destination $pluginDir -Force
Compress-Archive -Path (Join-Path $artifactRoot '*') -DestinationPath $zip -Force
Write-Host "Created $zip"
