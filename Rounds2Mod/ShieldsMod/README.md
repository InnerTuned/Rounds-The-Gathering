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

### Poison Resistance (3 tiers)

Reduces poison damage received by both your health **and** your shield. The **first** resistance card sets a tier-specific value; additional cards **stack** using the rules below.

| Card | Rarity | First resistance | When stacking |
|------|--------|------------------|---------------|
| Poison Resistance I | Uncommon | 25% | +5% |
| Poison Resistance II | Rare | 50% | Upgrade to 50% if below 50%; otherwise +5% and +10% max HP |
| Poison Resistance III | Epic | 75% | Upgrade to 75% if below 75%; otherwise +5% and +25% max HP |

Poison is detected from any source that uses ROUNDS' built-in poison bullet effect. Resistance applies to the raw damage before the shield absorbs it, so both your bubble and your HP benefit simultaneously. Any tier can appear in drafts — no lower-tier prerequisite.

---

### Stun Resistance (3 tiers)

Reduces stun duration received. Uses the same first-pick and stacking rules as Poison Resistance.

| Card | Rarity | First resistance | When stacking |
|------|--------|------------------|---------------|
| Stun Resistance I | Uncommon | 25% | +5% |
| Stun Resistance II | Rare | 50% | Upgrade to 50% if below 50%; otherwise +5% and +10% max HP |
| Stun Resistance III | Epic | 75% | Upgrade to 75% if below 75%; otherwise +5% and +25% max HP |

---

### Lifesteal Resistance (3 tiers)

Reduces healing your opponent gains from lifesteal when they damage you. Uses the same first-pick and stacking rules as Poison Resistance.

| Card | Rarity | First resistance | When stacking |
|------|--------|------------------|---------------|
| Lifesteal Resistance I | Uncommon | 25% | +5% |
| Lifesteal Resistance II | Rare | 50% | Upgrade to 50% if below 50%; otherwise +5% and +10% max HP |
| Lifesteal Resistance III | Epic | 75% | Upgrade to 75% if below 75%; otherwise +5% and +25% max HP |

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

The card delta preview (shown below the deck HUD during picks) shows before/after values when hovering a shield or resistance card. If **DeckBuilder**'s delete mechanic is installed, the delete-confirmation modal uses dedicated removal previews for these cards.

---

## Notes

- The shield is separate from the block mechanic — both can be active at the same time.
- Shield health persists between rounds within a match; it only resets between full games.
- Cards with `allowMultiple = false` — you can only hold one copy of each tier at a time.
