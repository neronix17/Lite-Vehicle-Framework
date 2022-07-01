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
    [HarmonyPatch(typeof(JobGiver_Haul), "TryGiveJob")]
    public static class Patch_JobGiver_Haul_TryGiveJob
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            if (pawn.GetComp<Comp_Vehicle>() != null)
            {
                Comp_Vehicle comp = pawn.GetComp<Comp_Vehicle>();
                if (comp.CanMove && comp.CanManipulate && !pawn.Drafted)
                {
                    return true;
                }
                __result = null;
                return false;
            }
            return true;
        }
    }
}
