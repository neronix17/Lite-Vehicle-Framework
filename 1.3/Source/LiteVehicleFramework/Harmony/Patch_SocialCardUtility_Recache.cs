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
    [HarmonyPatch(typeof(SocialCardUtility), "Recache")]
    public static class Patch_SocialCardUtility_Recache
    {
        [HarmonyPrefix]
        public static bool SocialTabNullHandling(Pawn selPawnForSocialInfo)
        {
            if (selPawnForSocialInfo == null || selPawnForSocialInfo.relations == null)
            {
                return false;
            }
            return true;
        }
    }
}
