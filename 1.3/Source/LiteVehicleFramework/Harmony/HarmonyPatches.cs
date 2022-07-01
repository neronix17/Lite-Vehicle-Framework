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
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {

            LiteVehiclesMod.harmony.Patch(typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(HarmonyPatch), nameof(MechanoidsFixer)));

            LiteVehiclesMod.harmony.Patch(typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(Harmony), nameof(MechanoidsFixerAncient)));

            LiteVehiclesMod.harmony.Patch(typeof(DamageWorker_AddInjury).GetMethods(bindingAttr: AccessTools.all).First(mi => mi.ReturnType == typeof(float) && mi.GetParameters().ElementAt(1).ParameterType == typeof(Hediff_Injury)), null, new HarmonyMethod(typeof(Harmony), nameof(TryInjureVehicleOccupants)));

            LiteVehiclesMod.harmony.Patch(typeof(CaravanTicksPerMoveUtility).GetMethods().First(mi => mi.GetParameters().ElementAt(0).ParameterType == typeof(List<Pawn>)), null, new HarmonyMethod(typeof(Harmony), nameof(GetTicksPerMove_Postfix)));

        }

        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (def.race.HasComp(typeof(Comp_Vehicle)))
            {
                __result = false;
            }
        }

        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (kind.race.HasComp(typeof(Comp_Vehicle)))
            {
                __result = false;
            }
        }
        public static void TryInjureVehicleOccupants(DamageWorker_AddInjury __instance, Pawn pawn, Hediff_Injury injury, DamageInfo dinfo, ref DamageWorker.DamageResult result, ref float __result)
        {
            Comp_Vehicle vehicleComp = pawn.GetComp<Comp_Vehicle>();
            if (vehicleComp != null)
            {
                List<Pawn> affectedPawns = new List<Pawn>();

                if (vehicleComp.handlers != null && vehicleComp.handlers.Count > 0)
                {
                    foreach (VehicleHandlerGroup group in vehicleComp.handlers)
                    {
                        if (group.occupiedParts != null && group.handlers != null && group.handlers.Count > 0)
                        {
                            if (group.occupiedParts.Contains(injury.Part))
                            {
                                affectedPawns.AddRange(group.handlers);
                            }
                        }
                    }
                }

                if (affectedPawns != null && affectedPawns.Count > 0)
                {
                    DamageInfo newDamageInfo = new DamageInfo(dinfo);
                    float criticalBonus = 0f;
                    if (Rand.Value < vehicleComp.Props.seatHitCriticalHitChance) criticalBonus = dinfo.Amount * 2;
                    float newDamFloat = dinfo.Amount * vehicleComp.Props.seatHitDamageFactor + criticalBonus;
                    newDamageInfo.SetAmount((int)newDamFloat);
                    affectedPawns.RandomElement().TakeDamage(newDamageInfo);
                }
            }
        }

        public static void GetTicksPerMove_Postfix(List<Pawn> pawns, ref int __result)
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
