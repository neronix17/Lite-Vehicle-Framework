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
    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned")]
    public static class Patch_Pawn_HealthTracker_ShouldBeDowned
    {
        [HarmonyPrefix]
        public static bool VehicleShouldBeDowned(Pawn_HealthTracker __instance, ref bool __result)
        {
            var pawn = (Pawn)AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
            if (pawn != null)
            {
                Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
                if (vehicleComp != null)
                    if (!vehicleComp.Props.canBeDowned)
                    {
                        __result = false;
                        return false;
                    }
            }
            return true;
        }
    }
}
