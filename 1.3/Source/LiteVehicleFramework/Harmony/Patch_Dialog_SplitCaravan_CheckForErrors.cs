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
using RimWorld.Planet;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(Dialog_SplitCaravan), "CheckForErrors")]
    public static class Patch_Dialog_SplitCaravan_CheckForErrors
    {
        [HarmonyPrefix]
        public static bool Prefix(Dialog_SplitCaravan __instance, List<Pawn> pawns, ref bool __result)
        {
            var thisCaravan = Traverse.Create(__instance).Field("caravan").GetValue<Caravan>();
            if (thisCaravan?.pawns?.InnerListForReading?.Any(x => x.GetComp<Comp_Vehicle>() != null) ?? false)
            {
                var result = __result;
                if ((!thisCaravan?.PawnsListForReading?.NullOrEmpty() ?? false) && !pawns.NullOrEmpty())
                {
                    foreach (var p in thisCaravan.PawnsListForReading)
                    {
                        if (p.GetComp<Comp_Vehicle>() is Comp_Vehicle vehicleComp && vehicleComp?.PawnsInVehicle is List<VehicleHandlerTemp> vehicleHandlers && !vehicleHandlers.NullOrEmpty())
                        {
                            foreach (var temp in vehicleHandlers)
                            {
                                foreach (var o in pawns)
                                {
                                    if (temp?.handlers?.Contains(o) ?? false)
                                    {
                                        Messages.Message("CompVehicle_CannotSplitWhileInVehicles".Translate(),
                                            MessageTypeDefOf.RejectInput);
                                        __result = false;
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
