using DV.Radio;
using HarmonyLib;
using System;

namespace DvMod.RadioBridge
{
    [HarmonyPatch(typeof(RadioPlayer), "LogNoMoreData")]
    static class StreamRecovery
    {
        static DateTime startTime;
        static DateTime lastTime;

        public static bool Prefix()
        {
            if (lastTime == null)
                lastTime = DateTime.Now;

            if (startTime == null)
            {
                startTime = lastTime;
                return false; // skip the original method in which playback is stopped
            }

            DateTime now = DateTime.Now;

            if ((now - lastTime).TotalMilliseconds > Main.settings.recoveryReset)
            {
                startTime = lastTime = now;
                return false; // skip the original method in which playback is stopped
            }

            lastTime = now;

            if ((now - startTime).TotalMilliseconds < Main.settings.recoveryTimeout * 1000)
            {
                return false; // skip the original method in which playback is stopped
            }

            // Reaching this point means the recovery timeout has been exceeded
            return true; // run the original method in which playback is stopped
        }
    }
}
