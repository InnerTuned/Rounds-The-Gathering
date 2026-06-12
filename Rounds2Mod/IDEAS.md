# In-Game Search Infrastructure — IMPLEMENTED
- `CardSearchModalUI` — opaque centered modal with [Cancel] / [Select], search bar, pagination
- `CardSearchPool` — builds searchable pools from runtime deck + prerequisites
- Used by Rare Search and Legendary Search (`SearchCardPatches`)

# Rare Search — IMPLEMENTED
- Rare Search card tutors a Rare-or-lower card from the player's remaining runtime deck

# Legendary Search — IMPLEMENTED
- Legendary Search card tutors any unlocked remaining deck card (no rarity cap)

# Sniped! [Uncommon]
- Click to remove a card from a player's stack
- Confirmation [Cancel][Remove] buttons, just like our Swap and Copycat cards
- They get an extra hand card pick, in this round