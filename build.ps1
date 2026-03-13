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
$projectFile = Join-Path $projectRoot "RegentWithASteelPipe.csproj"
$manifestTemplate = Join-Path $projectRoot "codex.regent_with_a_steel_pipe.json"
$packProjectDir = Join-Path $projectRoot "pack"
$outputDir = Join-Path $gameRoot "mods\\RegentWithASteelPipe"
$legacyOutputDir = Join-Path $gameRoot "mods\\RegentPipeSfx"
$outputDll = Join-Path $outputDir "codex.regent_with_a_steel_pipe.dll"
$outputPck = Join-Path $outputDir "codex.regent_with_a_steel_pipe.pck"
$outputManifest = Join-Path $outputDir "codex.regent_with_a_steel_pipe.json"
$legacyNamedDll = Join-Path $outputDir "RegentWithASteelPipe.dll"
$legacyNamedPck = Join-Path $outputDir "RegentWithASteelPipe.pck"
$tempPck = Join-Path $projectRoot "bin\\Release\\codex.regent_with_a_steel_pipe.pck"
$godotExe = Join-Path $gameRoot "SlayTheSpire2.exe"
$relativePackScript = "pack_mod.gd"
$prepareScript = Join-Path $projectRoot "prepare_assets.py"

python $prepareScript $projectRoot | Out-Host

dotnet build $projectFile -c Release -p:Sts2GameDir="$gameRoot" | Out-Host

$builtDll = Join-Path $projectRoot "bin\\Release\\RegentWithASteelPipe.dll"
if (-not (Test-Path $builtDll)) {
    throw "Build output not found: $builtDll"
}

if ((Test-Path $legacyOutputDir) -and ($legacyOutputDir -ne $outputDir)) {
    Remove-Item $legacyOutputDir -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
Copy-Item $builtDll $outputDll -Force
Copy-Item $manifestTemplate $outputManifest -Force
foreach ($legacyPath in @($legacyNamedDll, $legacyNamedPck)) {
    if (Test-Path $legacyPath) {
        Remove-Item $legacyPath -Force
    }
}

if (Test-Path $tempPck) {
    Remove-Item $tempPck -Force
}

Push-Location $packProjectDir
try {
    & $godotExe --headless --nomods --path $packProjectDir --script $relativePackScript -- $packProjectDir $tempPck
    if ($LASTEXITCODE -ne 0) {
        throw "Godot pack command failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}

for ($i = 0; $i -lt 20 -and -not (Test-Path $tempPck); $i++) {
    Start-Sleep -Milliseconds 250
}

if (-not (Test-Path $tempPck)) {
    throw "PCK build failed: $tempPck was not created."
}

Copy-Item $tempPck $outputPck -Force

Write-Host "Built mod:"
Write-Host "  $outputDll"
Write-Host "  $outputPck"
Write-Host "  $outputManifest"
