using CommandTerminal;
using Crosstales.NAudio.Wave;
using Crosstales.NAudio.Wave.Compression;
using HarmonyLib;
using System;
using System.IO;

namespace DvMod.RadioBridge
{
    public static class Commands
    {
        [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
        public static class RegisterCommandsPatch
        {
            public static void Postfix()
            {
                Register();
            }
        }

        public static void Register(CommandInfo command)
        {
            command.name = Main.mod!.Info.Id + "." + command;
            if (Terminal.Shell == null)
                return;
            if (Terminal.Shell.Commands.Remove(command.name.ToUpper()))
            {
                Main.DebugLog($"replacing existing command {command.name}");
                Terminal.Autocomplete.known_words.Remove(command.name.ToLower());
            }
            else
            {
                Terminal.Autocomplete.Register(command);
            }
            Terminal.Shell.AddCommand(command);
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
            Commands.Register(new CommandInfo
            {
                name = "enumerateaudiocapture",
                proc = _ =>
                {
                    for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                    {
                        var deviceInfo = WaveInEvent.GetCapabilities(i);
                        Terminal.Log($"Device #{i}: {deviceInfo.ProductName}");
                    }
                },
            });

            Commands.Register(new CommandInfo
            {
                name = "enumerateacm", proc = _ =>
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
                },
            });

            Recording? recording = null;

            Commands.Register(new CommandInfo
            {
                name = "startrecording",
                proc = _ =>
                {
                    recording = new Recording(File.Create(Path.Combine(Main.mod!.Path, $"output-{DateTime.Now:s}.mp3")));
                    recording.BeginCapture();
                },
            });

            Commands.Register(new CommandInfo
            {
                name = "stoprecording",
                proc = _ =>
                {
                    recording?.StopCapture();
                    recording = null;
                },
            });
        }
    }
}
