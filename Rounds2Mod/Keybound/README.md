# Keybound Mod for ROUNDS

Utility cards with **[Keybound]** effects — bind a key during the draft, then activate the effect from the **Effect Stack** (bottom-right) instead of the normal card bar.

Part of the **Rounds: The Gathering** mod pack.

## How it works

1. Draft a **[Keybound]** card from the table.
2. A modal asks you to press a key. **[Bind Key]** stays disabled until you press an unbound key.
3. Confirm to bind the effect. The card is **not** added to your normal card stack — it goes to the **Effect Stack**.
4. During combat, press the bound key to activate the effect.
5. Effects show a cooldown overlay (drains downward), a **red** border while locked, and a **bright green** border when ready.
6. Hover an effect slot to preview the card and see which key it is bound to.

## Cards

### Teleport [Rare]
- Press your bound key to teleport to your cursor.
- Becomes available **10s** after the round starts.
- **15s** cooldown after each use.

### Invisibility [Rare]
- Press your bound key to turn ghostly white at **20%** opacity (player and shield).
- While active, bullets pass through you and your shield without collision or damage.
- Becomes available **10s** after the round starts.
- Lasts **7s**. **20s** cooldown after it ends.

## Debugging

Filter logs with `[Keybound]`.

## Requirements

Same as the mod pack: BepInEx, UnboundLib, ModdingUtils, RarityLib, RarityBundle.
