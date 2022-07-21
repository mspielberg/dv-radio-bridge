using Crosstales.NAudio.Wave;
using Crosstales.NAudio.Wave.Compression;
using System;
using System.IO;

namespace DvMod.RadioBridge
{
    public static class Recording
    {
        private static readonly WaveInEvent waveIn = new WaveInEvent();
        private static Encoder? encoder;
        private static Stream? output;

        private static int FindVirtualCableDevice()
        {
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                if (deviceInfo.ProductName.StartsWith("CABLE Output"))
                    return i;
            }
            return -1;
        }

        public static void BeginCapture()
        {
            var deviceId = FindVirtualCableDevice();
            if (deviceId < 0)
                throw new Exception("Could not find Virtual Audio Cable device");
            output = File.Create(Path.Combine(Main.mod.Path, "output.mp3"));
            encoder = new Encoder();
            waveIn.DeviceNumber = deviceId;
            waveIn.WaveFormat = new WaveFormat(48000, 2);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();
            Main.DebugLog(() => $"Started recording from device {waveIn.DeviceNumber} ({WaveInEvent.GetCapabilities(waveIn.DeviceNumber).ProductName})");
        }

        public static void StopCapture()
        {
            waveIn.StopRecording();
            encoder!.Flush(output!);
            Main.DebugLog(() => $"Stopped recording from device {waveIn.DeviceNumber} ({WaveInEvent.GetCapabilities(waveIn.DeviceNumber).ProductName})");
        }

        private static void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            Main.DebugLog(() => $"Got buffer with {args.BytesRecorded} bytes of PCM WAV data");
            encoder!.Encode(args.Buffer, args.BytesRecorded, output!);
        }

        private static void OnRecordingStopped(object sender, StoppedEventArgs args)
        {
            output!.Close();
            if (args.Exception != null)
                Main.DebugLog(() => "Recording stopped with exception:", args.Exception);
        }
    }
}
