# ShieldsMod

Adds a shield bubble and a suite of upgradeable shield and resistance cards to ROUNDS. Players have **no shield** until they pick **any** Upgrade Shield Health card.

---

## How Shields Work

Players start with **no shield**. After picking **any** Upgrade Shield Health card, a shield pool sits **in front of your health**. While the shield is active, incoming hits are fully absorbed — the shield takes the damage instead of your HP. Once depleted, normal damage applies until the shield regenerates at the start of the next point.

The bubble color reflects your current shield strength:

| Color | Threshold |
|-------|-----------|
| 🟢 Green | Above 65% |
| 🟡 Yellow | 35% – 65% |
| 🔴 Red | Below 35% |

The shield resets to full at the start of every battle.

---

## Cards

### Upgrade Shield Health (5 tiers)

Permanently increases your maximum shield health. The **first** shield card sets a tier-specific base; additional shield cards **multiply** max health by the tier's percentage bonus.

| Card | Rarity | First shield max | Stack bonus |
|------|--------|------------------|-------------|
| Upgrade Shield Health I | Common | 100 | +20% |
| Upgrade Shield Health II | Scarce | 150 | +50% |
| Upgrade Shield Health III | Uncommon | 250 | +100% |
| Upgrade Shield Health IV | Epic | 500 | +200% |
| Upgrade Shield Health V | Legendary | 1,000 | +250% |

Any tier can appear in drafts based on rarity — no lower-tier prerequisite.

---

### Upgrade Resistance

A single draft card that opens a modal to choose which resistance type to improve: **Poison**, **Stun**, or **Lifesteal**. Each row shows your current level for that type (e.g. `Poison Resistance (LV 2)`) and an Info Overhaul-style delta preview of what the next upgrade would grant.

| Card | Rarity | Behavior |
|------|--------|----------|
| Upgrade Resistance | Uncommon | Opens resistance picker modal with `[Cancel]` / `[Select]` |

Uses Poison Resistance III artwork. **Cancel** returns you to the same draft hand without consuming the card. **Select** applies the chosen resistance upgrade and ends the pick — Upgrade Resistance **stays in your deck** and can be drafted again later.

Legacy resistance cards and internal marker cards cannot be added to custom decks or the DeckBuilder card browser.

Upgrades follow the same tier progression as before — the Nth pick of a given type uses the Nth tier effect:

| Upgrade # | Effect |
|-----------|--------|
| 1st | 25% resistance |
| 2nd | Upgrade to 50% if below 50%; otherwise +5% and +10% max HP |
| 3rd | Upgrade to 75% if below 75%; otherwise +5% and +25% max HP |
| 4th+ | +5% resistance |

- **Poison** — reduces poison damage to health and shield (detected from ROUNDS' built-in poison bullet effect).
- **Stun** — reduces stun duration received.
- **Lifesteal** — reduces healing opponents gain from lifesteal when they damage you.

Legacy tier cards (Poison/Stun/Lifesteal Resistance I–III) still work if already in a deck or hand, but no longer appear in drafts.

---

## Mod Compatibility

| Mod | Status | Notes |
|-----|--------|-------|
| **UnboundLib** | Required | Card registration |
| **ModdingUtils** | Required | Card framework |
| **DeckBuilder** | Optional | In-hand card deletion during picks |
| **RarityLib / RarityBundle** | Required | Custom rarities used by shield and resistance cards |
| **Info Overhaul** | Optional | Shield Health and all resistance stats in the stats popup, draft delta preview, and delete-confirmation preview |

---

## Info Overhaul Integration

If **Info Overhaul** is also installed, the following stats appear in the pick-screen stats panel:

- **Shield Health** — current / max (e.g. `74 / 110`), or `None` if you have no shield cards
- **Poison / Stun / Lifesteal Resistance** — current reduction percentage (e.g. `25%`, or `0%` if none)

The card delta preview (shown below the deck HUD during picks) shows before/after values when hovering a shield or Upgrade Resistance card. The resistance picker modal also shows per-type deltas inline and in the preview panel. If **DeckBuilder**'s delete mechanic is installed, the delete-confirmation modal uses dedicated removal previews for resistance marker cards.

---

## Notes

- The shield is separate from the block mechanic — both can be active at the same time.
- Shield health persists between rounds within a match; it only resets between full games.
- Cards with `allowMultiple = false` — you can only hold one copy of each tier at a time.
