using HarmonyLib;
using System;
using UnityModManagerNet;

namespace DvMod.RadioBridge
{
    [EnableReloading]
    public static class Main
    {
        public static UnityModManager.ModEntry? mod;

        static public bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;

            modEntry.OnToggle = OnToggle;

            return true;
        }

        static private bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            if (value)
            {
                harmony.PatchAll();
                Commands.Register();

            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }

        public static void DebugLog(string msg)
        {
            mod!.Logger.Log(msg);
        }

        public static void DebugLog(Func<string> msg)
        {
            DebugLog(msg());
        }

        public static void DebugLog(Func<string> msg, Exception e)
        {
            mod!.Logger.LogException(msg(), e);
        }
    }
}
