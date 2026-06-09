# DeckBuilder Mod

A deck-building layer for ROUNDS inspired by trading card games. Ships inside the **Rounds: The Gathering** Thunderstore package as `DeckBuilder.dll`. Build custom card pools before a match, set one as your active deck, and only draft from it during pick phases — no more rolling the dice on the full card library every time.

---

## Features

### Deck Builder
Create and manage named decks from any cards in the game. Each deck is:
- Up to **50 cards** (configurable)
- Persisted to disk between sessions (`BepInEx/config/RoundsTheGathering/decks.json`)
- Selectable per-match from a dedicated in-game screen

Deck limits per card (via [RarityLib](https://thunderstore.io/c/rounds/p/Root/RarityLib/) / [RarityBundle](https://thunderstore.io/c/rounds/p/CrazyCoders/RarityBundle/) rarity names):
- **Trinket / Common / Scarce:** max 3 copies
- **Uncommon / Rare / Exotic:** max 2 copies
- **Epic and above** (Epic, Legendary, Mythical, Divine, Unique, etc.): max 1 copy

### Deck Selector Screen
Replaces the vanilla Toggle Cards menu with a full deck management UI:
- **Create New Deck** — opens the deck editor with a blank slate
- **Edit Deck** — modify an existing deck's card list
- **Set Playable Deck** — activates the selected deck for the next match
- **Delete Deck** — removes a deck (with confirmation; the Default Deck cannot be deleted)
- Active deck name is always shown at the bottom of the panel

### Pick-Phase Integration
During card picks, the active deck becomes your draw pool. Cards are drawn randomly from your deck and shrink as you pick — just like a real deck. The remaining count displays in a HUD at the bottom of the screen.

When all cards in your deck have been picked, the pool resets and you draw from the full deck again.

If no custom deck is set (or a bot is picking), the full vanilla card pool is used as normal.

### Card Deletion During Picks
On your pick turn, you can **delete a card from your hand** instead of drafting a new one. Click any card in your card bar while the draft is open — it highlights red on hover.

A **confirmation modal** appears before anything is removed:

```
Are you sure you want to delete Big Mag?

DMG: 82 --> 55
Ammo: 5 --> 3

[No]  [Yes]
```

- The modal is **fully opaque** — you cannot interact with the pick screen behind it.
- **No** closes the modal and you continue picking normally.
- **Yes** deletes the card for the rest of the match.

If **Info Overhaul** is installed, the modal shows the stat changes that deletion will cause (same green/red formatting as the draft-card delta preview). Cards without a registered preview show **N/A**.

Deletion is networked (all clients stay in sync), ends your pick turn, and clears the draft choices on screen. Other mods can hook `CardDeleteManager.URPC_SyncDelete` to recalculate stats after a card is removed.

### Deck HUD
A persistent overlay during pick phases shows:

```
Shields_Test — 20 cards
```

Red text when 5 or fewer cards remain.

### Card Unlock Prerequisites (API for modders)
DeckBuilder exposes a public API for other mods to declare tiered card unlock chains:

```csharp
CardPrerequisiteRegistry.RegisterPrerequisite("Upgrade Shield Health II", "Upgrade Shield Health I");
```

A card with a prerequisite only appears in draft picks once the player already owns the required card — evaluated per-player, per-pick, from their current hand. No fragile global enable/disable required.

Other mods can register prerequisites via direct reference or via reflection (see the Modder API section below).

---

## How to Use

1. Open the main menu and look for the **Deck Manager** button (where Toggle Cards used to be).
2. Create a deck, add the cards you want, give it a name.
3. Use **Set Playable Deck** to make it active.
4. Start a match — you will only draft from that deck.

To go back to vanilla drafting, set the **Default Deck** as active.

---

## Mod Compatibility

| Mod | Status | Notes |
|-----|--------|-------|
| **UnboundLib** | Required | Card registration and menu integration |
| **ModdingUtils** | Required | Card framework |
| **RarityLib** | Required | Custom rarity framework (extends the `CardInfo.Rarity` enum) — [Thunderstore](https://thunderstore.io/c/rounds/p/Root/RarityLib/) / [source](https://github.com/Tess-y/RarityLib) |
| **RarityBundle** | Required | Standard modded rarities (Trinket, Scarce, Exotic, Epic, Mythical, Divine, Unique, etc.) — [Thunderstore](https://thunderstore.io/c/rounds/p/CrazyCoders/RarityBundle/) / [source](https://github.com/willuwontu/Rarity-Bundle) |
| **ShieldsMod** | Optional | Rebuilds shield/resistance stats after card deletion |
| **Info Overhaul** | Optional | Shows remaining deck count, draft-card delta previews, and delete-confirmation stat previews during picks |

---

## Modder API

### Registering Card Prerequisites

From another mod, register a prerequisite chain so that a card only appears in picks once the player already owns an earlier tier:

**Direct reference (if you have a compile dependency on DeckBuilder):**
```csharp
CardPrerequisiteRegistry.RegisterPrerequisite("My Card Tier II", "My Card Tier I");
```

**Via reflection (soft dependency — your mod still loads if RTG isn't installed):**
```csharp
Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.GetName().Name == "DeckBuilder");

Type registry = asm?.GetType("DeckBuilder.CardPrerequisiteRegistry");
MethodInfo method = registry?.GetMethod("RegisterPrerequisite",
    BindingFlags.Public | BindingFlags.Static);

method?.Invoke(null, new object[] { "My Card II", "My Card I" });
```

Prerequisites are evaluated from the player's live hand each pick — so if a player acquires tier I mid-match, tier II immediately becomes available on their next turn.

---

## Save File Location

Decks are saved to:
```
%AppData%\r2modmanPlus-local\ROUNDS\profiles\<Profile>\BepInEx\config\RoundsTheGathering\decks.json
```
