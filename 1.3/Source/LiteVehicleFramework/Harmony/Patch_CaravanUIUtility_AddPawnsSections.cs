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
    [HarmonyPatch(typeof(CaravanUIUtility), "AddPawnsSections")]
    public static class Patch_CaravanUIUtility_AddPawnsSections
    {
        [HarmonyPostfix]
        public static void Postfix(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
            var source = from x in transferables where x.ThingDef.category == ThingCategory.Pawn select x;
            widget.AddSection("CompVehicle_VehicleSection".Translate(), from x in source where ((Pawn)x.AnyThing).GetComp<Comp_Vehicle>() != null && ((Pawn)x.AnyThing).GetComp<Comp_Vehicle>().MovementHandlerAvailable select x);
        }
    }
}
