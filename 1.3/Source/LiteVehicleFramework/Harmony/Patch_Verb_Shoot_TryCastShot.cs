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
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public static class Patch_Verb_Shoot_TryCastShot
    {
        [HarmonyPrefix]
        public static bool Prefix(Verb_Shoot __instance, ref bool __result)
        {
            var pawn = __instance.CasterPawn;
            if (pawn != null)
            {
                Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
                if (vehicleComp != null)
                {
                    if (vehicleComp.weaponStatus == WeaponState.frozen)
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
