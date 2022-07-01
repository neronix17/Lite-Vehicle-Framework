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
    [StaticConstructorOnStartup]
    public static class VehicleUtil
    {
        static public bool IsVehicle(this FleshTypeDef fleshType)
        {
            return fleshType == LiteVehiclesDefOf.LVF_VehicleFlesh;
        }
    }
}
