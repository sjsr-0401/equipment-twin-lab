Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$projectPath = Join-Path $repoRoot "src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj"

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "WPF HMI project was not found: $projectPath"
}

dotnet run --project $projectPath --configuration Debug
