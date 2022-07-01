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
    [HarmonyPatch(typeof(Pawn_RotationTracker), "RotationTrackerTick")]
    public static class Patch_Pawn_RotationTracker_RotationTrackerTick
    {
        public static bool Prefix(Pawn_RotationTracker __instance)
        {
            if (Traverse.Create(__instance).Field("pawn").GetValue<Pawn>() is Pawn thisPawn &&
                thisPawn?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle)
            {
                if (thisPawn?.Destroyed ?? false)
                    return false;
                if (thisPawn?.jobs?.HandlingFacing ?? false)
                    return false;
                if (thisPawn?.pather?.Moving ?? false)
                {
                    if (thisPawn.pather.curPath == null || thisPawn.pather.curPath.NodesLeftCount < 1)
                        return false;
                    //this.FaceAdjacentCell(thisPawn.pather.nextCell);
                    AccessTools.Method(typeof(Pawn_RotationTracker), "FaceAdjacentCell")
                        .Invoke(__instance, new object[] { thisPawn.pather.nextCell });
                    //Traverse.Create(__instance).Method("FaceAdjacentCell", new object[] { thisPawn.pather.nextCell });
                    //compVehicle.lastDirection = thisPawn.Rotation;
                    return false;
                }
                var stance_Busy = thisPawn.stances.curStance as Stance_Busy;
                if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
                {
                    if (stance_Busy.focusTarg.HasThing)
                        AccessTools.Method(typeof(Pawn_RotationTracker), "Face").Invoke(__instance,
                            new object[] { stance_Busy.focusTarg.Thing.DrawPos });
                    else
                        AccessTools.Method(typeof(Pawn_RotationTracker), "FaceCell")
                            .Invoke(__instance, new object[] { stance_Busy.focusTarg.Cell });
                    return false;
                }
                if (thisPawn?.jobs?.curJob != null)
                {
                    var target = thisPawn.CurJob.GetTarget(thisPawn.jobs.curDriver.rotateToFace);
                    if (target.HasThing)
                    {
                        if (compVehicle.CanMove)
                        {
                            var flag = false;
                            var c = default(IntVec3);
                            var cellRect = target.Thing.OccupiedRect();
                            for (var i = cellRect.minZ; i <= cellRect.maxZ; i++)
                                for (var j = cellRect.minX; j <= cellRect.maxX; j++)
                                    if (thisPawn.Position == new IntVec3(j, 0, i))
                                    {
                                        //this.Face(target.Thing.DrawPos);
                                        //Traverse.Create(__instance).Method("Face", new object[] { target.Thing.DrawPos });
                                        AccessTools.Method(typeof(Pawn_RotationTracker), "Face")
                                            .Invoke(__instance, new object[] { target.Thing.DrawPos });
                                        return false;
                                    }
                            for (var k = cellRect.minZ; k <= cellRect.maxZ; k++)
                                for (var l = cellRect.minX; l <= cellRect.maxX; l++)
                                {
                                    var intVec = new IntVec3(l, 0, k);
                                    if (intVec.AdjacentToCardinal(thisPawn.Position))
                                    {
                                        //this.FaceAdjacentCell(intVec);
                                        AccessTools.Method(typeof(Pawn_RotationTracker), "FaceAdjacentCell")
                                            .Invoke(__instance, new object[] { intVec });
                                        //Traverse.Create(__instance).Method("FaceAdjacentCell", new object[] { intVec });
                                        return false;
                                    }
                                    if (intVec.AdjacentTo8Way(thisPawn.Position))
                                    {
                                        flag = true;
                                        c = intVec;
                                    }
                                }
                            if (flag)
                            {
                                if (DebugViewSettings.drawPawnRotatorTarget)
                                {
                                    thisPawn.Map.debugDrawer.FlashCell(thisPawn.Position, 0.6f, "jbthing");
                                    GenDraw.DrawLineBetween(thisPawn.Position.ToVector3Shifted(), c.ToVector3Shifted());
                                }
                                //this.FaceAdjacentCell(c);
                                AccessTools.Method(typeof(Pawn_RotationTracker), "FaceAdjacentCell")
                                    .Invoke(__instance, new object[] { c });
                                //Traverse.Create(__instance).Method("FaceAdjacentCell", new object[] { c });
                                return false;
                            }
                            AccessTools.Method(typeof(Pawn_RotationTracker), "Face")
                                .Invoke(__instance, new object[] { target.Thing.DrawPos });
                            //Traverse.Create(__instance).Method("Face", new object[] { target.Thing.DrawPos });
                            return false;
                        }
                    }
                    else
                    {
                        if (thisPawn.Position.AdjacentTo8Way(target.Cell))
                        {
                            if (DebugViewSettings.drawPawnRotatorTarget)
                            {
                                thisPawn.Map.debugDrawer.FlashCell(thisPawn.Position, 0.2f, "jbloc");
                                GenDraw.DrawLineBetween(thisPawn.Position.ToVector3Shifted(),
                                    target.Cell.ToVector3Shifted());
                            }
                            //this.FaceAdjacentCell(target.Cell);
                            AccessTools.Method(typeof(Pawn_RotationTracker), "FaceAdjacentCell")
                                .Invoke(__instance, new object[] { target.Cell });
                            //Traverse.Create(__instance).Method("FaceAdjacentCell", new object[] { target.Cell });
                            return false;
                        }
                        if (target.Cell.IsValid && target.Cell != thisPawn.Position)
                        {
                            //this.Face(target.Cell.ToVector3());
                            //Traverse.Create(__instance).Method("Face", new object[] { target.Cell.ToVector3() });
                            AccessTools.Method(typeof(Pawn_RotationTracker), "Face")
                                .Invoke(__instance, new object[] { target.Cell.ToVector3() });
                            return false;
                        }
                    }
                }
                //if (thisPawn.Drafted)
                //{
                //    thisPawn.Rotation = compVehicle.lastDirection;
                //}
                return false;
            }
            return true;
        }
    }
}
