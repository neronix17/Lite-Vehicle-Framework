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
    [HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
    public static class Patch_HealthCardUtility_DrawOverviewTab
    {
        [HarmonyPrefix]
        public static bool Prefix(Rect leftRect, Pawn pawn, float curY, ref float __result)
        {
            if (pawn != null)
            {
                Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
                if (vehicleComp != null)
                {
                    curY += 4f;

                    if (vehicleComp.Props.movementHandling > HandlingType.Incapable)
                    {
                        //Movement Systems: Online

                        Text.Font = GameFont.Tiny;
                        Text.Anchor = TextAnchor.UpperLeft;
                        GUI.color = new Color(0.9f, 0.9f, 0.9f);
                        var text = StringUtil.Movement;
                        if (vehicleComp.movingStatus == MovingState.able)
                            text = text + ": " + StringUtil.On;
                        else
                            text = text + ": " + StringUtil.Off;
                        var rect = new Rect(0f, curY, leftRect.width, 34f);
                        Widgets.Label(rect, text.CapitalizeFirst());
                    }

                    if (vehicleComp.Props.manipulationHandling > HandlingType.Incapable)
                    {
                        //Manipulation Systems: Online

                        curY += 34f;
                        Text.Font = GameFont.Tiny;
                        Text.Anchor = TextAnchor.UpperLeft;
                        GUI.color = new Color(0.9f, 0.9f, 0.9f);
                        var textM = StringUtil.Manipulation;
                        if (vehicleComp.manipulationStatus == ManipulationState.able)
                            textM = textM + ": " + StringUtil.On;
                        else
                            textM = textM + ": " + StringUtil.Off;
                        var rectM = new Rect(0f, curY, leftRect.width, 34f);
                        Widgets.Label(rectM, textM.CapitalizeFirst());
                    }

                    if (vehicleComp.Props.weaponHandling > HandlingType.Incapable)
                    {
                        //Weapons Systems: Online

                        curY += 34f;
                        Text.Font = GameFont.Tiny;
                        Text.Anchor = TextAnchor.UpperLeft;
                        GUI.color = new Color(0.9f, 0.9f, 0.9f);
                        var text2 = StringUtil.Weapons;
                        if (vehicleComp.weaponStatus == WeaponState.able)
                            text2 = text2 + ": " + StringUtil.On;
                        else
                            text2 = text2 + ": " + StringUtil.Off;
                        var rect2 = new Rect(0f, curY, leftRect.width, 34f);
                        Widgets.Label(rect2, text2.CapitalizeFirst());
                    }
                    curY += 34f;
                    __result = curY;
                    return false;
                }
            }
            return true;
        }
    }
}
