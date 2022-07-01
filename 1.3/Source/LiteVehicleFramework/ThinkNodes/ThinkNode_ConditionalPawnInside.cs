using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace LiteVehicles
{
    public class ThinkNode_ConditionalPawnInside : ThinkNode_Conditional
    {
        //If anyone is inside the cockpit, allow this action to take place.
        public override bool Satisfied(Pawn pawn)
        {
            if (pawn?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle &&
                ((compVehicle?.AllOccupants?.Count ?? 0) > 0 ||
                 compVehicle.manipulationStatus == ManipulationState.able)) return true;
            return false;
        }
    }
}