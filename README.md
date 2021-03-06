# Asset expander for Unturned 3

This plugin includes default asset expansions to add some missing methods/fields on server-side

You can think of this plugin as of what i would add to Unturned if i would work on it

### Custom Fields

Asset Type | Field Name | Field Type | Description | Requirement | Source
--- | --- | :-: | --- | --- | ---
Item | Thirst | byte | Decreases players water value | None | [Issue 1522 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1522)
Item | Starve | byte | Decreases players satiety | None | [Issue 1522 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1522)
Item | Tire | byte | Decreases players stamina | None | [Issue 1522 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1522)
Item | Lifesteal | float | Heals the player by specified amount when they attack players, zombies or animals. | None | [Issue 1969 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1969)
Item | Lifesteal_Min | float | Specifies minimum for lifesteal (e.g. if lifesteal must heal 1 hp, but you've set this field to 2, it will heal 2 hp) | None | [Issue 1969 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1969)
Item | Lifesteal_Max | float | Specifies maximum for lifesteal (e.g. if lifesteal must heal 150 hp, but you've set this field to 50, it will heal 50 hp) | None | [Issue 1969 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1969)
Item | Proof_Acid | tag | Works only for shirt and pants. If both shirt and pants have this tag player will be immune to acid | None | [Issue 1846 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1846)
Item | Disable_Backpack | tag | Works for every piece of clothing except for backpack for obvious reasons. | None | [Dumfoozler](https://github.com/Dumfoozler)
Item | Disable_Backpack_Bypass | tag | Applied to every piece of clothing. Bypasses 'Disable_Backpack' | None | [Dumfoozler](https://github.com/Dumfoozler)
Item | Player_Speed | float | Modifies player speed multiplier when item is held. Value is between '0.01' and max float value | None | [DerpyHoowes](https://github.com/DerpyHoowes)
Vehicle | Hook_Radius | float | Specifies distance that is used to determine which vehicles to hook. | 0Harmony | [Issue 1990 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1990)
Vehicle | Battery_ID | ushort | Specifies battery ID that should be used to replace | 0Harmony | [Issue 2239 in Unturned 3.x Community](https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/2239)

### Type Cheatsheet (for those who doesn't code)

Type Name | Possible Values
--- | :-:
tag | No values
byte | [0..255]
sbyte | [-127..128]
string | Any text
short | [-32768..32767]
ushort | [0..65535]
int | [-2147483648..2147483647]
uint | [0..4294967295]
long | [-9223372036854775808..9223372036854775807]
ulong | [0..18446744073709551615]
float | [-3.40282347E+38..3.40282347E+38]
double | [-1.7976931348623157E+308..1.7976931348623157E+308]
decimal | [-79228162514264337593543950335..79228162514264337593543950335]
