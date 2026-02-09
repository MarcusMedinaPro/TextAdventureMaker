param(
    [string]$Configuration = "Release",
    [string]$ArtifactsDir = "$(Join-Path $PSScriptRoot '..' 'artifacts')"
)

$ErrorActionPreference = "Stop"

if (!(Test-Path $ArtifactsDir)) {
    New-Item -Path $ArtifactsDir -ItemType Directory | Out-Null
}

dotnet pack (Join-Path $PSScriptRoot "..\src\MarcusMedina.TextAdventure\MarcusMedina.TextAdventure.csproj") -c $Configuration -o $ArtifactsDir
dotnet pack (Join-Path $PSScriptRoot "..\src\MarcusMedina.TextAdventure.AI\MarcusMedina.TextAdventure.AI.csproj") -c $Configuration -o $ArtifactsDir

if ($env:NUGET_API_KEY) {
    dotnet nuget push (Join-Path $ArtifactsDir "*.nupkg") --api-key $env:NUGET_API_KEY --source "https://api.nuget.org/v3/index.json" --skip-duplicate
} else {
    Write-Host "NUGET_API_KEY not set; packages packed to $ArtifactsDir."
}
