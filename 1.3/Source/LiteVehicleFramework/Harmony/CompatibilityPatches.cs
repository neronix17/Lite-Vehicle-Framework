using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.BaseGen;
using System.Runtime.CompilerServices;
using System.Reflection;
using RimWorld.Planet;

namespace LiteVehicles
{
    [StaticConstructorOnStartup]
    public static class CompatibilityPatches
    {
        static CompatibilityPatches()
        {
            //if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "unlimitedhugs.allowtool"))
            //{
            //    LiteVehiclesMod.harmony.Patch(AccessTools.Method("AllowTool.PartyHuntHandler:TryGetGizmo"), new HarmonyMethod(typeof(Harmony), nameof(CompatibilityUtil.GizmoCompatability_Prefix)));
            //}
            //if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Ignite Everything"))
            //{
            //    LiteVehiclesMod.harmony.Patch(AccessTools.Method("IgniteEverything.Harmony_PawnAttackGizmoUtility:GetAttackGizmos_Postfix"), new HarmonyMethod(typeof(Harmony), nameof(CompatibilityUtil.GizmoCompatability_Prefix)));
            //}
        }

    }
}
