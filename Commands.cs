using CommandTerminal;
using Crosstales.NAudio.Wave;
using Crosstales.NAudio.Wave.Compression;
using HarmonyLib;
using System;

namespace DvMod.RadioBridge
{
    public static class Commands
    {
        [HarmonyPatch(typeof(Terminal), nameof(Terminal.Start))]
        public static class RegisterCommandsPatch
        {
            public static void Postfix()
            {
                Register();
            }
        }

        public static void Register(string name, Action<CommandArg[]> proc)
        {
            name = Main.mod!.Info.Id + "." + name;
            if (Terminal.Shell == null)
                return;
            if (Terminal.Shell.Commands.Remove(name.ToUpper()))
                Main.DebugLog($"replacing existing command {name}");
            else
                Terminal.Autocomplete.Register(name);
            Terminal.Shell.AddCommand(name, proc);
        }

        public static void Register()
        {
            NAudioCommands.Register();
        }
    }

    public static class NAudioCommands
    {
        public static void Register()
        {
            Commands.Register("enumerateaudiocapture", _ =>
            {
                for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var deviceInfo = WaveInEvent.GetCapabilities(i);
                    Terminal.Log($"Device #{i}: {deviceInfo.ProductName}");
                }
            });

            Commands.Register("enumerateacm", _ =>
            {
                foreach (var driver in AcmDriver.EnumerateAcmDrivers())
                {
                    using (driver)
                    {
                        Terminal.Log($"{driver.DriverId}: {driver.ShortName} ({driver.LongName})");
                        driver.Open();
                        foreach (var formatTag in driver.FormatTags)
                        {
                            Terminal.Log($"FormatTag: {formatTag}");
                        }
                    }
                }
            });

            Commands.Register("startrecording", _ =>
            {
                Recording.BeginCapture();
            });

            Commands.Register("stoprecording", _ =>
            {
                Recording.StopCapture();
            });
        }
    }
}
