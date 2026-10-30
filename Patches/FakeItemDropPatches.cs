using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using EnemyDropLoot.Config;
using HarmonyLib;

namespace EnemyDropLoot.Patches
{
	[HarmonyPatch(typeof(InventoryController), nameof(InventoryController.OnDead))]
	internal static class FakeItemDeathDropPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var extract = AccessTools.Method(typeof(InventoryController), nameof(InventoryController.ExtractAllItemElement), new[] { typeof(bool) });
			var filtered = AccessTools.Method(typeof(FakeItemDeathDropPatch), nameof(ExtractDeathDrops));
			var result = instructions.ToList();
			int replaced = 0;
			foreach (var instruction in result)
			{
				if (!instruction.Calls(extract)) continue;
				instruction.opcode = OpCodes.Call;
				instruction.operand = filtered;
				replaced++;
			}
			if (replaced != 1)
				throw new InvalidOperationException($"EnemyDropLoot: InventoryController.OnDead must extract inventory exactly once; found {replaced} calls.");
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Dictionary<int, ItemElement> ExtractDeathDrops(InventoryController inventory, bool turnOff)
		{
			var drops = inventory.ExtractAllItemElement(turnOff);
			if (!EnemyDropLootPreferences.RemoveFakeItemDrops) return drops;

			// Filter the extracted copy. Native equipment shutdown and inventory cleanup still run.
			foreach (int slot in drops.Where(item => item.Value.IsFake).Select(item => item.Key).ToArray())
				drops.Remove(slot);
			return drops;
		}
	}
}
