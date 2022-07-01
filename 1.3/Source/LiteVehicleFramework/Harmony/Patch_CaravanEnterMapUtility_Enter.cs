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
    [HarmonyPatch(typeof(CaravanEnterMapUtility), "Enter")]
    public static class Patch_CaravanEnterMapUtility_Enter
    {
        [HarmonyPrefix]
        public static void Prefix(Caravan caravan)
        {
            var members = caravan.PawnsListForReading;
            for (var i = 0; i < members.Count; i++)
            {
                var vehicle = members[i].GetComp<Comp_Vehicle>();
                if (vehicle != null && vehicle.PawnsInVehicle != null && vehicle.PawnsInVehicle.Count > 0)
                {
                    for (var j = 0; j < members.Count; j++)
                    {
                        for (var l = 0; l < vehicle.PawnsInVehicle.Count; l++)
                        {
                            var group = vehicle.PawnsInVehicle[l];
                            for (var k = 0; k < group.handlers.Count; k++)
                            {
                                var pawn = group.handlers[k];
                                if (pawn == members[j])
                                {
                                    foreach (var vgroup in vehicle.handlers)
                                    {
                                        if (vgroup.role.label == group.role.label)
                                        {
                                            vgroup.handlers.TryAdd(pawn);
                                            caravan.RemovePawn(pawn);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    vehicle.PawnsInVehicle = null;
                }
            }
        }
    }
}
