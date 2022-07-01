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
    [HarmonyPatch(typeof(IncidentWorker_Disease), "PotentialVictims")]
    public class Patch_IncidentWorker_Disease_PotentialVictims
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Pawn> __result, IIncidentTarget target)
        {
            __result = __result.Where(delegate (Pawn p)
            {
                if (p.RaceProps.FleshType.IsVehicle())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            });
        }
    }
}
