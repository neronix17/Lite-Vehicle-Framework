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
    [HarmonyPatch(typeof(JobGiver_Orders), "TryGiveJob")]
    public static class Patch_JobGiver_Orders_TryGiveJob
    {
        [HarmonyPostfix]
        public static void Postfix(ref Job __result, Pawn pawn)
        {
            if (__result != null && pawn?.GetComp<Comp_Vehicle>() is Comp_Vehicle vehicleComp && !vehicleComp.CanFireWeapons)
            {
                __result = null;
            }
        }
    }
}
