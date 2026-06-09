# Info Overhaul

Adds a pick-phase stats panel and a card delta preview bar to ROUNDS. See your current stats at a glance during any card pick, and instantly know how each draft option will change your build before you commit.

---

## Features

### Show Stats Button
A **Show Stats** button appears in the bottom-left corner during card picks — for both players' turns, not just yours.

Clicking it opens a fully opaque dual-column popup showing **your** stats (regardless of whose turn it is):

| Column 1 | Column 2 |
|----------|----------|
| HP | DMG |
| Lives | Knockback |
| Block CD | Life Steal |
| Block Count | Damage Grow |
| Shield Health* | Bullet Slow |
| Poison Resistance* | Move SPD |
| … | … |

All stats from Infoholic are included. Shield Health and Poison Resistance appear automatically if **ShieldsMod** is also installed.

The popup refreshes live while open. It closes automatically when the pick sequence ends (before the battle starts).

### Card Delta Preview Bar
A thin panel sits **above the deck HUD** at the bottom of the screen during picks. As you scroll through draft cards, it shows how your stats will change if you pick the highlighted card:

```
If Big Mag:
DMG: 55 --> 82     Ammo: 3 --> 5
```

- After-values are **green** when the stat increases, **red** when it decreases
- Up to **3 stat changes per column**; extra stats overflow into additional columns to the right (supports up to 15 lines)
- Cards with no registered delta show **N/A**
- Effect notes (custom mechanics that can't be expressed numerically) appear as plain text lines

---

## Card Delta Coverage

### Automatic (Tier 1)
Any card — vanilla or from a mod — that sets its stats in the standard `SetupCard()` method is picked up and simulated automatically when the game loads. This covers the majority of all cards in the base game and most card packs on Thunderstore.

### Explicit (Tier 1 opt-in)
Cards with custom `OnAddCard()` logic register their own deltas. ShieldsMod's shield upgrade and poison resistance cards are fully supported out of the box.

### Effect Notes (Tier 2)
Cards that change stats **and** have a special mechanic can register an effect note that appends below the numeric lines:

```
If Thunder Core:
DMG: 55 --> 70
+ Arc lightning on hit
```

### Unregistered
Cards that do everything in `OnAddCard()` with no standard stat template show **N/A**. This is intentional — a missing preview is always more honest than a wrong one.

---

## Mod Compatibility

| Mod | Status | Notes |
|-----|--------|-------|
| **UnboundLib** | Required | |
| **ModdingUtils** | Required | |
| **DeckBuilder** | Optional | Deck name and remaining card count shown in the HUD; pick-phase lifecycle hooks used for show/hide timing |
| **ShieldsMod** | Optional | Shield Health and Poison Resistance stats appear in the stats popup and pick delta preview |
| **MFM** | Automatic | Most MFM stat cards (Calibrate: *, Ammo +N, etc.) are auto-detected as Tier 1 |

---

## Modder API

Info Overhaul exposes a public API so any mod can register delta previews for its own cards.

### Simple registration (compile reference)

```csharp
using InfoOverhaul.Delta;

// Numeric before/after
CardDeltaRegistry.RegisterSimple("My Card", (player, card) => new[]
{
    ("DMG", "55", "82"),
    ("Ammo", "3", "5"),
});

// Effect note only
CardDeltaTier2.RegisterEffectNotes("My Fancy Card", "+ Explodes on hit");

// Numeric stats + effect note (Tier 2 addon — merges onto top of auto stats)
CardDeltaTier2.RegisterEffectNotes("Thunder Core", "+ Arc lightning on hit");

// Override auto-detected stats if they're wrong for a specific card
CardDeltaTier2.RegisterOverrideSimple("My Card", (player, card) => new[]
{
    ("Custom Stat", "10", "20"),
});
```

### Via reflection (no compile dependency)

```csharp
var asm = AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.GetName().Name == "InfoOverhaul");

var registry = asm?.GetType("InfoOverhaul.Delta.CardDeltaRegistry");
var registerSimple = registry?.GetMethod("RegisterSimple",
    BindingFlags.Public | BindingFlags.Static);

// The SimpleDeltaProvider delegate type lives on CardDeltaRegistry
var delegateType = registry?.GetNestedTypes(BindingFlags.Public)
    .FirstOrDefault(t => t.Name == "SimpleDeltaProvider");

Func<Player, CardInfo, IReadOnlyList<(string, string, string)>> myProvider = (player, card) =>
    new[] { ("My Stat", "10", "20") };

var del = Delegate.CreateDelegate(delegateType, myProvider.Target, myProvider.Method);
registerSimple?.Invoke(null, new object[] { "My Card Name", del });
```

Registrations can be made at any time before or after the main menu first loads — the auto-scan runs once at first main menu open, but manual registrations are always respected first.

---

## Credits

Stat formulas adapted from [Infoholic](https://github.com/PenialJackson/Infoholic) by PenialJackson.
