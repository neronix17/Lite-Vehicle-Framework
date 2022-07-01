using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LiteVehicles
{
    public class VehicleLoadingBill : IExposable
    {
        public VehicleHandlerGroup group;
        public Pawn pawnToLoad;

        public VehicleLoadingBill()
        {
        }

        public VehicleLoadingBill(Pawn newLoad, Pawn newVehicle, VehicleHandlerGroup newGroup)
        {
            pawnToLoad = newLoad;
            group = newGroup;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref pawnToLoad, "pawnToLoad");
            Scribe_References.Look(ref group, "group");
        }
    }
}
