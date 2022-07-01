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
    [HarmonyPatch(typeof(AddictionUtility), "CheckDrugAddictionTeachOpportunity")]
    public class Patch_AddictionUtility_CheckDrugAddictionTeachOpportunity
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn)
        {
            if (pawn.def.race.FleshType.IsVehicle())
            {
                return false;
            }
            return true;
        }
    }
}
