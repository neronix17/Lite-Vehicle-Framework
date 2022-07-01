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
    [HarmonyPatch(typeof(GameEnder), "IsPlayerControlledWithFreeColonist")]
    public static class Patch_GameEnder_IsPlayerControlledWithFreeColonist
    {
        static void Postfix(Caravan caravan, ref bool __result)
        {
            if (!__result)
            {
                __result = caravan.PawnsListForReading.Any(p => p.Faction == Faction.OfPlayer && (p.TryGetComp<Comp_Vehicle>()?.AllOccupants.Any() ?? false));
            }
        }
    }
}
