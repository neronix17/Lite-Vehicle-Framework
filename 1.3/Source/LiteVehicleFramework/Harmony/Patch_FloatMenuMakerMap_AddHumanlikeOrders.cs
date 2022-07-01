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

namespace LiteVehicles
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class Patch_FloatMenuMakerMap_AddHumanlikeOrders
    {
        [HarmonyPrefix]
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            var c = IntVec3.FromVector3(clickPos);
            var groundPawn = c.GetThingList(pawn.Map).FirstOrDefault(x => x is Pawn) as Pawn;
            if (groundPawn != null)
                if (groundPawn?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle)
                {
                    var toCheck = "Rescue".Translate(groundPawn.Label);
                    var optToRemove = opts.FirstOrDefault(x => x.Label.Contains(toCheck));
                    if (optToRemove != null) opts.Remove(optToRemove);
                }
        }
    }
}
