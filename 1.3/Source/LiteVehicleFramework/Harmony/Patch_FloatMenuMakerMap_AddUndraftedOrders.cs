using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using System.Reflection.Emit;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddUndraftedOrders")]
    public static class Patch_FloatMenuMakerMap_AddUndraftedOrders
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = instructions.ToList();
            var workSettings = AccessTools.Field(typeof(Pawn), nameof(Pawn.workSettings));
            for (var i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                yield return instruction;
                #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (instruction.operand == workSettings)
                {
                    yield return new CodeInstruction(OpCodes.Brfalse_S, instructionList[i + 3].operand);
                    yield return new CodeInstruction(instructionList[i - 2]) { labels = new List<Label>() };
                    yield return new CodeInstruction(instructionList[i - 1]);
                    yield return new CodeInstruction(instruction);
                }
                #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }
        }
    }
}
