using HarmonyLib;
using System;
using UnityModManagerNet;

namespace DvMod.RadioBridge
{
    [EnableReloading]
    public static class Main
    {
        public static UnityModManager.ModEntry? mod;
        public static Settings settings = new Settings();

        static public bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;

            try
            {
                Settings loaded = Settings.Load<Settings>(modEntry);
                settings = loaded.version == modEntry.Info.Version ? loaded : new Settings();
            }
            catch
            {
                settings = new Settings();
            }

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            return true;
        }

        static private void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw();
        }

        static private void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        static private bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            if (value)
            {
                harmony.PatchAll();
                Commands.Register();
                HttpServer.Create();
            }
            else
            {
                HttpServer.Destroy();
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }

        public static void DebugLog(string msg)
        {
            if (settings.enableLogging)
                mod!.Logger.Log(msg);
        }

        public static void DebugLog(Func<string> msg)
        {
            if (settings.enableLogging)
                DebugLog(msg());
        }

        public static void DebugLog(Func<string> msg, Exception e)
        {
            if (settings.enableLogging)
                mod!.Logger.LogException(msg(), e);
        }
    }
}
