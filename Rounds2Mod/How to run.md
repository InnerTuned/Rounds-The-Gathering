# How to Run — Rounds: The Gathering (dev guide)

This repo contains one **Thunderstore modpack** made of three BepInEx plugins:

| Source folder | DLL | What it does |
|---------------|-----|----------------|
| `DeckBuilder/` | `DeckBuilder.dll` | Deck builder, card deletion, tier prerequisites |
| `ShieldsMod/` | `ShieldsMod.dll` | Shields, resistance cards, card art (`assets/`) |
| `InfographicsOverhaul/` | `InfoOverhaul.dll` | Pick-phase stats popup and card delta preview |

Users install a single Thunderstore package named **RoundsTheGathering** (display name: *Rounds: The Gathering*).

---

## Prerequisites

1. **ROUNDS** (Steam)
2. **[r2modman](https://thunderstore.io/c/rounds/p/RoundsModding/)** (or Thunderstore Mod Manager)
3. **[.NET SDK](https://dotnet.microsoft.com/download)** (6.0 or later — builds target .NET Framework 4.7.1)

---

## First-time setup

### 1. r2modman profile

Create a ROUNDS profile in r2modman and install these mods before testing your local build:

- BepInEx Pack
- UnboundLib
- ModdingUtils

Our modpack adds on top of these; it does not replace them.

### 2. Copy the proprietary game DLLs into `DeckBuilder/Libs/`

Most reference DLLs (0Harmony, UnboundLib, ModdingUtils) are **already committed to the repo** and require no extra steps.

You only need to manually copy the following because they are proprietary and cannot be distributed in source control:

**From your ROUNDS install** (`Steam/steamapps/common/Rounds/Rounds_Data/Managed/`):

| File | Where to find it |
|------|-----------------|
| `Assembly-CSharp.dll` | Game managed folder |
| `MMHOOK_Assembly-CSharp.dll` | r2modman profile `BepInEx/plugins/` (from a hook pack) |
| `UnityEngine.dll` | Game managed folder |
| `UnityEngine.CoreModule.dll` | Game managed folder |
| `UnityEngine.UI.dll` | Game managed folder |
| `Unity.TextMeshPro.dll` | Game managed folder |
| `Photon3Unity3D.dll` | Game managed folder |
| `PhotonRealtime.dll` | Game managed folder |
| `PhotonUnityNetworking.dll` | Game managed folder |

If a build complains about a missing reference, search your `BepInEx` folder for that filename — it is usually in the profile's `BepInEx/core/` or `BepInEx/plugins/` subfolders.

---

## Configure your local deploy path

1. Copy [`Directory.Build.user.props.example`](Directory.Build.user.props.example) → `Directory.Build.user.props` (same folder).
2. Open your new `Directory.Build.user.props` and replace the placeholder with your actual r2modman plugin path:

```
%AppData%\r2modmanPlus-local\ROUNDS\profiles\<YourProfile>\BepInEx\plugins\Rounds2-RoundsTheGathering
```

The file is gitignored, so each developer keeps their own without affecting others.  
The `Rounds2-RoundsTheGathering` folder will be created automatically on first build.

---

## Build and run locally

From the `Rounds2Mod/` directory:

```powershell
dotnet build ModPack.csproj -c Release
```

This will:

1. Build `DeckBuilder.dll`, `ShieldsMod.dll`, and `InfoOverhaul.dll`
2. Copy all three plus `ShieldsMod/assets/*.png` into your `RtgPluginDir`

Then launch ROUNDS through r2modman with that profile selected.

### Solution file

You can also open `Rounds2Mod.sln` in Visual Studio or Rider and build the **ModPack** project (or the whole solution).

### Iterating on a single mod

Building an individual `.csproj` still works, but it **does not** deploy to the game. Always run `dotnet build ModPack.csproj` when you want to test in ROUNDS.

---

## Publishing to Thunderstore

### 1. Bump the version

Edit [`ThunderstorePackage/manifest.json`](ThunderstorePackage/manifest.json) — update `version_number` (semver: `Major.Minor.Patch`).

### 2. Build the ZIP

```powershell
.\build_package.ps1
```

Output: `Release_Build/RoundsTheGathering_<version>.zip`

The ZIP root must contain (flat, no nested folders):

```
manifest.json
README.md
icon.png
DeckBuilder.dll
ShieldsMod.dll
InfoOverhaul.dll
assets/
  lv1_artwork.png
  ...
```

### 3. Upload

1. Go to [Thunderstore — ROUNDS](https://thunderstore.io/c/rounds/create/)
2. Sign in as team **Rounds2**
3. Upload the ZIP for package **RoundsTheGathering**
4. Thunderstore will install BepInEx / UnboundLib / ModdingUtils automatically from `manifest.json` dependencies

### 4. Verify on a clean profile

Create a fresh r2modman profile, install only **Rounds2 — RoundsTheGathering**, then confirm:

- Deck Manager appears in the main menu
- Shield card art renders (not blank)
- Show Stats button works during picks

---

## Repo layout (quick reference)

```
Rounds2Mod/
  DeckBuilder/           # Deck builder + card delete source (Libs/ included except game DLLs)
  ShieldsMod/            # Shields + resistance cards + assets/
  InfographicsOverhaul/  # Stats / delta preview source
  ThunderstorePackage/   # manifest.json, README.md, icon.png
  ModPack.csproj         # Build all + deploy locally
  build_package.ps1      # Build all + create Thunderstore ZIP
  Directory.Build.props            # Shared build settings
  Directory.Build.user.props       # Your local deploy path (gitignored — copy from .example)
  Directory.Build.user.props.example  # Template for the above
  .gitignore             # Excludes build output and proprietary DLLs
  How to run.md          # This file
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Build fails: missing DLL | Copy the file into `DeckBuilder/Libs/` (see the table above) |
| Mods not loading in game | Check `BepInEx/LogOutput.log`; ensure all three DLLs are in `plugins/Rounds2-RoundsTheGathering/` |
| Card art is blank | Confirm `assets/` sits **next to** `ShieldsMod.dll` in the plugin folder |
| Wrong profile gets DLLs | Update `RtgPluginDir` in your `Directory.Build.user.props` |
| Old decks missing | Deck saves live at `BepInEx/config/RoundsTheGathering/decks.json` (unchanged path) |
