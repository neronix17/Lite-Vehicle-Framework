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
    [HarmonyPatch(typeof(Pawn), "CurrentlyUsableForBills")]
    public static class Patch_Pawn_CurrentlyUsableForBills
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            Comp_Vehicle vehicleComp = __instance.GetComp<Comp_Vehicle>();
            if (vehicleComp != null)
            {
                if (!__instance.pather.MovingNow)
                {
                    __result = true;
                }
            }
        }
    }
}
