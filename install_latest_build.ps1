param(
    [string]$GameRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-Sts2GameRoot {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectRoot,
        [string]$ExplicitGameRoot
    )

    $candidates = New-Object System.Collections.Generic.List[string]
    if ($ExplicitGameRoot) {
        $candidates.Add($ExplicitGameRoot)
    }
    if ($env:STS2_GAME_DIR) {
        $candidates.Add($env:STS2_GAME_DIR)
    }

    $candidates.Add((Split-Path -Parent (Split-Path -Parent $ProjectRoot)))

    foreach ($candidate in $candidates) {
        if (-not $candidate) {
            continue
        }

        $resolved = [System.IO.Path]::GetFullPath($candidate)
        $exePath = Join-Path $resolved "SlayTheSpire2.exe"
        $dllPath = Join-Path $resolved "data_sts2_windows_x86_64\\sts2.dll"
        if ((Test-Path $exePath) -and (Test-Path $dllPath)) {
            return $resolved
        }
    }

    throw "Could not locate the Slay the Spire 2 install root. Pass -GameRoot <path> or set STS2_GAME_DIR."
}

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$gameRoot = Resolve-Sts2GameRoot -ProjectRoot $projectRoot -ExplicitGameRoot $GameRoot
$builtDll = Join-Path $projectRoot "bin\\Release\\RegentWithASteelPipe.dll"
$builtPck = Join-Path $projectRoot "bin\\Release\\codex.regent_with_a_steel_pipe.pck"
$manifestTemplate = Join-Path $projectRoot "codex.regent_with_a_steel_pipe.json"
$targetDir = Join-Path $gameRoot "mods\\RegentWithASteelPipe"
$legacyTargetDir = Join-Path $gameRoot "mods\\RegentPipeSfx"
$targetDll = Join-Path $targetDir "codex.regent_with_a_steel_pipe.dll"
$targetPck = Join-Path $targetDir "codex.regent_with_a_steel_pipe.pck"
$targetManifest = Join-Path $targetDir "codex.regent_with_a_steel_pipe.json"
$legacyNamedDll = Join-Path $targetDir "RegentWithASteelPipe.dll"
$legacyNamedPck = Join-Path $targetDir "RegentWithASteelPipe.pck"

if (-not (Test-Path $builtDll)) {
    throw "Build output not found: $builtDll"
}

if (-not (Test-Path $builtPck)) {
    throw "Build output not found: $builtPck"
}

if ((Test-Path $legacyTargetDir) -and ($legacyTargetDir -ne $targetDir)) {
    Remove-Item $legacyTargetDir -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
Copy-Item $builtDll $targetDll -Force
Copy-Item $builtPck $targetPck -Force
Copy-Item $manifestTemplate $targetManifest -Force
foreach ($legacyPath in @($legacyNamedDll, $legacyNamedPck)) {
    if (Test-Path $legacyPath) {
        Remove-Item $legacyPath -Force
    }
}

Write-Host "Installed latest build:"
Write-Host "  $targetDll"
Write-Host "  $targetPck"
Write-Host "  $targetManifest"
