using Crosstales.NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace DvMod.RadioBridge
{
    public class Recording
    {
        private readonly WaveInEvent waveIn = new WaveInEvent();
        private Encoder? encoder;
        private readonly Stream output;

        public Recording(Stream outputStream)
        {
            output = outputStream;
        }

        public static string[] GetDeviceNames()
        {
            var names = new string[WaveInEvent.DeviceCount];
            for (int i = 0; i < names.Length; i++)
                names[i] = WaveInEvent.GetCapabilities(i).ProductName;
            return names;
        }

        private static int FindInputDevice()
        {
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                if (deviceInfo.ProductName.Equals(Main.settings.inputDeviceName))
                    return i;
            }
            return -1;
        }

        public void BeginCapture()
        {
            var deviceId = FindInputDevice();
            if (deviceId < 0)
                throw new Exception($"Could not find input device: \"{Main.settings.inputDeviceName}\"");
            encoder = new Encoder();
            waveIn.DeviceNumber = deviceId;
            waveIn.WaveFormat = new WaveFormat(48000, 2);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();
            Main.DebugLog(() => $"Started recording from device {waveIn.DeviceNumber} ({WaveInEvent.GetCapabilities(waveIn.DeviceNumber).ProductName})");
        }

        public void StopCapture()
        {
            waveIn.StopRecording();
            encoder!.Flush(output!);
            Main.DebugLog(() => $"Stopped recording from device {waveIn.DeviceNumber} ({WaveInEvent.GetCapabilities(waveIn.DeviceNumber).ProductName})");
        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            // Main.DebugLog(() => $"Got buffer with {args.BytesRecorded} bytes of PCM WAV data");
            encoder!.Encode(args.Buffer, args.BytesRecorded, output);
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs args)
        {
            output.Close();
            if (args.Exception != null && !(args.Exception is SocketException))
                Main.DebugLog(() => "Recording stopped with exception:", args.Exception);
        }
    }
}
