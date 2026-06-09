# ShieldsMod

Adds a persistent shield bubble to every player and a suite of upgradeable shield cards to ROUNDS.

---

## How Shields Work

Each player has a shield pool that sits **in front of their health**. While the shield is active, incoming hits are fully absorbed — the shield takes the damage instead of your HP. Once depleted, normal damage applies until the shield regenerates at the start of the next point.

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

Permanently increases your maximum shield health. Tiers **stack multiplicatively** — taking multiple upgrades compounds the bonus.

| Card | Rarity | Max HP Multiplier | Cumulative (from base 100) |
|------|--------|-------------------|---------------------------|
| Upgrade Shield Health I | Common | ×1.10 | 110 |
| Upgrade Shield Health II | Common | ×1.25 | 137 |
| Upgrade Shield Health III | Common | ×1.50 | 206 |
| Upgrade Shield Health IV | Common | ×2.00 | 413 |
| Upgrade Shield Health V | Common | ×2.65 | 1,094 |

Each tier **requires the previous tier** to already be in your hand (unlock-gated via DeckBuilder if installed).

---

### Poison Resistance (3 tiers)

Reduces poison damage received by both your health **and** your shield. Only the highest tier you own applies — they do not stack.

| Card | Rarity | Poison Damage Reduction |
|------|--------|------------------------|
| Poison Resistance I | Common | 25% |
| Poison Resistance II | Rare | 50% |
| Poison Resistance III | Rare | 75% |

Poison is detected from any source that uses ROUNDS' built-in poison bullet effect. Resistance applies to the raw damage before the shield absorbs it, so both your bubble and your HP benefit simultaneously.

Each tier requires the previous tier (unlock-gated via DeckBuilder if installed).

---

## Mod Compatibility

| Mod | Status | Notes |
|-----|--------|-------|
| **UnboundLib** | Required | Card registration |
| **ModdingUtils** | Required | Card framework |
| **DeckBuilder** | Optional | Tier unlock-gating (pick tier I before tier II, etc.) and in-hand card deletion during picks |
| **Info Overhaul** | Optional | Adds Shield Health and Poison Resistance to the pick-screen stats display and card delta preview |

---

## Info Overhaul Integration

If **Info Overhaul** is also installed, the following stats appear in the pick-screen stats panel:

- **Shield Health** — current / max (e.g. `74 / 110`)
- **Poison Resistance** — current reduction percentage (e.g. `25%`)

The card delta preview (shown below the deck HUD during picks) also shows the before/after values when hovering a Shield or Poison Resistance card.

---

## Notes

- The shield is separate from the block mechanic — both can be active at the same time.
- Shield health persists between rounds within a match; it only resets between full games.
- Cards with `allowMultiple = false` — you can only hold one copy of each tier at a time.
