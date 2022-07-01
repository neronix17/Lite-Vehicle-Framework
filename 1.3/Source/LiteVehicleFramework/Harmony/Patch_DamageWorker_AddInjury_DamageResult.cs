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
    [HarmonyPatch(typeof(DamageWorker_AddInjury).GetMethods(bindingAttr: AccessTools.all).First(mi => mi.ReturnType == typeof(float) && mi.GetParameters().ElementAt(1).ParameterType == typeof(Hediff_Injury)))]
    public static class Patch_Pawn_IsColonistPlayerControlled
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__instance.def.HasComp(typeof(Comp_Vehicle)))
            {
                __result = __instance.Spawned && (__instance.Faction != null && __instance.Faction.IsPlayer) && __instance.MentalStateDef == null && __instance.HostFaction == null;
            }
        }
    }
}
