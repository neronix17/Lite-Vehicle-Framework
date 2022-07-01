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
    [HarmonyPatch(typeof(FoodUtility), "IsAcceptablePreyFor")]
    public class Patch_FoodUtility_IsAcceptablePreyFor
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Pawn predator, Pawn prey)
        {
            if (prey.def.race.FleshType.IsVehicle())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
