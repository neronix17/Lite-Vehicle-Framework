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
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", new Type[] { typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult) })]
    public static class Patch_DamageWorker_AddInjury_FinalizeAndAddInjury
    {
        [HarmonyPostfix]
        public static void Postfix(DamageWorker_AddInjury __instance, Pawn pawn, Hediff_Injury injury, DamageInfo dinfo, ref DamageWorker.DamageResult result, ref float __result)
        {
            Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
            if (vehicleComp != null)
            {
                List<Pawn> affectedPawns = new List<Pawn>();

                if (vehicleComp.handlers != null && vehicleComp.handlers.Count > 0)
                {
                    foreach (VehicleHandlerGroup group in vehicleComp.handlers)
                    {
                        if (group.occupiedParts != null && group.handlers != null && group.handlers.Count > 0)
                        {
                            if (group.occupiedParts.Contains(injury.Part))
                            {
                                affectedPawns.AddRange(group.handlers);
                            }
                        }
                    }
                }

                if (affectedPawns != null && affectedPawns.Count > 0)
                {
                    DamageInfo newDamageInfo = new DamageInfo(dinfo);
                    float criticalBonus = 0f;
                    if (Rand.Value < vehicleComp.Props.seatHitCriticalHitChance) criticalBonus = dinfo.Amount * 2;
                    float newDamFloat = dinfo.Amount * vehicleComp.Props.seatHitDamageFactor + criticalBonus;
                    newDamageInfo.SetAmount((int)newDamFloat);
                    affectedPawns.RandomElement().TakeDamage(newDamageInfo);
                }
            }
        }
    }
}
