# Asset expander for Unturned 3

This plugin includes default asset expansions to add some missing methods/fields on server-side

### Custom Fields

Asset Type | Field Name | Field Type | Description
--- | --- | :-: | ---
Item | Thirst | byte | Decreases players water value
Item | Starve | byte | Decreases players satiety
Item | Tire | byte | Decreases players stamina

### Type Cheatsheet (for those who doesn't code)

Type Name | Possible Values
--- | :-:
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
