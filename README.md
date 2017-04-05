# Silver Pexer

## Build
1. Install the [.NET Framework 4.6.2 Developer Pack](http://go.microsoft.com/fwlink/?LinkId=780617)
2. Install the [.NET Core 1.1+ SDK ](https://www.microsoft.com/net/download/core#/current)
3. Run `make.bat`

## Run
1. Build with `make.bat`
2. Run `.\bin\SilverPexer.exe`

## Configuration
You can edit `.\bin\appsettings.json` to change the following settings :

- `username` Your username (optional)
- `password` Your password (optional)
- `actionPoints` The amount of action points to use (before to go to sleep)
- `sleepType` The type of place you want to sleep in: _forteresse_ is `Camp`, _auberge_ is `Inn`
- `pathToSleep` The path to the closest inn or camp (used to go to sleep)
- `timeToSleep` The time to sleep in the inn (in hours)
- `goToSleepWhenMessage` To go to sleep when a message is received (true or false)
- `goToSleepWhenPlayer` To go to sleep when another player is spotted (true or false),
- `grabLoot`: To grab the loot on the ground (true or false),
- `levelUp` Stats distribution on level up
  - `constitution` The amount of points to place in constitution
  - `strength` The amount of points to place in strength
  - `agility` The amount of points to place in agility
  - `intelligence` The amount of points to place in intelligence
- `spell` The spell number to use for attacking monsters (e.g. Cone de froid image src is systeme/mag`17`.gif, hence the value should be **17**, `0` for swichting to warrior mode)
- `monster` The monster ID to use for attacking monsters (e.g. Leviathan de Corail src is systeme/monster`286`.gif, hence the value should be **286**)
- `potion` Potion to drink
  - `id` The potion number (e.g. systeme/obj`303`.gif is the image for a mana potion, hence the value should be **303**)
  - `amount` The amount of potions to drink
  - `hits` The amount of hits to do before to drink the amount of potions specified

## System Requirements

- Windows 7+
- .NET Framework 4.6.2
- .NET Core 1.1+
- Google Chrome 55+

## Linux and MacOS support
You can fetch the "feature/unix-support" branch but it only works with .NET Core 1.0 so far and has problems running on Windows.
