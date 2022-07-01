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
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class Patch_PawnRenderer_DrawEquipment
    {
        static void Prefix(PawnRenderer __instance, ref Vector3 rootLoc)
        {
            Pawn pawn = __instance.graphics.pawn;
            Vector2 offsetLoc = new Vector2(0, 0);
            if (pawn?.def?.GetModExtension<DefModExt_RenderWeaponExt>()?.renderPosOffset != null)
            {
                offsetLoc = pawn.def.GetModExtension<DefModExt_RenderWeaponExt>().renderPosOffset;
            }
            if (offsetLoc != null)
            {
                if (pawn.Rotation == Rot4.South)
                {
                    rootLoc.x += offsetLoc.x;
                    rootLoc.z += offsetLoc.y;
                }
                else if (pawn.Rotation == Rot4.North)
                {
                    rootLoc.x -= offsetLoc.x;
                    rootLoc.z -= offsetLoc.y;
                }
                else if (pawn.Rotation == Rot4.East)
                {
                    rootLoc.x -= offsetLoc.y;
                    rootLoc.z += offsetLoc.x;
                }
                else if (pawn.Rotation == Rot4.West)
                {
                    rootLoc.x += offsetLoc.y;
                    rootLoc.z -= offsetLoc.x;
                }
            }
            if (pawn.def.HasModExtension<DefModExt_RenderWeaponExt>() && pawn.def.GetModExtension<DefModExt_RenderWeaponExt>().renderOverVehicle)
            {
                rootLoc.y += 1.0f;
            }
        }
    }
}
