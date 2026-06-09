# Keybind: Utility mod
- A card that has a [Keybound] marker to it has special behavior
- On card select, a modal pops up, asking the user to bind the card's effect to a specific key
- There's [Cancel] and [Bind Key] on the modal ([Bind Key] is initially greyed out)
- On key press (if that key isn't taken), the modal displays text "Bind [card name] to [key]?", and the [Bind key] button becomes selectable
- On selection, the card doesn't go to the regular card stack, but goes to the Effect Stack.
- The Effect Stack mirrors each player's Card stack, on the bottom-right corner of the screen, and displays exactly like the regular Card Stack
- All [Keybound] cards appear on the Effect Stack, instead of the regular Card Stack
- When a [Keybound] card isn't re-loading or isn't available when the match first begins (it has an initial timer, and a re-load timer), it's covered by a transparency film, which drains downwards, as the skill become more available. The skill has a red border when it cannot be activate by keypress, and has a green border when it becomes available.
- Hovering over the [Keyboard] effect displays just like the top card stack, but it also displays what key the skill is bound to

# Teleport [Rare]
- Press [keybind] to Teleport to cursor's location
- Becomes available 10s after the game stars
- Initial cooldown: 15s

# Sniped! [Uncommon]
- Click to remove a card from a player's stack
- Confirmation [Cancel][Remove] buttons, just like our Swap and Copycat cards
- They get an extra hand card pick, in this round