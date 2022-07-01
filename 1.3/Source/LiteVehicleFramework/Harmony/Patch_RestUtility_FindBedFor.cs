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
    [HarmonyPatch(typeof(RestUtility), "FindBedFor")]
    public static class Patch_RestUtility_FindBedFor
    {
        [HarmonyPostfix]
        public static void Postfix(ref Building_Bed __result, Pawn sleeper)
        {
            __result = sleeper?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle ? null : __result;
        }
    }
}
