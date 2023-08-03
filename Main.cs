using HarmonyLib;
using System;
using UnityModManagerNet;

namespace DvMod.RadioBridge
{
    [EnableReloading]
    public static class Main
    {
        public static UnityModManager.ModEntry? mod;
        public static Settings Settings { get; private set; } = new Settings();

        static public bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            try { Settings = Settings.Load<Settings>(modEntry); } catch {}

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            return true;
        }

        static private void OnGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Draw();
        }

        static private void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
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
            if (Settings.enableLogging)
                mod!.Logger.Log(msg);
        }

        public static void DebugLog(Func<string> msg)
        {
            if (Settings.enableLogging)
                DebugLog(msg());
        }

        public static void DebugLog(Func<string> msg, Exception e)
        {
            if (Settings.enableLogging)
                mod!.Logger.LogException(msg(), e);
        }
    }
}
