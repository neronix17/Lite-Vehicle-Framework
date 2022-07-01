using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.Planet;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(Caravan), "IsPlayerControlled", MethodType.Getter)]
    public static class Patch_Caravan_IsPlayerControlled
    {
        [HarmonyPostfix]
        public static void Postfix(Caravan __instance, ref bool __result)
        {
            if (!__result)
            {
                __result = __instance.PawnsListForReading.Any(p => p.Faction == Faction.OfPlayer && (p.TryGetComp<Comp_Vehicle>()?.AllOccupants.Any() ?? false));
            }
        }
    }
}
