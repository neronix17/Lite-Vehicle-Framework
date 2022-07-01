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
    [HarmonyPatch(typeof(Caravan_NeedsTracker), "TrySatisfyPawnNeeds")]
    public static class Patch_Caravan_NeedsTracker_TrySatisfyPawnNeeds
    {
        [HarmonyPrefix]
        public static bool Prefix(Caravan_NeedsTracker __instance, Pawn pawn)
        {
            CompRefuelable fuelComp = pawn.GetComp<CompRefuelable>();
            Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
            if (pawn.Dead || fuelComp == null || vehicleComp == null)
            {
                return true;
            }

            if (fuelComp.FuelPercentOfMax < fuelComp.Props.autoRefuelPercent)
            {
                int num = fuelComp.GetFuelCountToFullyRefuel();
                if (TryGetFuel(__instance.caravan, pawn, fuelComp, out var thing, out var pawn2))
                {
                    fuelComp.Refuel(num);

                    if (thing.stackCount < num)
                    {
                        num = thing.stackCount;
                    }
                    thing.SplitOff(num);
                    if (thing.Destroyed)
                    {
                        if (pawn2 != null)
                        {
                            pawn2.inventory.innerContainer.Remove(thing);
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryGetFuel(Caravan caravan, Pawn forPawn, CompRefuelable refuelable, out Thing fuel, out Pawn owner)
        {
            var list = CaravanInventoryUtility.AllInventoryItems(caravan);

            var filter = refuelable.Props.fuelFilter;
            for (var i = 0; i < list.Count; i++)
            {
                var thing2 = list[i];
                if (filter.Allows(thing2))
                {
                    fuel = thing2;
                    owner = CaravanInventoryUtility.GetOwnerOf(caravan, thing2);
                    forPawn.GetComp<Comp_Vehicle>().WarnedOnNoFuel = false;
                    return true;
                }
            }

            fuel = null;
            owner = null;

            if (!forPawn.GetComp<CompRefuelable>().HasFuel)
            {
                if (forPawn.GetComp<Comp_Vehicle>().WarnedOnNoFuel == false)
                {
                    Messages.Message("MessageCaravanRunOutOfFuel".Translate(caravan.LabelCap, forPawn.Label), caravan, MessageTypeDefOf.ThreatBig);
                    forPawn.GetComp<Comp_Vehicle>().WarnedOnNoFuel = true;
                }
            }

            return false;
        }
    }
}
