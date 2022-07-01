using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;
using System.Reflection.Emit;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(FloatMenuUtility), "GetMeleeAttackAction")]
    public static class Patch_FloatMenuUtility_GetMeleeAttackAction
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            var storyInfo = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));
            var done = false;
            var instructionList = instructions.ToList();
            for (var i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                if (!done && instruction.operand == storyInfo)
                {
                    yield return instruction;
                    yield return new CodeInstruction(instructionList[i + 3]);
                    yield return new CodeInstruction(instructionList[i - 2]) { labels = new List<Label>() };
                    yield return new CodeInstruction(instructionList[i - 1]);
                    instruction = new CodeInstruction(instruction);
                    done = true;
                }

                yield return instruction;
            }
        }
    }
}
