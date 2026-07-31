[CmdletBinding()]
param(
    [string]$GameDir,
    [string]$Repository = 'anmuxixixi/DaveTheDiver_Mod',
    [string]$PackagePath,
    [switch]$SkipBepInEx
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$bepInExUrl = 'https://builds.bepinex.dev/projects/bepinex_be/785/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.785%2B6abdba4.zip'
$userAgent = 'DaveTheDiver-Mod-Installer'

function Test-GameDirectory {
    param([string]$Path)

    return -not [string]::IsNullOrWhiteSpace($Path) -and
        (Test-Path -LiteralPath (Join-Path $Path 'DaveTheDiver.exe'))
}

function Get-SteamRoots {
    $roots = [System.Collections.Generic.List[string]]::new()
    $registryLocations = @(
        @{ Path = 'HKCU:\Software\Valve\Steam'; Name = 'SteamPath' },
        @{ Path = 'HKLM:\SOFTWARE\WOW6432Node\Valve\Steam'; Name = 'InstallPath' }
    )

    foreach ($location in $registryLocations) {
        try {
            $value = (Get-ItemProperty -LiteralPath $location.Path -Name $location.Name -ErrorAction Stop).($location.Name)
            if (-not [string]::IsNullOrWhiteSpace($value)) {
                $null = $roots.Add($value)
            }
        }
        catch {
            # Steam may be installed only for the other registry scope.
        }
    }

    foreach ($root in @($roots)) {
        $libraryFile = Join-Path $root 'steamapps\libraryfolders.vdf'
        if (-not (Test-Path -LiteralPath $libraryFile)) {
            continue
        }

        $content = Get-Content -LiteralPath $libraryFile -Raw
        foreach ($match in [regex]::Matches($content, '"path"\s+"(?<path>[^"]+)"')) {
            $libraryRoot = $match.Groups['path'].Value -replace '\\\\', '\'
            if (-not [string]::IsNullOrWhiteSpace($libraryRoot)) {
                $null = $roots.Add($libraryRoot)
            }
        }
    }

    return @($roots) | Select-Object -Unique
}

function Find-GameDirectory {
    foreach ($steamRoot in Get-SteamRoots) {
        $candidate = Join-Path $steamRoot 'steamapps\common\Dave the Diver'
        if (Test-GameDirectory $candidate) {
            return (Resolve-Path -LiteralPath $candidate).Path
        }
    }

    throw 'Dave the Diver was not found automatically. Pass -GameDir with the directory containing DaveTheDiver.exe.'
}

function Invoke-Download {
    param(
        [string]$Uri,
        [string]$Destination
    )

    Write-Host "Downloading: $Uri"
    Invoke-WebRequest -UseBasicParsing -Headers @{ 'User-Agent' = $userAgent } -Uri $Uri -OutFile $Destination
}

function Get-LatestModPackage {
    param([string]$Destination)

    $apiUrl = "https://api.github.com/repos/$Repository/contents/artifacts?ref=main"
    $items = Invoke-RestMethod -Headers @{ 'User-Agent' = $userAgent; 'Accept' = 'application/vnd.github+json' } -Uri $apiUrl
    $packages = foreach ($item in $items) {
        if ($item.name -match '^BeetleBattlePredictor-v(?<version>\d+\.\d+\.\d+)\.zip$') {
            [PSCustomObject]@{
                Version = [version]$Matches['version']
                Url = $item.download_url
                Name = $item.name
            }
        }
    }

    $latest = $packages | Sort-Object Version -Descending | Select-Object -First 1
    if ($null -eq $latest -or [string]::IsNullOrWhiteSpace($latest.Url)) {
        throw "No installable package was found in the artifacts directory of $Repository."
    }

    Write-Host "Latest version: $($latest.Version)"
    Invoke-Download -Uri $latest.Url -Destination $Destination
}

if ([string]::IsNullOrWhiteSpace($GameDir)) {
    $GameDir = Find-GameDirectory
}
elseif (-not (Test-GameDirectory $GameDir)) {
    throw "The game directory is invalid or DaveTheDiver.exe is missing: $GameDir"
}
else {
    $GameDir = (Resolve-Path -LiteralPath $GameDir).Path
}

if (Get-Process -Name 'DaveTheDiver' -ErrorAction SilentlyContinue) {
    throw 'Dave the Diver is running. Exit the game and run the installer again.'
}

Write-Host "Game directory: $GameDir"
$tempRoot = Join-Path ([IO.Path]::GetTempPath()) ("DaveTheDiver-ModInstaller-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    $bepInExCore = Join-Path $GameDir 'BepInEx\core\BepInEx.Unity.IL2CPP.dll'
    if (-not (Test-Path -LiteralPath $bepInExCore)) {
        if ($SkipBepInEx) {
            throw 'BepInEx 6 IL2CPP is missing and -SkipBepInEx was specified.'
        }

        Write-Host 'BepInEx 6 IL2CPP was not found. Installing the verified be.785 build.'
        $bepInExZip = Join-Path $tempRoot 'BepInEx.zip'
        $bepInExExtract = Join-Path $tempRoot 'BepInEx'
        Invoke-Download -Uri $bepInExUrl -Destination $bepInExZip
        Expand-Archive -LiteralPath $bepInExZip -DestinationPath $bepInExExtract -Force

        $downloadedCore = Join-Path $bepInExExtract 'BepInEx\core\BepInEx.Unity.IL2CPP.dll'
        if (-not (Test-Path -LiteralPath $downloadedCore)) {
            throw 'The downloaded BepInEx archive has an unexpected layout. Installation stopped.'
        }

        Copy-Item -Path (Join-Path $bepInExExtract '*') -Destination $GameDir -Recurse -Force
    }
    else {
        Write-Host 'BepInEx 6 IL2CPP is already installed.'
    }

    $modZip = Join-Path $tempRoot 'BeetleBattlePredictor.zip'
    if ([string]::IsNullOrWhiteSpace($PackagePath)) {
        Get-LatestModPackage -Destination $modZip
    }
    else {
        $resolvedPackage = (Resolve-Path -LiteralPath $PackagePath).Path
        Copy-Item -LiteralPath $resolvedPackage -Destination $modZip -Force
        Write-Host "Using local package: $resolvedPackage"
    }

    $modExtract = Join-Path $tempRoot 'Mod'
    Expand-Archive -LiteralPath $modZip -DestinationPath $modExtract -Force
    $sourceDll = Join-Path $modExtract 'BepInEx\plugins\BeetleBattlePredictor\BeetleBattlePredictor.dll'
    if (-not (Test-Path -LiteralPath $sourceDll)) {
        throw 'The mod archive has an unexpected layout: BeetleBattlePredictor.dll is missing.'
    }

    $targetDir = Join-Path $GameDir 'BepInEx\plugins\BeetleBattlePredictor'
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    $targetDll = Join-Path $targetDir 'BeetleBattlePredictor.dll'
    Copy-Item -LiteralPath $sourceDll -Destination $targetDll -Force

    $version = (Get-Item -LiteralPath $targetDll).VersionInfo.FileVersion
    $sha256 = (Get-FileHash -LiteralPath $targetDll -Algorithm SHA256).Hash
    Write-Host ''
    Write-Host 'Installation completed successfully.' -ForegroundColor Green
    Write-Host "Mod version: $version"
    Write-Host "DLL SHA256: $sha256"
    Write-Host 'The first launch after installing BepInEx may take longer while IL2CPP interop files are generated. Press F8 during a beetle battle to enable prediction.'
}
finally {
    if (Test-Path -LiteralPath $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
