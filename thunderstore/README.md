# MIMESIS - EnemyDropLoot

> 🛟 **Need help or found a bug?** Get support at [support.doodesch.de/enemydroploot](https://support.doodesch.de/enemydroploot).


> Dungeon enemies in MIMESIS now drop loot - every kill rolls against the current map's own loot pool and can drop a map-appropriate item next to the corpse, so combat actually rewards you.

![Version](https://img.shields.io/badge/version-1.2.0-blue)
![Game](https://img.shields.io/badge/game-MIMESIS-purple)
![MelonLoader](https://img.shields.io/badge/MelonLoader-0.7.3+-green)
![Status](https://img.shields.io/badge/status-working-brightgreen)

![EnemyDropLoot in MIMESIS](https://raw.githubusercontent.com/DooDesch-Mods/Mimesis-EnemyDropLoot/main/docs/img/header.jpg)

## Features

- Rolls for loot whenever an enemy dies in an active dungeon room and spawns the reward at the corpse.
- Builds its loot pool from the live dungeon's own spawn data, so drops only ever contain items that could naturally appear on that level.
- Picks each drop through the source's own weighted roll, with a fallback to a known candidate item if a roll yields nothing.
- Spawns loot with the game's own helpers: snaps to the nearest navmesh point before spawning.
- Additional loot only activates inside real dungeon rooms; its loot pool is never configured in shop, tram or arena scenes.
- Configurable drop chance and number of rolls per kill via MelonPreferences.
- Optional host setting to prevent fake inventory items from dropping on death. Real inventory items, vanilla reward drops and the mod's extra loot are kept.

## Screenshots

![Native dungeon enemies in MIMESIS before a kill](https://raw.githubusercontent.com/DooDesch-Mods/Mimesis-EnemyDropLoot/main/docs/img/dungeon-enemies.jpg)

A real MIMESIS dungeon encounter. The drop photo below comes from a separate captured kill.

![A native bust and broken umbrella dropped by EnemyDropLoot beside a defeated mimic](https://raw.githubusercontent.com/DooDesch-Mods/Mimesis-EnemyDropLoot/main/docs/img/enemy-drops.jpg)

A bust and broken umbrella beside the defeated mimic. Both visible items are matched to EnemyDropLoot spawns; this photo session uses 100% drop chance and three rolls.

## Requirements

| Component | Version |
|-----------|---------|
| MIMESIS | 0.4.0 |
| MelonLoader | 0.7.3+ |
| MimicAPI | Required - MimicAPI 0.4.3, [Nexus download](https://www.nexusmods.com/mimesis/mods/11), install separately |

EnemyDropLoot uses MimicAPI to read private game internals, so MimicAPI must be installed for the mod to work.

For Nexus, install [MimicAPI 0.4.3](https://www.nexusmods.com/mimesis/mods/11) separately. Keep one `Mods/MimicAPI.dll`; the mod archive does not include it. Thunderstore resolves its existing MimicAPI dependency through the mod manager.

## Installation

- Recommended: install via a Thunderstore mod manager (r2modman / Gale). It pulls in MelonLoader and MimicAPI for you.
- Manual:
  1. Make sure MelonLoader 0.7.3+ is installed in your game folder.
  2. Download `EnemyDropLoot.dll` from the [Releases page](https://github.com/DooDesch-Mods/Mimesis-EnemyDropLoot/releases).
  3. Copy `EnemyDropLoot.dll` **and** `MimicAPI.dll` into `MIMESIS/Mods/`.
  4. Launch the game once so the config section is generated.

## Configuration

Stored in `UserData/MelonPreferences.cfg` under `[EnemyDropLoot]`.

Gameplay settings follow the lobby host without overwriting saved client preferences. Without the mod on the host, clients use vanilla behavior. Live settings apply after the host changes them through a MelonPreferences UI or reloads the preferences; editing the file alone does not reload it. Use the same current mod version on host and clients for the new options and live updates. Older versions retain their initial legacy snapshot. Invalid new values use the registered default with a warning.

All settings are live for subsequent kills and drops. Only the host spawns extra loot or suppresses fake death drops; clients receive those server objects.

| Option | Default | Range and effect |
|---|---|---|
| `Enabled` | `true` | Master switch. |
| `DropChance` | `0.1` | Chance per roll, clamped to 0-1. Zero produces no extra loot; 1 succeeds on every roll. |
| `MaxDropsPerKill` | `1` | Rolls per kill, clamped to 0-100. Zero performs no rolls. |
| `RemoveFakeItemDrops` | `false` | Filter fake inventory items from death drops while Enabled is true. |
| `AllowedItemIds` | Empty | Comma-separated positive item master IDs, up to 128. Empty allows every eligible current-map item. |
| `BlockedItemIds` | Empty | Comma-separated positive item master IDs, up to 128. Blocked IDs take priority. |

Filters apply only to extra mod loot and never introduce items outside the current map pool. If no eligible item remains, no extra item is spawned. `RemoveFakeItemDrops` preserves real items, native rewards and extra loot. It does not remove items already on the ground or fake items deliberately dropped by living mimics.

## Usage

No keybinds and no menu - the mod is fully automatic. Enter any real dungeon mission map and the mod silently builds that room's loot pool; killed enemies may then drop map-appropriate items at their death position. The MelonLoader console confirms the pool loaded (for example, `Loaded N loot spawn bundles with M unique items`).

## Images

Gallery images are captured in MIMESIS; release artwork uses real game photos.

Built against MIMESIS 0.4.0 / MelonLoader 0.7.3.
