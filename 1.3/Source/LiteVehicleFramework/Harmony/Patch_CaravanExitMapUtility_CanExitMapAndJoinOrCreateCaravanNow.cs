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
using RimWorld.Planet;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(CaravanExitMapUtility), "CanExitMapAndJoinOrCreateCaravanNow")]
    public static class Patch_CaravanExitMapUtility_CanExitMapAndJoinOrCreateCaravanNow
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            __result = pawn.Spawned && pawn.Map.exitMapGrid.MapUsesExitGrid && (pawn.IsColonist || pawn.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle && compVehicle.CanMove || CaravanExitMapUtility.FindCaravanToJoinFor(pawn) != null);
        }
    }
}
