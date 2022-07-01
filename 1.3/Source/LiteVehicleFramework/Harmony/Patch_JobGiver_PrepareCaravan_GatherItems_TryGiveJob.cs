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
using Verse.AI.Group;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(JobGiver_PrepareCaravan_GatherItems), "TryGiveJob")]
    public static class Patch_JobGiver_PrepareCaravan_GatherItems_TryGiveJob
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Job __result, Pawn pawn)
        {
            if (pawn?.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v)
            {
                if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                {
                    __result = null;
                    return false;
                }
                var lord = pawn.GetLord();
                var thing = GatherItemsForCaravanUtility.FindThingToHaul(pawn, lord);
                if (thing == null)
                {
                    __result = null;
                    return false;
                }
                __result = new Job(JobDefOf.PrepareCaravan_GatherItems, thing)
                {
                    lord = lord
                };
                return false;
            }
            return true;
        }
    }
}
