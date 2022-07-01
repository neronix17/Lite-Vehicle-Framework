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
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherAnimals), "UpdateAllDuties")]
    public static class Patch_LordToil_PrepareCaravan_GatherAnimals_UpdateAllDuties
    {
        // Might not be necessary anymore thanks to the animal system changing to differentiate roaming and smart animals.
        [HarmonyPrefix]
        public static void Prefix(LordToil_PrepareCaravan_GatherAnimals __instance)
        {
            if (__instance.lord.ownedPawns is List<Pawn> pawns && !pawns.NullOrEmpty() && pawns.FirstOrDefault(x => x.GetComp<Comp_Vehicle>() != null) != null)
            {
                for (var i = 0; i < __instance.lord.ownedPawns.Count; i++)
                {
                    var pawn = __instance.lord.ownedPawns[i];
                    if (pawn.IsColonist || pawn.RaceProps.Animal ||
                        pawn.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle && compVehicle.MovementHandlerAvailable)
                    {
                        var meetingPoint = Traverse.Create(__instance).Field("meetingPoint").GetValue<IntVec3>();

                        pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_GatherAnimals, meetingPoint, -1f);
                    }
                    else
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_Wait);
                    }
                }
            }
        }
    }
}
