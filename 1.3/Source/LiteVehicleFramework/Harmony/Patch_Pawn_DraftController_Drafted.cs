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
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted")]
    public static class Patch_Pawn_DraftController_Drafted
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn_DraftController __instance, bool value)
        {
            if (__instance?.pawn?.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v)
            {
                if (value && !__instance.Drafted)
                {
                    if (!v.CanMove)
                    {
                        Messages.Message("CompVehicle_CannotMove".Translate(__instance.pawn.KindLabel), MessageTypeDefOf.RejectInput);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
