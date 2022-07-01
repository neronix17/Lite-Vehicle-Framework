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
    [HarmonyPatch(typeof(MassUtility), "Capacity")]
    public static class Patch_MassUtility_Capacity
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Pawn p)
        {
            if (p?.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v)
            {
                __result = __result != 0 ? (v?.Props?.cargoCapacity ?? p.BodySize * 35) : 0;
            }
        }
    }
}
