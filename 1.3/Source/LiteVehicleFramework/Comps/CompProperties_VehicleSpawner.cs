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
    public class CompProperties_VehicleSpawner : CompProperties
    {
        public float assemblyTime = 20f; //In seconds
        public string useVerb = "Assemble {0}";
        public PawnKindDef vehicleToSpawn = null;
        public EffecterDef workEffect = null;

        public CompProperties_VehicleSpawner()
        {
            compClass = typeof(Comp_VehicleSpawner);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (workEffect == null)
            {
                workEffect = EffecterDefOf.ConstructMetal;
            }
        }
    }
}
