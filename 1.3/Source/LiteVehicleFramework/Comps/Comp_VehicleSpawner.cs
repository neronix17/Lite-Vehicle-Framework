using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace LiteVehicles
{
    public class Comp_VehicleSpawner : ThingComp
    {
        /// Get the thing that is creating the vehicle.
        public ThingWithComps Spawner => parent;

        /// Use the XML configurations.
        public CompProperties_VehicleSpawner Props => props as CompProperties_VehicleSpawner;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (Props.assemblyTime <= 0f)
            {
                Notify_Assembled();
            }
        }

        /// Adds a right click option to unpack and spawn the vehicle.
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (var o in base.CompFloatMenuOptions(selPawn))
            {
                yield return o;
            }

            if (!Spawner.DestroyedOrNull() && Spawner.Spawned && !selPawn.DestroyedOrNull() && selPawn.Spawned)
            {
                yield return new FloatMenuOption(string.Format(Props.useVerb, Spawner.Label), delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(new Job(DefDatabase<JobDef>.GetNamed("CompVehicle_Assemble"),
                        Spawner));
                });
            }
        }

        /// When assembled, be sure to spawn the vehicle and destroy this object.
        public void Notify_Assembled()
        {
            Pawn pawn = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(Props.vehicleToSpawn, Faction.OfPlayer), Spawner.PositionHeld, Spawner.MapHeld);
            Spawner.Destroy(DestroyMode.KillFinalize);
        }
    }
}
