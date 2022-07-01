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

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Neronix17.LiteVehicles").RootDir.FullName, "Version.txt");
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
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Lite Vehicle Framework";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }
    }
}
