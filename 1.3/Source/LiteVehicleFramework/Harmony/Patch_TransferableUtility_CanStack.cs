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
    [HarmonyPatch(typeof(TransferableUtility), "CanStack")]
    public static class Patch_TransferableUtility_CanStack
    {
        [HarmonyPrefix]
        public static bool Prefix(Thing thing, ref bool __result)
        {
            if (thing.def.category == ThingCategory.Pawn)
            {
                Pawn pawn = (Pawn)thing;
                if (pawn.def.HasComp(typeof(Comp_Vehicle)))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
