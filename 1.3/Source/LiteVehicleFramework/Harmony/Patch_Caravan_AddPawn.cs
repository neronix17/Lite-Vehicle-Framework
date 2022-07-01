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
    [HarmonyPatch(typeof(Caravan), "AddPawn")]
    public static class Patch_Caravan_AddPawn
    {
        [HarmonyPrefix]
        public static bool Prefix(Caravan __instance, Pawn p, bool addCarriedPawnToWorldPawnsIfAny)
        {
            if (p?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle)
            {
                if (p == null || p.Dead)
                {
                    LogUtil.LogWarning("Tried to add a null or dead pawn to " + __instance);
                    return false;
                }
                if (p.Spawned) 
                { 
                    p.DeSpawn(); 
                }
                if (p.holdingOwner != null)
                {
                    p.holdingOwner?.TryTransferToContainer(p, __instance.pawns);
                }
                else
                {
                    __instance.pawns.TryAdd(p);
                }

                if (compVehicle.handlers != null && compVehicle.handlers.Count > 0)
                {
                    compVehicle.PawnsInVehicle = new List<VehicleHandlerTemp>();
                    foreach (var group in compVehicle.handlers)
                    {
                        compVehicle.PawnsInVehicle.Add(new VehicleHandlerTemp(group));

                        for (var i = 0; i < group.handlers.Count; i++)
                        {
                            var innerPawn = group.handlers[i];

                            if (innerPawn.Spawned)
                            {
                                innerPawn.DeSpawn();
                            }
                            if (innerPawn.holdingOwner != null)
                            {
                                innerPawn?.holdingOwner?.TryTransferToContainer(innerPawn, __instance.pawns);
                            }
                            else
                            {
                                if (!__instance.pawns.TryAdd(innerPawn, true))
                                {
                                    LogUtil.LogMessage("Failed to load caravan with vehicle pawn: " + innerPawn.Label);
                                }
                            }
                        }
                    }
                    var pawn = p.carryTracker.CarriedThing as Pawn;
                    if (pawn != null)
                    {
                        p.carryTracker.innerContainer.Remove(pawn);
                        __instance.AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny);
                        if (addCarriedPawnToWorldPawnsIfAny)
                        {
                            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                        }
                    }
                }
                else
                {
                    LogUtil.LogError("Couldn't add pawn " + p + " to caravan.");
                }
                return false;
            }
            return true;
        }
    }
}
