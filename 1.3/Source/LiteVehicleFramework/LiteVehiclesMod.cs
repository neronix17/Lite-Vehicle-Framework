using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LiteVehicles
{
    public class LiteVehiclesMod : Mod
    {
        public static LiteVehiclesMod mod;
        public static LiteVehiclesSettings settings;
        public static Harmony harmony;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Neronix17.LiteVehicleFramework").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public LiteVehiclesMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<LiteVehiclesSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);

            harmony = new Harmony("Neronix17.LiteVehicleFramework.RimWorld");
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Allow Tool"))
            {
                harmony.Patch(AccessTools.Method("AllowTool.PartyHuntController:TryGetGizmo"), new HarmonyMethod(typeof(Harmony), nameof(CompatibilityUtil.GizmoCompatability_Prefix)));
            }
            // Compat Patch for IgniteEverything
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Ignite Everything"))
            {
                harmony.Patch(AccessTools.Method("IgniteEverything.Harmony_PawnAttackGizmoUtility:GetAttackGizmos_Postfix"), new HarmonyMethod(typeof(Harmony), nameof(CompatibilityUtil.GizmoCompatability_Prefix)));
            }
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Lite Vehicle Framework";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }
    }
}
