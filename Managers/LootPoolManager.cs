using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EnemyDropLoot.Config;
using EnemyDropLoot.Services;
using MelonLoader;
using MimicAPI.GameAPI;
using UnityEngine;

namespace EnemyDropLoot.Managers
{
	internal static class LootPoolManager
	{
		private static readonly List<RandomSpawnedItemActorData> LootSources = new();
		private static readonly List<int> FallbackItemIds = new();

		private static IVroom _activeRoom = null!;
		private static bool _hasActiveRoom;
		private static string _allowText;
		private static string _blockText;
		private static HashSet<int> _allowed = new();
		private static HashSet<int> _blocked = new();
		private static bool _emptyFilterReported;

		internal static void Reset()
		{
			LootSources.Clear();
			FallbackItemIds.Clear();
			_hasActiveRoom = false;
			_emptyFilterReported = false;
#if DEBUG
			Debugging.DebugTools.OnRoomReset();
#endif
		}

#if DEBUG
		// Debug-only accessors used by the profiling/stress-test harness (DebugTools).
		internal static IVroom? ActiveRoomDebug => _hasActiveRoom ? _activeRoom : null;

		internal static bool DebugTryPickItem(out int itemMasterId) => TryPickItem(out itemMasterId);
#endif

		internal static void ConfigureForDungeon(DungeonRoom room)
		{
			Reset();
			_activeRoom = room;
			_hasActiveRoom = true;

			try
			{
				var spawnDict = ReflectionHelper.GetFieldValue<Dictionary<int, SpawnedActorData>>(room, "_spawnedActorDatas");
				if (spawnDict == null)
				{
					MelonLogger.Warning("EnemyDropLoot: Unable to fetch spawn data for current dungeon.");
					return;
				}

				foreach (var data in spawnDict.Values)
				{
					if (data == null || data.MarkerType != MapMarkerType.LootingObject)
					{
						continue;
					}

					if (data is RandomSpawnedItemActorData randomData)
					{
						LootSources.Add(randomData);
						foreach (var candidate in randomData.Candidates)
						{
							if (!FallbackItemIds.Contains(candidate.Key))
							{
								FallbackItemIds.Add(candidate.Key);
							}
						}
					}
				}

				MelonLogger.Msg("EnemyDropLoot: Loaded {0} loot spawn bundles with {1} unique items.", LootSources.Count, FallbackItemIds.Count);
			}
			catch (Exception ex)
			{
				MelonLogger.Error($"EnemyDropLoot: Failed to read dungeon loot pool. {ex}");
			}
		}

		internal static void TryHandleMonsterDeath(VMonster monster)
		{
			if (!EnemyDropLootPreferences.Enabled || LootSources.Count == 0)
			{
				return;
			}

			if (!_hasActiveRoom || monster.VRoom != _activeRoom)
			{
				return;
			}

			int dropCount = EnemyDropLootPreferences.RollDropCount();
			if (dropCount <= 0)
			{
				return;
			}

			for (int i = 0; i < dropCount; i++)
			{
				if (!TryPickItem(out int itemMasterId) || !LootSpawnService.TrySpawnLoot(monster, itemMasterId)) break;
			}
		}

		private static bool TryPickItem(out int itemMasterId)
		{
			itemMasterId = 0;

			if (LootSources.Count == 0)
			{
				return false;
			}
			RefreshFilters();
			if (_allowed.Count != 0 || _blocked.Count != 0)
			{
				int[] eligible = FallbackItemIds.Where(IsAllowed).ToArray();
				if (eligible.Length == 0)
				{
					if (!_emptyFilterReported) MelonLogger.Msg("EnemyDropLoot: No map loot matches the host's item filters; extra drops skipped.");
					_emptyFilterReported = true;
					return false;
				}
				for (int attempt = 0; attempt < 32; attempt++)
				{
					int candidate = LootSources[UnityEngine.Random.Range(0, LootSources.Count)].GetPickedItemValue();
					if (candidate != 0 && IsAllowed(candidate)) { itemMasterId = candidate; return true; }
				}
				itemMasterId = eligible[UnityEngine.Random.Range(0, eligible.Length)];
				return true;
			}

			RandomSpawnedItemActorData source = LootSources[UnityEngine.Random.Range(0, LootSources.Count)];
			int pickedItem = source.GetPickedItemValue();
			if (pickedItem == 0 && FallbackItemIds.Count > 0)
			{
				pickedItem = FallbackItemIds[UnityEngine.Random.Range(0, FallbackItemIds.Count)];
			}

			if (pickedItem == 0)
			{
				return false;
			}

			itemMasterId = pickedItem;
			return true;
		}

		private static bool IsAllowed(int id) => (_allowed.Count == 0 || _allowed.Contains(id)) && !_blocked.Contains(id);
		private static void RefreshFilters()
		{
			string allow = EnemyDropLootPreferences.AllowedItemIds, block = EnemyDropLootPreferences.BlockedItemIds;
			if (allow == _allowText && block == _blockText) return;
			_allowed = ParseIds(allow);
			_blocked = ParseIds(block);
			_allowText = allow;
			_blockText = block;
			_emptyFilterReported = false;
		}
		private static HashSet<int> ParseIds(string text) => string.IsNullOrWhiteSpace(text) ? new HashSet<int>() :
			new HashSet<int>(text.Split(',').Select(id => int.Parse(id, NumberStyles.Integer, CultureInfo.InvariantCulture)));
	}
}

