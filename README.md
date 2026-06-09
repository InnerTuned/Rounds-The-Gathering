# Rounds: The Gathering

One install, four mods — a complete pick-phase overhaul for ROUNDS.

---

## What's included

### DeckBuilder Mod
- Build and save custom card decks (you can configure the max number of cards, per deck)
- Draft only from your active deck during picks
- Deck HUD showing remaining cards
- **Deck Manager UI** with pagination, search, and a "My Deck" view
- **Swap card** to delete a card from your hand during picks — with a confirmation modal and stat-change preview
- **Copycat card** to duplicate any card from any player's hand
- Tiered card unlock API for other mods

### Keybound Mod
- Utility cards with **[Keybound]** effects
- Bind a key during the draft, then activate the effect from the **Effect Stack** (bottom-right)
- **Teleport** card: teleport to your cursor on a cooldown

### ShieldsMod
- Persistent shield bubble (unlocked via **Upgrade Shield Health I**)
- Shield health upgrades (5 tiers, multiplicative)
- Poison / Stun / Lifesteal resistance cards (3 tiers each)

### Info Overhaul
- **Show Stats** button during picks — full stat popup (shield health, poison/stun/lifesteal resistance, and more)
- **Card delta preview** — see how each draft card changes your build before you pick
- **Delete preview** — when confirming a hand-card deletion, shows the stat changes that removal will cause

---

## Requirements

Installed automatically by r2modman:
- BepInEx
- UnboundLib
- ModdingUtils
- [RarityLib](https://thunderstore.io/c/rounds/p/Root/RarityLib/) (custom rarity framework — required by DeckBuilder)
- [RarityBundle](https://thunderstore.io/c/rounds/p/CrazyCoders/RarityBundle/) (Trinket, Scarce, Epic, Mythical, etc. — required by DeckBuilder)

---

## Quick start

1. Install this package via Thunderstore / r2modman.
2. Open the main menu **Deck Manager** (replaces Toggle Cards).
3. Create a deck, set it as your active deck, and start a match.
4. During picks, use **Show Stats** (bottom-left) and hover draft cards to see stat deltas.
5. To trim your build, click a card in your hand — a confirmation modal shows the stat impact before you delete.

---

## Save location

Decks are saved to:
```
BepInEx/config/RoundsTheGathering/decks.json
```

---

## Building locally

See How to run.md for the full developer guide (GitHub setup, Libs, publishing).

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
| `DeckBuilder.dll` | Deck builder, custom cards (Swap/Copycat), prerequisites |
| `Keybound.dll` | Keybound effects, Effect Stack UI, Teleport card |
| `ShieldsMod.dll` | Shields and resistance cards |
| `InfoOverhaul.dll` | Stats popup, draft delta preview, delete stat preview |
| `assets/` | Mod card artwork (PNG) |
