using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

using Harmony;

namespace LiteVehicles
{
    [StaticConstructorOnStartup]
    internal static class Harmony
    {
        //S Temporary prefix patching solution until a transpiler is made
        //Patches Pawn_PathFollower to create custom pathing based off of a vehicles stats, i.e. boats can only be in water
        public static bool CostToMoveIntoCell_PreFix(IntVec3 c, ref int __result, Pawn_PathFollower __instance)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_PathFollower), "pawn").GetValue(__instance);
            if (pawn.GetComp<CompVehicle>() != null)
            {
                int num;
                if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                {
                    num = pawn.TicksPerMoveCardinal;
                }
                else
                {
                    num = pawn.TicksPerMoveDiagonal;
                }
                num += CalculatedCostAt(c, false, pawn.Position, __instance,
                    pawn.GetComp<CompVehicle>().Props.vehicleType, pawn);
                Building edifice = c.GetEdifice(pawn.Map);
                if (edifice != null)
                {
                    num += (int)edifice.PathWalkCostFor(pawn);
                }
                if (num > 450)
                {
                    num = 450;
                }
                if (pawn.jobs.curJob != null)
                {
                    switch (pawn.jobs.curJob.locomotionUrgency)
                    {
                        case LocomotionUrgency.Amble:
                            num *= 3;
                            if (num < 60)
                            {
                                num = 60;
                            }
                            break;
                        case LocomotionUrgency.Walk:
                            num *= 2;
                            if (num < 50)
                            {
                                num = 50;
                            }
                            break;
                        case LocomotionUrgency.Jog:
                            break;
                        case LocomotionUrgency.Sprint:
                            num = Mathf.RoundToInt((float)num * 0.75f);
                            break;
                    }
                }
                __result = Mathf.Max(num, 1);
                return false;
            }

            return true;
        }

        //J If needed drivers are absent, Vehicles don't move.
        // Verse.AI.Pawn_PathFollower
        public static bool CanVehicleMove(Pawn_PathFollower __instance, LocalTargetInfo dest, PathEndMode peMode)
        {
            var pawn = (Pawn)AccessTools.Field(typeof(Pawn_PathFollower), "pawn").GetValue(__instance);
            if (pawn != null)
            {
                var compPilotable = pawn.GetComp<CompVehicle>();
                if (compPilotable != null)
                    if (compPilotable.movingStatus == MovingState.frozen) return false;
            }
            return true;
        }

        private static object tab;

        // RimWorld.Planet.CaravanMaker
        public static bool MakeCaravan_PreFix(ref Caravan __result, IEnumerable<Pawn> pawns, Faction faction,
            int startingTile, bool addToWorldPawnsIfNotAlready)
        {
            ////LogUtil.LogMessage("MC1");
            if (startingTile < 0 && addToWorldPawnsIfNotAlready)
                LogUtil.LogWarning(
                    "Tried to create a caravan but chose not to spawn a caravan but pass pawns to world. This can cause bugs because pawns can be discarded.");

            ////LogUtil.LogMessage("MC2");

            var tmpPawns = (List<Pawn>) AccessTools.Field(typeof(CaravanMaker), "tmpPawns").GetValue(null);

            tmpPawns.Clear();
            tmpPawns.AddRange(pawns);
            ////LogUtil.LogMessage("MC3");

            var caravan = (Caravan) WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Caravan);
            ////LogUtil.LogMessage("MC4");

            if (startingTile >= 0)
            {
                ////LogUtil.LogMessage("MC5a");

                caravan.Tile = startingTile;
                ////LogUtil.LogMessage("MC5b");
            }
            ////LogUtil.LogMessage("MC6");

            caravan.SetFaction(faction);
            ////LogUtil.LogMessage("MC7");

            caravan.Name = CaravanNameGenerator.GenerateCaravanName(caravan);
            ////LogUtil.LogMessage("MC8");

            if (startingTile >= 0)
            {
                ////LogUtil.LogMessage("MC9a");

                Find.WorldObjects.Add(caravan);
                ////LogUtil.LogMessage("MC9b");
            }
            for (var i = 0; i < tmpPawns.Count; i++)
            {
                ////LogUtil.LogMessage("MC10a");

                var pawn = tmpPawns[i];
                if (pawn.Spawned)
                {
                    //LogUtil.LogMessage("MC11a");

                    pawn.DeSpawn();
                    //LogUtil.LogMessage("MC11b");
                }
                if (pawn.Dead)
                {
                    LogUtil.LogWarning("Tried to form a caravan with a dead pawn " + pawn);
                }
                else
                {
                    //LogUtil.LogMessage("MC12a");

                    caravan.AddPawn(pawn, addToWorldPawnsIfNotAlready);
                    //LogUtil.LogMessage("MC12b");

                    if (addToWorldPawnsIfNotAlready && !pawn.IsWorldPawn())
                    {
                        //LogUtil.LogMessage("MC13a");

                        if (pawn.Spawned)
                            pawn.DeSpawn();
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                        //LogUtil.LogMessage("MC13b");
                    }
                    //LogUtil.LogMessage("MC12c");
                }
            }
            __result = caravan;
            return false;
        }


        private static int CalculatedCostAt(IntVec3 c, bool perceivedStatic, IntVec3 prevCell,
            Pawn_PathFollower __instance, VehicleType type, Pawn pawn)
        {
            int num = 0;
            TerrainDef terrainDef = pawn.Map.terrainGrid.TerrainAt(c);
            if (type == VehicleType.Sea || type == VehicleType.SeaSubmarine)
            {
                if (terrainDef == null || !terrainDef.defName.Contains("Water"))
                {
                    num = 10000;
                }
                else
                {
                    num += 1;
                }
            }
            else if (type == VehicleType.Amphibious)
            {
                if (terrainDef == null || (terrainDef.passability == Traversability.Impassable &&
                                           !terrainDef.defName.Contains("Water")))
                {
                    num = 10000;
                }
                else
                {
                    num += 1;
                }
            }
            else
            {
                if (terrainDef == null || terrainDef.passability == Traversability.Impassable)
                {
                    num = 10000;
                }
                else
                {
                    num += terrainDef.pathCost;
                }
            }


            int num2 = SnowUtility.MovementTicksAddOn(pawn.Map.snowGrid.GetCategory(c));
            num += num2;
            List<Thing> list = pawn.Map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (thing.def.passability == Traversability.Impassable)
                {
                    return 10000;
                }
                if (!IsPathCostIgnoreRepeater(thing.def) || !prevCell.IsValid ||
                    !ContainsPathCostIgnoreRepeater(prevCell, pawn))
                {
                    num += thing.def.pathCost;
                }
                if (prevCell.IsValid && thing is Building_Door)
                {
                    Building edifice = prevCell.GetEdifice(pawn.Map);
                    if (edifice != null && edifice is Building_Door)
                    {
                        num += 45;
                    }
                }
            }
            if (perceivedStatic)
            {
                for (int j = 0; j < 9; j++)
                {
                    IntVec3 b = GenAdj.AdjacentCellsAndInside[j];
                    IntVec3 c2 = c + b;
                    if (c2.InBounds(pawn.Map))
                    {
                        Fire fire = null;
                        list = pawn.Map.thingGrid.ThingsListAtFast(c2);
                        for (int k = 0; k < list.Count; k++)
                        {
                            fire = (list[k] as Fire);
                            if (fire != null)
                            {
                                break;
                            }
                        }
                        if (fire != null && fire.parent == null)
                        {
                            if (b.x == 0 && b.z == 0)
                            {
                                num += 1000;
                            }
                            else
                            {
                                num += 150;
                            }
                        }
                    }
                }
            }
            return num;
        }

        private static bool IsPathCostIgnoreRepeater(ThingDef def)
        {
            return def.pathCost >= 25 && def.pathCostIgnoreRepeat;
        }

        private static bool ContainsPathCostIgnoreRepeater(IntVec3 c, Pawn pawn)
        {
            List<Thing> list = pawn.Map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                if (IsPathCostIgnoreRepeater(list[i].def))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TryGetFuel(Caravan caravan, Pawn forPawn, CompRefuelable refuelable, out Thing fuel,
            out Pawn owner)
        {
            //Find fuel for the vehicle by looking through all items
            var list = CaravanInventoryUtility.AllInventoryItems(caravan);

            //Get acceptable fuel items
            var filter = refuelable.Props.fuelFilter;
            for (var i = 0; i < list.Count; i++)
            {
                //If the thing found is an acceptable fuel source
                var thing2 = list[i];
                if (filter.Allows(thing2))
                {
                    fuel = thing2;
                    owner = CaravanInventoryUtility.GetOwnerOf(caravan, thing2);
                    //Reset the spammer preventer since the vehicle now has fuel
                    forPawn.GetComp<CompVehicle>().WarnedOnNoFuel = false;
                    return true;
                }
            }
            fuel = null;
            owner = null;
            //Couldn't find a fuel source, check if vehicle is out of fuel
            if (!forPawn.GetComp<CompRefuelable>().HasFuel)
                if (forPawn.GetComp<CompVehicle>().WarnedOnNoFuel == false)
                {
                    //Notify player that caravan is out of fuel
                    Messages.Message("MessageCaravanRunOutOfFuel".Translate(caravan.LabelCap, forPawn.Label), caravan,
                        MessageTypeDefOf.ThreatBig);
                    //No more spam
                    forPawn.GetComp<CompVehicle>().WarnedOnNoFuel = true;
                }
            return false;
        }

        private static bool AnythingOutOfFuel(Caravan caravan)
        {
            if (AnythingNeedsFuel(caravan, out var needfuel))
                if (needfuel != null)
                    for (var i = 0; i < needfuel.Count; i++)
                        if (!needfuel[i].GetComp<CompRefuelable>().HasFuel)
                            return true;
            return false;
        }

        private static bool AnythingNeedsFuel(Caravan caravan, out List<Pawn> needfuel)
        {
            var pawns = caravan.PawnsListForReading;
            needfuel = new List<Pawn>();
            if (pawns != null)
                for (var i = 0; i < pawns.Count; i++)
                    if (pawns[i].GetComp<CompRefuelable>() != null)
                        needfuel.Add(pawns[i]);
            if (needfuel.Count > 0)
                return true;
            return false;
        }

        private static float ApproxDaysWorthOfFuel(List<Pawn> pawns, IEnumerable<Thing> goods)
        {
            var supplies = 0f;
            float totalFuelUse = 0;
            var allowed = new List<ThingDef>();
            for (var i = 0; i < pawns.Count; i++)
            {
                var refuel = pawns[i].GetComp<CompRefuelable>();
                foreach (var thing in refuel.Props.fuelFilter.AllowedThingDefs)
                    if (!allowed.Contains(thing))
                        allowed.Add(thing);
                totalFuelUse += refuel.Props.fuelConsumptionRate / 60000;
                supplies += refuel.FuelPercentOfMax;
            }
            foreach (var item in goods)
                if (allowed.Contains(item.def))
                    supplies += item.stackCount;

            if (Math.Abs(totalFuelUse) > double.Epsilon)
                return supplies / (GenDate.TicksPerDay * totalFuelUse);
            return 10000;
        }

        private static void DrawOnGUI(CompRefuelable fuel_comp, Rect rect, bool doTooltip,
            int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true)
        {
            //Code is modified from the DrawOnGui method for Needs
            var CurLevelPercentage = fuel_comp.FuelPercentOfMax;
            if (rect.height > 70f)
            {
                var num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            if (Mouse.IsOver(rect))
                Widgets.DrawHighlight(rect);
            if (doTooltip)
                TooltipHandler.TipRegion(rect, new TipSignal(() => GetTipString(fuel_comp), rect.GetHashCode()));
            var num2 = 14f;
            var num3 = customMargin < 0f ? num2 + 15f : customMargin;
            if (rect.height < 50f)
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            Text.Font = rect.height <= 55f ? GameFont.Tiny : GameFont.Small;
            Text.Anchor = TextAnchor.LowerLeft;
            var rect2 = new Rect(rect.x + num3 + rect.width * 0.1f, rect.y, rect.width - num3 - rect.width * 0.1f,
                rect.height / 2f);
            Widgets.Label(rect2, "Fuel");
            Text.Anchor = TextAnchor.UpperLeft;
            var rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);
            rect3 = new Rect(rect3.x + num3, rect3.y, rect3.width - num3 * 2f, rect3.height - num2);
            //Fill the rectangle up to the current level of fuel
            Widgets.FillableBar(rect3, CurLevelPercentage);
            if (drawArrows)
                Widgets.FillableBarChangeArrows(rect3, 0);
            var curInstantLevelPercentage = CurLevelPercentage;
            if (curInstantLevelPercentage >= 0f)
                DrawBarInstantMarkerAt(rect3, curInstantLevelPercentage);
            Text.Font = GameFont.Small;
        }

        private static string GetTipString(CompRefuelable refuel)
        {
            return string.Concat(new[]
            {
                "Fuel: ",
                refuel.FuelPercentOfMax.ToStringPercent(),
                "\n",
                "Fuel is necessary for vehicles and other machines to operate."
            });
        }

        private static void DrawBarInstantMarkerAt(Rect barRect, float pct)
        {
            var num = 12f;
            if (barRect.width < 150f)
                num /= 2f;
            var vector = new Vector2(barRect.x + barRect.width * pct, barRect.y + barRect.height);
            var position = new Rect(vector.x - num / 2f, vector.y, num, num);
            GUI.DrawTexture(position, ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarker", true));
        }

        private static void DrawConfig(Rect rect, Traverse traverseobj)
        {
            var ExitDirectionRadioSize = traverseobj.Field("ExitDirectionRadioSize").GetValue<Vector2>();
            var map = traverseobj.Field("map").GetValue<Map>();
            var CurrentTile = traverseobj.Property("CurrentTile").GetValue<int>();
            var startingTile = traverseobj.Field("startingTile").GetValue<int>();
            var rect2 = new Rect(0f, rect.y, rect.width, 30f);
            Text.Font = GameFont.Medium;
            Widgets.Label(rect2, "ExitDirection".Translate());
            Text.Font = GameFont.Small;
            var list = CaravanExitMapUtility.AvailableExitTilesAt(map);
            if (list.Any())
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var direction8WayFromTo = Find.WorldGrid.GetDirection8WayFromTo(CurrentTile, list[i]);
                    var y = rect.y + i * ExitDirectionRadioSize.y + 30f + 4f;
                    var rect3 = new Rect(rect.x, y, ExitDirectionRadioSize.x, ExitDirectionRadioSize.y);
                    var vector = Find.WorldGrid.LongLatOf(list[i]);
                    var labelText = "ExitDirectionRadioButtonLabel".Translate(direction8WayFromTo.LabelShort(),
                        vector.y.ToStringLatitude(), vector.x.ToStringLongitude());
                    if (Widgets.RadioButtonLabeled(rect3, labelText, startingTile == list[i]))
                        startingTile = list[i];
                }
            }
            else
            {
                GUI.color = Color.gray;
                Widgets.Label(new Rect(rect.x, rect.y + 30f + 4f, rect.width, 100f),
                    "NoCaravanExitDirectionAvailable".Translate());
                GUI.color = Color.white;
            }
        }

        private static void DoBottomButtons(Rect rect, Dialog_FormCaravan instance, Pair<float, float> DaysWorthOfFood,
            Traverse traverseobj, bool reform, List<TransferableOneWay> transferables, float DaysWorthOfFuel,
            bool StuffHasNoFuel)
        {
            //traverse object was passed along, grab more private variables
            var BottomButtonSize = traverseobj.Field("BottomButtonSize").GetValue<Vector2>();
            var rect2 = new Rect(rect.width / 2f - BottomButtonSize.x / 2f, rect.height - 55f, BottomButtonSize.x,
                BottomButtonSize.y);
            var MostFoodWillRotSoon = traverseobj.Property("MostFoodWillRotSoon").GetValue<bool>();
            var showEstTimeToDestinationButton = traverseobj.Field("showEstTimeToDestinationButton").GetValue<bool>();

            //If they pressed Accept
            if (Widgets.ButtonText(rect2, "AcceptButton".Translate(), true, false, true))
                if (reform)
                {
                    if ((bool) traverseobj.Method("TryReformCaravan").GetValue(new object[] { }))
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        instance.Close(false);
                        tab = traverseobj.Field("tab").GetValue();
                    }
                }
                else
                {
                    string text = null;
                    var daysWorthOfFood = DaysWorthOfFood;
                    if (daysWorthOfFood.First < 5f)
                        text = daysWorthOfFood.First >= 0.1f
                            ? "DaysWorthOfFoodWarningDialog".Translate(daysWorthOfFood.First.ToString("0.#"))
                            : "DaysWorthOfFoodWarningDialog_NoFood".Translate();
                    else if (MostFoodWillRotSoon)
                        text = "CaravanFoodWillRotSoonWarningDialog".Translate();
                    else if (DaysWorthOfFuel < 5f)
                        text = DaysWorthOfFuel >= 0.1f
                            ? "DaysWorthOfFuelWarningDialog".Translate(DaysWorthOfFuel.ToString("0.#"))
                            : "DaysWorthOfFuelWarningDialog_NoFuel".Translate();

                    if (!text.NullOrEmpty())
                    {
                        if ((bool) AccessTools.Method(typeof(Dialog_FormCaravan), "CheckForErrors").Invoke(instance,
                            new object[] {TransferableUtility.GetPawnsFromTransferables(transferables)}))
                            if (StuffHasNoFuel)
                                Messages.Message("CaravanVehicleNoFuelWarningDialog".Translate(),
                                    MessageTypeDefOf.RejectInput);
                            else
                                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
                                {
                                    if ((bool) AccessTools.Method(typeof(Dialog_FormCaravan), "TryFormAndSendCaravan")
                                        .Invoke(instance, new object[] { }))
                                    {
                                        instance.Close(false);
                                        tab = traverseobj.Field("tab").GetValue();
                                    }
                                }, false, null));
                    }
                    else if ((bool) AccessTools.Method(typeof(Dialog_FormCaravan), "TryFormAndSendCaravan")
                        .Invoke(instance, new object[] { }))
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        instance.Close(false);
                        tab = traverseobj.Field("tab").GetValue();
                    }
                }
            var rect3 = new Rect(rect2.x - 10f - BottomButtonSize.x, rect2.y, BottomButtonSize.x, BottomButtonSize.y);
            if (Widgets.ButtonText(rect3, "ResetButton".Translate(), true, false, true))
            {
                SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                AccessTools.Method(typeof(Dialog_FormCaravan), "CalculateAndRecacheTransferables")
                    .Invoke(instance, new object[] { });
            }
            var rect4 = new Rect(rect2.xMax + 10f, rect2.y, BottomButtonSize.x, BottomButtonSize.y);
            if (Widgets.ButtonText(rect4, "CancelButton".Translate(), true, false, true))
            {
                instance.Close(true);
                tab = traverseobj.Field("tab").GetValue();
            }
            if (showEstTimeToDestinationButton)
            {
                var rect5 = new Rect(rect.width - BottomButtonSize.x, rect2.y, BottomButtonSize.x, BottomButtonSize.y);
                if (Widgets.ButtonText(rect5, "EstimatedTimeToDestinationButton".Translate(), true, false, true))
                {
                    var pawnsFromTransferables = TransferableUtility.GetPawnsFromTransferables(transferables);
                    if (!pawnsFromTransferables.Any(x =>
                        CaravanUtility.IsOwner(x, Faction.OfPlayer) && !x.Downed ||
                        x.GetComp<CompVehicle>() is CompVehicle))
                        Messages.Message("CaravanMustHaveAtLeastOneColonist".Translate(), MessageTypeDefOf.RejectInput);
                    else
                        Find.WorldRoutePlanner.Start(instance);
                }
            }
            if (Prefs.DevMode)
            {
                var width = 200f;
                var num = BottomButtonSize.y / 2f;
                var rect6 = new Rect(0f, rect.height - 55f, width, num);
                if (Widgets.ButtonText(rect6, "Dev: Send instantly", true, false, true) && (bool) AccessTools
                        .Method(typeof(Dialog_FormCaravan), "DebugTryFormCaravanInstantly")
                        .Invoke(instance, new object[] { }))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                    instance.Close(false);
                    tab = traverseobj.Field("tab").GetValue();
                }
                var rect7 = new Rect(0f, rect.height - 55f + num, width, num);
                if (Widgets.ButtonText(rect7, "Dev: Select everything", true, false, true))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                    AccessTools.Method(typeof(Dialog_FormCaravan), "SetToSendEverything")
                        .Invoke(instance, new object[] { });
                }
            }
        }

        private static bool StuffHasNoFuel(List<TransferableOneWay> transferables,
            IgnorePawnsInventoryMode ignoreInventory)
        {
            var needsfuel = new List<Pawn>();
            for (var i = 0; i < transferables.Count; i++)
            {
                var transferableOneWay = transferables[i];
                if (transferableOneWay.HasAnyThing)
                    if (transferableOneWay.AnyThing is Pawn)
                        for (var l = 0; l < transferableOneWay.CountToTransfer; l++)
                        {
                            //Get a list of pawns that need fuel
                            var pawn = (Pawn) transferableOneWay.things[l];
                            if (pawn.GetComp<CompRefuelable>() != null)
                                needsfuel.Add(pawn);
                        }
            }
            for (var i = 0; i < needsfuel.Count; i++)
                //If it needs fuel and doesn't have fuel the caravan shouldn't form since the pawn can't move
                if (!needsfuel[i].GetComp<CompRefuelable>().HasFuel)
                    return true;
            return false;
        }
    }
}