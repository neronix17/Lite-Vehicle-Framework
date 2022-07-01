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
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherItems), "LordToilTick")]
    public static class Patch_LordToil_LordToil_PrepareCaravan_GatherItems_LordToilTick
    {
        public static bool firstRun;

        [HarmonyPrefix]
        public static bool Prefix(LordToil_PrepareCaravan_GatherItems __instance)
        {
            if (Find.TickManager.TicksGame % 120 == 0)
            {
                var flag = true;
                for (var i = 0; i < __instance.lord.ownedPawns.Count; i++)
                {
                    var pawn = __instance.lord.ownedPawns[i];
                    if ((pawn.IsColonist || pawn.GetComp<Comp_Vehicle>() != null) && pawn.mindState.lastJobTag != JobTag.WaitingForOthersToFinishGatheringItems)
                    {
                        if (!firstRun)
                        {
                            flag = false;
                            firstRun = true;
                            break;
                        }
                        else
                        {
                            if (pawn.CurJob.def != JobDefOf.Wait_Wander)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                }
                if (flag)
                {
                    var allPawnsSpawned = __instance.Map.mapPawns.AllPawnsSpawned;
                    for (var j = 0; j < allPawnsSpawned.Count; j++)
                    {
                        if (allPawnsSpawned[j].CurJob != null && allPawnsSpawned[j].jobs.curDriver is JobDriver_PrepareCaravan_GatherItems && allPawnsSpawned[j].CurJob.lord == __instance.lord)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    __instance.lord.ReceiveMemo("AllItemsGathered");
                    firstRun = false;
                }
            }
            return false;
        }
    }
}
