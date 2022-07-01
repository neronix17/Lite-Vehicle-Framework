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
    [HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove", new Type[] { typeof(List<Pawn>), typeof(float), typeof(float), typeof(StringBuilder) })]
    public static class Patch_CaravanTicksPerMoveUtility_GetTicksPerMove
    {
        [HarmonyPostfix]
        public static void Postfix(List<Pawn> pawns, ref int __result)
        {
            if (!pawns.NullOrEmpty() && pawns.FindAll(x => x?.GetComp<Comp_Vehicle>() != null && !x.Dead && !x.Downed) is List<Pawn> vehicles && vehicles.Count > 0)
            {
                var pawnsOutsideVehicle = new List<Pawn>(pawns.FindAll(x => x?.GetComp<Comp_Vehicle>() == null));
                if (pawnsOutsideVehicle != null && pawnsOutsideVehicle.Count > 0)
                {
                    if ((vehicles?.Count ?? 0) > 0)
                    {
                        foreach (var vehicle in vehicles)
                        {
                            if ((vehicle?.GetComp<Comp_Vehicle>().PawnsInVehicle?.Count ?? 0) > 0)
                                foreach (var group in vehicle?.GetComp<Comp_Vehicle>().PawnsInVehicle)
                                {
                                    if ((group?.handlers?.Count ?? 0) > 0)
                                    {
                                        foreach (var p in group.handlers)
                                        {
                                            if (pawnsOutsideVehicle.Count == 0)
                                            {
                                                break;
                                            }
                                            if (pawnsOutsideVehicle.Contains(p))
                                            {
                                                pawnsOutsideVehicle.Remove(p);
                                            }
                                        }
                                    }
                                    if (pawnsOutsideVehicle.Count == 0)
                                    {
                                        break;
                                    }
                                }
                            if (pawnsOutsideVehicle.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }

                if ((pawnsOutsideVehicle?.Count ?? 0) > 0)
                {
                    return;
                }

                var slowestLandSpeed = 999f;
                foreach (var vehicle in vehicles)
                {
                    slowestLandSpeed = Math.Min(vehicle.GetComp<Comp_Vehicle>().Props.worldSpeedFactor, slowestLandSpeed);
                }
                __result = (int)(__result / slowestLandSpeed);
            }
        }
    }
}
