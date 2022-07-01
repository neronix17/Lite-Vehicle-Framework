using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly")]
    public static class Patch_PawnRenderer_CarryWeaponOpenly
    {
        static void Postfix(PawnRenderer __instance, ref bool __result)
        {
            Pawn pawn = __instance.graphics.pawn;
            if (pawn.def.HasModExtension<DefModExt_RenderWeaponExt>() && pawn.def.GetModExtension<DefModExt_RenderWeaponExt>().alwaysRender)
            {
                __result = true;
            }
        }
    }
}
