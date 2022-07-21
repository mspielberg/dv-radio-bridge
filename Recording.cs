using Crosstales.NAudio.Wave;
using Crosstales.NAudio.Wave.Compression;
using System;
using System.IO;

namespace DvMod.RadioBridge
{
    public class Recording
    {
        private readonly WaveInEvent waveIn = new WaveInEvent();
        private Encoder? encoder;
        private Stream? output;

        public Recording(Stream outputStream)
        {
            output = outputStream;
        }

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

        public void BeginCapture()
        {
            var deviceId = FindVirtualCableDevice();
            if (deviceId < 0)
                throw new Exception("Could not find Virtual Audio Cable device");
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
            Main.DebugLog(() => $"Got buffer with {args.BytesRecorded} bytes of PCM WAV data");
            encoder!.Encode(args.Buffer, args.BytesRecorded, output!);
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs args)
        {
            output!.Close();
            if (args.Exception != null)
                Main.DebugLog(() => "Recording stopped with exception:", args.Exception);
        }
    }
}
