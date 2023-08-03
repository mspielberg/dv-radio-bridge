using System;
using UnityEngine;
using UnityModManagerNet;

namespace DvMod.RadioBridge
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public string inputDeviceName = "CABLE Output (VB-Audio Virtual ";
        [Draw("HTTP server port")]
        public int serverPort = 7100;
        [Draw("Enable logging")]
        public bool enableLogging = true;
        [Draw("Seconds to wait for the data stream to recover before stopping playback")]
        public int recoveryTimeout = 2;
        [Draw("Milliseconds after which the recovery timeout will be reset if no errors have occurred")]
        public int recoveryReset = 500;

        public readonly string? version = Main.mod?.Info.Version;

        public void Draw()
        {
            GUILayout.Label("Input device:");
            var deviceNames = Recording.GetDeviceNames();
            var currentIndex = Math.Max(0, Array.FindIndex(deviceNames, x => x == inputDeviceName));
            GUILayout.BeginVertical("box");
            if (UnityModManager.UI.ToggleGroup(ref currentIndex, deviceNames))
            {
                inputDeviceName = deviceNames[currentIndex];
                HttpServer.Destroy();
                HttpServer.Create();
            }
            GUILayout.EndVertical();
            this.Draw(Main.mod);
        }

        override public void Save(UnityModManager.ModEntry entry)
        {
            Save(this, entry);
        }

        public void OnChange()
        {
        }
    }
}
