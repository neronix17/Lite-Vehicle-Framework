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
    [HarmonyPatch(typeof(Dialog_FormCaravan), "CheckForErrors")]
    public static class Patch_Dialog_FormCaravan_CheckForErrors
    {
        [HarmonyPrefix]
        public static bool Prefix(Dialog_FormCaravan __instance, List<Pawn> pawns, ref bool __result)
        {
            if (pawns.FindAll(x => x.GetComp<Comp_Vehicle>() != null) is List<Pawn> vehicles)
            {
                var localReform = Traverse.Create(__instance).Field("reform").GetValue<bool>();
                if (!localReform && Traverse.Create(__instance).Field("startingTile").GetValue<int>() < 0)
                {
                    Messages.Message("NoExitDirectionForCaravanChosen".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
                if (!pawns.Any(x => CaravanUtility.IsOwner(x, Faction.OfPlayer) && !x.Downed))
                {
                    if (!pawns.Any(y => y.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v))
                    {
                        if (!pawns.Any(z => z.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v && v.CanMove))
                        {
                            Messages.Message("CaravanMustHaveAtLeastOneColonist".Translate(), MessageTypeDefOf.RejectInput);
                            return false;
                        }
                    }
                }
                if (!localReform && Traverse.Create(__instance).Property("MassUsage").GetValue<float>() > Traverse.Create(__instance).Property("MassCapacity").GetValue<float>())
                {
                    AccessTools.Method(typeof(Dialog_FormCaravan), "FlashMass").Invoke(__instance, null);
                    Messages.Message("TooBigCaravanMassUsage".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
                var pawn = pawns.Find(x => (x.TryGetComp<Comp_Vehicle>() == null || x.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v && !v.CanMove) && !pawns.Any(y => !y.IsPrisoner && !y.RaceProps.Animal && y.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, false, TraverseMode.ByPawn)));
                if (pawn != null)
                {
                    Messages.Message("CaravanPawnIsUnreachable".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput);
                    return false;
                }
                for (var i = 0; i < __instance.transferables.Count; i++)
                {
                    if (__instance.transferables[i].ThingDef.category == ThingCategory.Item)
                    {
                        var countToTransfer = __instance.transferables[i].CountToTransfer;
                        var num = 0;
                        if (countToTransfer > 0)
                        {
                            for (var j = 0; j < __instance.transferables[i].things.Count; j++)
                            {
                                var t = __instance.transferables[i].things[j];
                                if (!t.Spawned || pawns.Any(x =>
                                        (x.IsColonist || x.TryGetComp<Comp_Vehicle>() is Comp_Vehicle v && v.CanMove) && x.CanReach(t, PathEndMode.Touch, Danger.Deadly, false, false, TraverseMode.ByPawn)))
                                {
                                    num += t.stackCount;
                                    if (num >= countToTransfer)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (num < countToTransfer)
                            {
                                if (countToTransfer == 1)
                                {
                                    Messages.Message("CaravanItemIsUnreachableSingle".Translate(__instance.transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput);
                                }
                                else
                                {
                                    Messages.Message("CaravanItemIsUnreachableMulti".Translate(countToTransfer, __instance.transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput);
                                }
                            }
                        }
                    }
                }
                __result = true;
                return false;
            }
            return true;
        }
    }
}
