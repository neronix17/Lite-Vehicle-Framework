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
    [HarmonyPatch(typeof(MapPawns), "AnyPawnBlockingMapRemoval", MethodType.Getter)]
    public static class Patch_MapPawns_AnyPawnblockingMapRemoval
    {
        static void PostFix(MapPawns __instance, ref bool __result)
        {
            if (!__result)
            {
                __result = __instance.AllPawns.ToList().Any(p => p.Faction == Faction.OfPlayer && (p.TryGetComp<Comp_Vehicle>()?.AllOccupants.Any() ?? false));
            }
        }
    }
}
