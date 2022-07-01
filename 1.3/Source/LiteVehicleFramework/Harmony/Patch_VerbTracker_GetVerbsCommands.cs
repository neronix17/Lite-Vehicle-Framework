using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;

namespace LiteVehicles
{
    [HarmonyPatch(typeof(VerbTracker), "GetVerbsCommands")]
    public static class Patch_VerbTracker_GetVerbsCommands
    {
        [HarmonyPrefix]
        public static bool Prefix(VerbTracker __instance, ref IEnumerable<Command> __result, KeyCode hotKey = 0)
        {
            __result = GetVerbsCommands(__instance);
            return false;
        }

        public static IEnumerable<Command> GetVerbsCommands(VerbTracker __instance)
        {
            if (__instance.directOwner is CompEquippable ce)
            {
                Thing ownerThing = ce.parent;
                if (ownerThing?.def != null)
                {
                    List<Verb> verbs = __instance.AllVerbs;
                    foreach (Verb verb in verbs)
                    {
                        if (verb != null && verb.verbProps.hasStandardCommand && verb.CasterPawn != null && (!verb.CasterIsPawn || verb.CasterPawn.story != null))
                        {
                            yield return (Verse.Command)AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand").Invoke(__instance, new object[] { ownerThing, verb });
                        }
                    }

                    var meleeAttack = verbs.FirstOrDefault(x => x.verbProps.IsMeleeAttack);
                    if (!__instance.directOwner.Tools.NullOrEmpty() && ce.parent.def.IsMeleeWeapon && meleeAttack?.CasterPawn != null && (!meleeAttack.CasterIsPawn || meleeAttack.CasterPawn.story != null))
                    {
                        yield return (Verse.Command)AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand").Invoke(__instance, new object[] { ownerThing, meleeAttack });
                    }
                }
            }
        }
    }
}
