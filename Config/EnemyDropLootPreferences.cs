using System;
using DooDesch;
using MelonLoader;
using UnityEngine;

namespace EnemyDropLoot.Config
{
	internal static class EnemyDropLootPreferences
	{
		private const string CategoryId = "EnemyDropLoot";

		private static MelonPreferences_Category _category = null!;
		private static MelonPreferences_Entry<bool> _enabled = null!;
		private static MelonPreferences_Entry<float> _dropChance = null!;
		private static MelonPreferences_Entry<int> _maxDropsPerKill = null!;
		private static MelonPreferences_Entry<bool> _removeFakeItemDrops = null!;
		private static HostSettings _hostSettings = null!;
		private static HostSetting<bool> _hostEnabled = null!;
		private static HostSetting<float> _hostDropChance = null!;
		private static HostSetting<int> _hostMaxDropsPerKill = null!;
		private static HostSettings _fakeDropSettings = null!;
		private static HostSetting<bool> _hostRemoveFakeItemDrops = null!;

		internal static void Initialize()
		{
			if (_category != null)
			{
				return;
			}

			_category = MelonPreferences.CreateCategory(CategoryId, "Enemy Drop Loot");
			_enabled = CreateEntry("Enabled", true, "Enabled", "Toggle the Enemy Drop Loot mod. Default: true");
			_dropChance = CreateEntry("DropChance", 0.1f, "Drop chance", "Chance (0-1) per roll that a slain enemy drops loot. Default: 0.1");
			_maxDropsPerKill = CreateEntry("MaxDropsPerKill", 1, "Max drops per kill", "Maximum number of loot rolls performed per enemy. Default: 1");
			_removeFakeItemDrops = CreateEntry("RemoveFakeItemDrops", false, "Remove fake item drops", "Prevent fake inventory items from dropping when a creature dies. Real items and extra loot are kept. Host setting. Default: false");

			_hostSettings = new HostSettings(CategoryId);
			_hostEnabled = _hostSettings.Register(_enabled, false);
			_hostDropChance = _hostSettings.Register(_dropChance, 0f);
			_hostMaxDropsPerKill = _hostSettings.Register(_maxDropsPerKill, 0);
			// Keep the original snapshot schema readable by older EnemyDropLoot clients.
			_fakeDropSettings = new HostSettings(CategoryId + ".FakeDrops");
			_hostRemoveFakeItemDrops = _fakeDropSettings.Register(_removeFakeItemDrops, false);
		}

		private static MelonPreferences_Entry<T> CreateEntry<T>(string identifier, T defaultValue, string displayName, string? description = null)
		{
			if (_category == null)
			{
				throw new InvalidOperationException("Preference category not initialized.");
			}

			return _category.CreateEntry(identifier, defaultValue, displayName, description);
		}

		internal static bool Enabled => _hostEnabled?.Value ?? false;
		internal static float DropChance => Mathf.Clamp01(_hostDropChance?.Value ?? 0f);
		internal static int MaxDropsPerKill => Mathf.Clamp(_hostMaxDropsPerKill?.Value ?? 0, 0, 100);
		internal static bool RemoveFakeItemDrops => Enabled && _fakeDropSettings?.IsHost == true && (_hostRemoveFakeItemDrops?.Value ?? false);
		
		internal static int RollDropCount()
		{
			if (!Enabled)
			{
				return 0;
			}

			int rolls = Math.Max(1, MaxDropsPerKill);
			int successes = 0;
			for (int i = 0; i < rolls; i++)
			{
				if (UnityEngine.Random.value <= DropChance)
				{
					successes++;
				}
			}

			return successes;
		}
	}
}

