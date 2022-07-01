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
    [HarmonyPatch(typeof(RestUtility), "InBed")]
    public static class Patch_RestUtility_InBed
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn p, bool __result)
        {
            if (p.def.HasComp(typeof(Comp_Vehicle)))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
