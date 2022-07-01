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
    [HarmonyPatch(typeof(PawnDownedWiggler), "WigglerTick")]
    public static class Patch_PawnDownedWiggler_WigglerTick
    {
        [HarmonyPrefix]
        public static bool Prefix(PawnDownedWiggler __instance)
        {
            var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null)
            {
                Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
                if (vehicleComp != null)
                {
                    if (!vehicleComp.Props.canWiggleWhenDowned) 
                    { 
                        return false; 
                    }
                }
            }
            return true;
        }
    }
}
