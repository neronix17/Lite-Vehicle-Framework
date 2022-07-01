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
    public static class CompatibilityUtil
    {
        public static bool GizmoCompatability_Prefix(Pawn pawn)
        {
            if (pawn.def.HasComp(typeof(Comp_Vehicle)))
            {
                return false;
            }
            return true;
        }
    }
}
