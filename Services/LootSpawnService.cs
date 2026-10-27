using MelonLoader;
using MimicAPI.GameAPI;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

namespace EnemyDropLoot.Services
{
	internal static class LootSpawnService
	{
		internal static bool TrySpawnLoot(VMonster monster, int itemMasterId)
		{
			VWorld? vworld = CoreAPI.GetVWorld();
			if (vworld == null)
			{
				MelonLogger.Warning($"EnemyDropLoot: Skipping loot item {itemMasterId} for monster {monster.MasterID}: world is unavailable.");
				return false;
			}

			// Match native monster loot: snap to the nearest navigable point without horizontal scatter.
			Vector3 spawnPos = monster.PositionVector;
			Vector3 nearestPos = vworld.FindNearestPoly(spawnPos);
			if (nearestPos == NavMeshConstants.INVALID_POSITION)
			{
				MelonLogger.Warning($"EnemyDropLoot: Skipping loot item {itemMasterId} for monster {monster.MasterID}: no valid navmesh position near {spawnPos}.");
				return false;
			}

			ItemElement? itemElement = monster.VRoom.GetNewItemElement(itemMasterId, isFake: false);
			if (itemElement == null)
			{
				MelonLogger.Warning($"EnemyDropLoot: Failed to create loot item {itemMasterId} for monster {monster.MasterID}.");
				return false;
			}

			PosWithRot dropPos = monster.Position.Clone();
			dropPos.x = nearestPos.x;
			dropPos.y = nearestPos.y;
			dropPos.z = nearestPos.z;

			int spawnResult = monster.VRoom.SpawnLootingObject(itemElement, dropPos, monster.IsIndoor, ReasonOfSpawn.ActorDying);
			if (spawnResult == 0)
			{
				MelonLogger.Warning($"EnemyDropLoot: Failed to spawn loot item {itemMasterId} for monster {monster.MasterID}.");
				return false;
			}

#if DEBUG
			Debugging.DebugTools.OnModLootSpawned();
#endif
			return true;
		}
	}
}

