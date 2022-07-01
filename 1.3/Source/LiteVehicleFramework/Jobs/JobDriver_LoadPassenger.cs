using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using System.Diagnostics;

namespace LiteVehicles
{
    public class JobDriver_LoadPassenger : JobDriver
    {
        private readonly TargetIndex TransporterInd = TargetIndex.A;

        private Comp_Vehicle Vehicle
        {
            get
            {
                var thing = job.GetTarget(TransporterInd).Thing;
                if (thing == null)
                    return null;
                return thing.TryGetComp<Comp_Vehicle>();
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TransporterInd);
            //this.FailOn(() => !this.<> f__this.Transporter.LoadingInProgressOrReadyToLaunch);
            yield return Toils_Reserve.Reserve(TransporterInd, 1, -1, null);
            yield return Toils_Goto.GotoThing(TransporterInd, PathEndMode.Touch);
            yield return new Toil
            {
                initAction = delegate
                {
                    var vehicle = Vehicle;
                    vehicle.Notify_Loaded(pawn);
                }
            };
        }
    }
}