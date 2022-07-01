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
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming")]
    public static class Patch_PawnRenderer_DrawEquipmentAiming
    {
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        public static void Prefix(PawnRenderer __instance, ref float aimAngle)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);
            if (pawn != null && pawn.def.HasModExtension<DefModExt_RenderWeaponExt>())
            {
                Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
                if (!(stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid))
                {
                    if (pawn.def.GetModExtension<DefModExt_RenderWeaponExt>().weaponCardinalDirection)
                    {
                        if (pawn.Rotation == Rot4.South)
                        {
                            aimAngle = 180f;
                        }
                        else if (pawn.Rotation == Rot4.North)
                        {
                            aimAngle = 0f;
                        }
                        else if (pawn.Rotation == Rot4.East)
                        {
                            aimAngle = 90f;
                        }
                        else if (pawn.Rotation == Rot4.West)
                        {
                            aimAngle = 270f;
                        }
                    }
                }
            }
        }
    }
}
