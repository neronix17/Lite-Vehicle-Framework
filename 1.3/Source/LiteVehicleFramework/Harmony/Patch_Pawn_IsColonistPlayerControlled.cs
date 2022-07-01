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
    [HarmonyPatch(typeof(Pawn), "IsColonistPlayerControlled", MethodType.Getter)]
    public static class Patch_Pawn_IsColonistPlayerControlled
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn __instance, ref bool __result)
        {
            Comp_Vehicle vehicleComp = __instance.GetComp<Comp_Vehicle>();
            if (vehicleComp != null)
            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    __result = true;
                }
            }
        }
    }
}
