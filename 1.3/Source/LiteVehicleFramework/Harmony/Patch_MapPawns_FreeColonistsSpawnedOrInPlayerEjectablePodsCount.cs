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
    [HarmonyPatch(typeof(MapPawns), "FreeColonistsSpawnedOrInPlayerEjectablePodsCount")]
    public static class Patch_MapPawns_FreeColonistsSpawnedOrInPlayerEjectablePodsCount
    {
        [HarmonyPostfix]
        public static void Postfix(MapPawns __instance, ref int __result)
        {
            __result += __instance.AllPawns.Where(p => p.Faction == Faction.OfPlayer && p.TryGetComp<Comp_Vehicle>() != null).Sum(p => p.TryGetComp<Comp_Vehicle>().AllOccupants.Count);
        }
    }
}
