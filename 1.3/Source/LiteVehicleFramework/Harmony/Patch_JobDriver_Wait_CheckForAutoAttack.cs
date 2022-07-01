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
    [HarmonyPatch(typeof(JobDriver_Wait), "CheckForAutoAttack")]
    public static class Patch_JobDriver_Wait_CheckForAutoAttack
    {
        // Transpiler Fails, too thick to read IL code, I think it's trying to skip the work tag check.
        // Probably could get away with a simple prefix changing the method when it's a vehicle.
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var playerFactionInfo = AccessTools.Property(typeof(Faction), nameof(Faction.OfPlayer)).GetGetMethod();
            var done = false;
            var instructionList = instructions.ToList();
            for (var i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                if (!done && instruction.operand == playerFactionInfo)
                {
                    done = true;
                    yield return instruction;
                    yield return instructionList[i + 1];
                    yield return instructionList[i + 2];
                    yield return instructionList[i + 3];
                    yield return instructionList[i + 4];
                    instruction = new CodeInstruction(OpCodes.Brfalse_S, instructionList[i + 1].operand);
                    i++;
                }

                yield return instruction;
            }
        }
    }
}
