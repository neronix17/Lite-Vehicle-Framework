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
        
        public static JobDef O21_JobDriver_RepairVehicle;
    }
}
