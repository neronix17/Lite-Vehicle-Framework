using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(PawnUtility), "IsTravelingInTransportPodWorldObject")]
    public static class Patch_PawnUtility_IsTravelingInTransportPodWorldObject
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            bool prevResult = __result;
            //Check for vehicles...
            if (pawn?.Faction != null)
                __result = prevResult || (ThingOwnerUtility.AnyParentIs<VehicleHandlerGroup>(pawn));
        }
    }
}
