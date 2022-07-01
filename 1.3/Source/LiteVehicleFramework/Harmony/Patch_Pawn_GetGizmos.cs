using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        static bool Prefix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.def.HasComp(typeof(Comp_Vehicle)) && (__instance.Faction == null || !__instance.Faction.IsPlayer))
            {
                Comp_Vehicle comp = __instance.TryGetComp<Comp_Vehicle>();
                {
                    Command_Action com = new Command_Action();
                    com.icon = ContentFinder<Texture2D>.Get("UI/Designators/Claim", true);
                    com.defaultLabel = "DesignatorClaim".Translate();
                    com.defaultDesc = "DesignatorClaimDesc".Translate();

                    com.action = delegate ()
                    {
                        __instance.SetFaction(Faction.OfPlayer);
                        __instance.def.ResolveReferences();
                        comp.isClaimable = false;
                    };

                    __result.AddItem(com);
                }
            }
            return true;
        }
    }
}
