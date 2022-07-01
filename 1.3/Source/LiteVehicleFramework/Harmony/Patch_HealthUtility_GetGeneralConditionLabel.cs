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
    [HarmonyPatch(typeof(HealthUtility), "GetGeneralConditionLabel")]
    public static class Patch_HealthUtility_GetGeneralConditionLabel
    {
        [HarmonyPrefix]
        public static bool ReplaceConditionLabel(ref string __result, Pawn pawn, bool shortVersion = false)
        {
            if (pawn != null)
            {
                Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
                if (vehicleComp != null)
                {
                    if (pawn.Downed || !pawn.health.capacities.CanBeAwake)
                    {
                        __result = vehicleComp.Props.labelInoperable;
                        return false;
                    }
                    if (pawn.Dead)
                    {
                        __result = vehicleComp.Props.labelBroken;
                        return false;
                    }
                    if (pawn.health.summaryHealth.SummaryHealthPercent < 0.95)
                    {
                        __result = vehicleComp.Props.labelDamaged;
                        return false;
                    }
                    __result = vehicleComp.Props.labelUndamaged;
                    return false;
                }
            }
            return true;
        }
    }
}
