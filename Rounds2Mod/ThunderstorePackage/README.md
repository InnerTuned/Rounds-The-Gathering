# Rounds: The Gathering

One install, three mods — a complete pick-phase overhaul for ROUNDS.

---

## What's included

### DeckBuilder Mod
- Build and save custom card decks (up to 50 cards)
- Draft only from your active deck during picks
- Deck HUD showing remaining cards
- **Delete a card from your hand** during picks instead of drafting
- Tiered card unlock API for other mods

### ShieldsMod
- Persistent shield bubble (unlocked via **Upgrade Shield Health I**)
- Shield health upgrades (5 tiers, multiplicative)
- Poison / Stun / Lifesteal resistance cards (3 tiers each)

### Info Overhaul
- **Show Stats** button during picks — full stat popup
- **Card delta preview** — see how each draft card changes your build before you pick

---

## Requirements

Installed automatically by r2modman:
- BepInEx
- UnboundLib
- ModdingUtils

---

## Quick start

1. Install this package via Thunderstore / r2modman.
2. Open the main menu **Deck Manager** (replaces Toggle Cards).
3. Create a deck, set it as your active deck, and start a match.
4. During picks, use **Show Stats** (bottom-left) and hover draft cards to see stat deltas.

---

## Save location

Decks are saved to:
```
BepInEx/config/RoundsTheGathering/decks.json
```

---

## Building locally

See [How to run.md](../How%20to%20run.md) for the full developer guide (GitHub setup, Libs, publishing).

From `Rounds2Mod/`:

```powershell
dotnet build ModPack.csproj -c Release
```

This builds all three DLLs and deploys them to your r2modman profile folder (`BepInEx/plugins/Rounds2-RoundsTheGathering/`).

For a Thunderstore ZIP:

```powershell
.\build_package.ps1
```

---

## Package contents

| File | Role |
|------|------|
| `DeckBuilder.dll` | Deck builder, card deletion, prerequisites |
| `ShieldsMod.dll` | Shields and resistance cards |
| `InfoOverhaul.dll` | Stats popup and delta preview |
| `assets/` | ShieldsMod card artwork (PNG) |
