using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace LiteVehicles
{
    [DefOf]
    public static class LiteVehiclesDefOf
    {
        static LiteVehiclesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(LiteVehiclesDefOf));
        }
        
        public static JobDef LVF_JobDriver_RepairVehicle;

        public static FleshTypeDef LVF_VehicleFlesh;
    }
}
