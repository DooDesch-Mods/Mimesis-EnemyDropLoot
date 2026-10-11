# MIMESIS - EnemyDropLoot

> 🛟 **Need help or found a bug?** Get support at [support.doodesch.de/enemydroploot](https://support.doodesch.de/enemydroploot).


> Dungeon enemies in MIMESIS now drop loot - every kill rolls against the current map's own loot pool and can drop a map-appropriate item next to the corpse, so combat actually rewards you.

![Version](https://img.shields.io/badge/version-1.0.9-blue)
![Game](https://img.shields.io/badge/game-MIMESIS-purple)
![MelonLoader](https://img.shields.io/badge/MelonLoader-0.7.3+-green)
![Status](https://img.shields.io/badge/status-working-brightgreen)

## Features

- Rolls for loot whenever an enemy dies in an active dungeon room (Harmony postfix on `VMonster.OnDying`) and spawns the reward at the corpse.
- Builds its loot pool from the live dungeon's own spawn data, so drops only ever contain items that could naturally appear on that level.
- Picks each drop through the source's own weighted roll, with a fallback to a known candidate item if a roll yields nothing.
- Spawns loot with the game's own helpers: snaps to the nearest navmesh point before spawning.
- Only activates inside real dungeon rooms (wired on `DungeonRoom.Initialize`) and tears the pool down when the room is vacated; non-dungeon contexts like shop, tram or arena are never configured.
- Configurable drop chance and number of rolls per kill via MelonPreferences.

## Requirements

| Component | Version |
|-----------|---------|
| MIMESIS | 0.3.2 |
| MelonLoader | 0.7.3+ |
| MimicAPI | Required - MimicAPI 0.4.2, [DooDesch fork](https://github.com/DooDesch/MimicAPI), included in the Nexus archive |

This mod uses MimicAPI to read private game internals (room spawn data and the world reference). MimicAPI must be installed for EnemyDropLoot to work.

The Nexus archive includes the maintained MimicAPI 0.4.2 fork. The Thunderstore manifest still refers to the original NeoMimicry package.

## Installation

- Recommended: install via a Thunderstore mod manager (r2modman / Gale), which pulls in MelonLoader and MimicAPI automatically.
- Manual:
  1. Make sure MelonLoader 0.7.3+ is installed in your game folder.
  2. Download `EnemyDropLoot.dll` from the [Releases page](https://github.com/DooDesch-Mods/Mimesis-EnemyDropLoot/releases).
  3. Copy `EnemyDropLoot.dll` **and** `MimicAPI.dll` into `MIMESIS/Mods/`.
  4. Launch the game once so the config section is generated.

## Configuration

Stored in `UserData/MelonPreferences.cfg` under the `EnemyDropLoot` category.

| Option | Description | Default | Values/Range |
|--------|-------------|---------|--------------|
| `Enabled` | Master toggle for the mod. | `true` | `true` / `false` |
| `DropChance` | Chance per roll that a slain enemy drops loot. Clamped to 0-1 at read time. | `0.1` | `0.0` - `1.0` |
| `MaxDropsPerKill` | Maximum number of loot rolls performed per enemy. Clamped to 0-100; the roll routine performs at least 1 roll. | `1` | `0` - `100` |

Each kill performs `max(1, MaxDropsPerKill)` rolls, and each roll succeeds when a random value is at or below `DropChance`. Raise `DropChance` toward `1.0` for more frequent drops, and raise `MaxDropsPerKill` for the chance of multiple drops per enemy.

## Usage

There are no keybinds and no menu - the mod is fully automatic. Enter any real dungeon mission map and the mod silently builds that room's loot pool; killed enemies may then drop map-appropriate items at their death position. The MelonLoader console confirms the pool loaded (for example, `Loaded N loot spawn bundles with M unique items`).

## Compatibility

Built for Mimesis 0.3.2 / MelonLoader 0.7.3. Drops only trigger inside dungeon rooms, so shop, tram and arena scenes are unaffected.

## Building (developers)

Built against `netstandard2.1`. References resolve from `Workspace/lib/game` (game DLLs) and `Workspace/lib/melonloader`, plus a `ProjectReference` to `Workspace/MimicAPI`. From the mod folder:

```
dotnet build -c Release
```

The post-build step copies `EnemyDropLoot.dll` into the configured `ModsDirectory`; the shared deploy configuration defaults to `_Test/MIMESIS/Mods`.

## Credits / License

Author: DooDesch. Licensed under the MIT License. Source and releases: [Mimesis-EnemyDropLoot](https://github.com/DooDesch-Mods/Mimesis-EnemyDropLoot).

## AI disclosure

The mod icon is AI-generated.
