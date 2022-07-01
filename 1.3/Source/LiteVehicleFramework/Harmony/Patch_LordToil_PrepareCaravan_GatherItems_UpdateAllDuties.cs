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
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherItems), "UpdateAllDuties")]
    public static class Patch_LordToil_PrepareCaravan_GatherItems_UpdateAllDuties
    {
        [HarmonyPrefix]
        public static bool Prefix(LordToil_PrepareCaravan_GatherItems __instance)
        {
            if (__instance.lord.ownedPawns.FindAll(x => x.GetComp<Comp_Vehicle>() != null) is List<Pawn> pawns && !pawns.NullOrEmpty())
            {
                for (var i = 0; i < pawns.Count; i++)
                {
                    var pawn = pawns[i];
                    if (pawn.IsColonist || pawn.GetComp<Comp_Vehicle>() is Comp_Vehicle comp && comp.CanMove)
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_GatherItems);
                    }
                    else if (pawn.RaceProps.Animal)
                    {
                        var meetingPoint = Traverse.Create(__instance).Field("meetingPoint").GetValue<IntVec3>();
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_Wait, meetingPoint, -1f);
                    }
                    else
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_Wait);
                    }
                }
                return false;
            }
            return true;
        }
    }
}
