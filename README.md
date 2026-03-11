# Regent With A Steel Pipe

A Slay the Spire 2 joke mod that turns Regent's forge/refine/Sovereign Blade into steel-pipe nonsense.

## What It Changes

- Replaces Regent forge audio with `steel_pipe.ogg`
- Replaces Regent refine audio with `steel_pipe.ogg`
- Replaces Sovereign Blade attack audio with `steel_pipe.ogg`
- Replaces the Sovereign Blade card portrait
- Replaces the in-combat orbiting Sovereign Blade visual with a steel pipe

## Requirements

- Slay the Spire 2 installed locally
- .NET 9 SDK
- Python 3
- Pillow for the blade image preprocessing step

Install Pillow once:

```powershell
python -m pip install pillow
```

## Configure The Game Path

The project can build in either of these setups:

1. The repo lives inside your Slay the Spire 2 install directory
2. The repo lives anywhere else, and you point it at the game

If the repo is outside the game directory, set `STS2_GAME_DIR`:

```powershell
$env:STS2_GAME_DIR = 'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2'
```

You can also pass the path directly:

```powershell
.\build.ps1 -GameRoot 'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2'
```

## Build

From the repository root:

```powershell
.\build.ps1
```

This will:

- preprocess the blade image
- build the DLL
- build the PCK
- copy both outputs into `mods\RegentWithASteelPipe` under your game install

## Repo Contents

- `src/`: Harmony patches and runtime replacement logic
- `pack/`: packaged mod assets
- `prepare_assets.py`: converts the blade source image into the runtime blade texture

## Publishing

Do not upload game files.

Safe to upload:

- this project folder

Do not upload:

- `bin/`
- `obj/`
- the full game directory
- anything under `data_sts2_windows_x86_64/`
- any copied game binaries
