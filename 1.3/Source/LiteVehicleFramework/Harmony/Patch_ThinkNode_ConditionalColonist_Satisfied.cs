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

namespace LiteVehicles
{
    [HarmonyPatch(typeof(ThinkNode_ConditionalColonist), "Satisfied")]
    public static class Patch_ThinkNode_ConditionalColonist_Satisfied
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            __result = pawn.IsColonist || pawn.GetComp<Comp_Vehicle>() is Comp_Vehicle vehicleComp && vehicleComp.MovementHandlerAvailable;
        }
    }
}
