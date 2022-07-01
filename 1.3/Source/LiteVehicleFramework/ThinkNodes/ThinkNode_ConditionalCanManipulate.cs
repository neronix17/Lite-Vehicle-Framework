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
    public class ThinkNode_ConditionalCanManipulate : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            if (pawn?.GetComp<Comp_Vehicle>() is Comp_Vehicle compVehicle &&
                compVehicle.manipulationStatus == ManipulationState.able) return true;
            return false;
        }
    }
}